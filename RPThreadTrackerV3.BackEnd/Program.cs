// <copyright file="Program.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd
{
	using System;
	using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using AutoMapper;
    using Infrastructure.Data.Seeders;
    using Infrastructure.Providers;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using NLog;
    using NLog.Config;
	using NLog.Web;
    using RPThreadTrackerV3.BackEnd.Infrastructure.Data;
    using RPThreadTrackerV3.BackEnd.Infrastructure.Data.Entities;
    using RPThreadTrackerV3.BackEnd.Infrastructure.Services;
    using RPThreadTrackerV3.BackEnd.Interfaces.Data;
    using RPThreadTrackerV3.BackEnd.Interfaces.Services;
    using RPThreadTrackerV3.BackEnd.Models.Configuration;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// Base application bootstrapping file.
    /// </summary>
	[ExcludeFromCodeCoverage]
	public class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The application arguments.</param>
		public static void Main(string[] args)
        {
            var logger = InitLogger();
            logger.Debug("Initializing RPThreadTrackerV3.BackEnd");
            try
            {
				var host = CreateWebApp(args);
                SeedDatabase(host);
				host.Run();
			}
            catch (Exception exception)
            {
				// NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

		private static WebApplication CreateWebApp(string[] args)
        {
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfigurationRoot settings = configurationBuilder.AddJsonFile("appsettings.json", false, true).AddJsonFile("appsettings.secure.json", true, true).AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true).AddEnvironmentVariables().Build();
			builder.Configuration.AddConfiguration(settings);
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.Host.UseNLog();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var connection = settings.GetConnectionString("Database");
            builder.Services.AddDbContext<TrackerContext>(options =>
            {
                options.UseSqlServer(connection);
            });
            builder.Services.AddCors();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { options.User.AllowedUserNameCharacters = string.Empty; })
                .AddEntityFrameworkStores<TrackerContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddTransient<RoleInitializer>();
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = settings["Tokens:Issuer"],
                        ValidAudience = settings["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["Tokens:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = true;
                });

            builder.Services.AddOptions();
            builder.Services.Configure<AppSettings>(settings);

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IThreadService, ThreadService>();
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<IExporterService, ExporterService>();
            builder.Services.AddScoped<IPublicViewService, PublicViewService>();
            builder.Services.AddScoped<IEmailClient, SendGridEmailClient>();
            builder.Services.AddScoped<IRepository<Thread>, ThreadRepository>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            builder.Services.AddSingleton(typeof(IDocumentRepository<>), typeof(BaseDocumentRepository<>));
            builder.Services.AddSingleton(typeof(IDocumentClient<>), typeof(DocumentDbClient<>));
            builder.Services.AddSingleton<IEmailBuilder, EmailBuilder>();
            builder.Services.AddScoped<IPasswordHasher<IdentityUser>, CustomPasswordHasher>();
            builder.Services.AddScoped<GlobalExceptionHandlerAttribute>();
            builder.Services.AddScoped<DisableDuringMaintenanceFilterAttribute>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            app.UseCors(builder =>
                builder.WithOrigins(settings["Cors:CorsUrl"].Split(',')).AllowAnyHeader().AllowAnyMethod());
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            LogManager.Configuration.Variables["connectionString"] = settings.GetConnectionString("Database");

            return app;
        }

	    private static NLog.Logger InitLogger()
	    {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            logger.Debug("init main");
	        return logger;
	    }

		private static void SeedDatabase(WebApplication host)
		{
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var roleInitializer = services.GetRequiredService<RoleInitializer>();
                roleInitializer.Seed().Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
	}
}
