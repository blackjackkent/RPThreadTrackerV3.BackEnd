// <copyright file="Startup.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd
{
    using System;
    using System.Text;
    using AutoMapper;
    using Infrastructure.Data;
    using Infrastructure.Data.Entities;
    using Infrastructure.Data.Seeders;
    using Infrastructure.Providers;
    using Infrastructure.Services;
    using Interfaces.Data;
    using Interfaces.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using NLog;
    using NLog.Extensions.Logging;
    using NLog.Web;

    /// <summary>
    /// .NET Core application startup class.
    /// </summary>
    public class Startup
	{
		private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

        /// <summary>
        /// Configures services and dependency injection for the application container.
        /// </summary>
        /// <param name="services">The application service collection.</param>
        public void ConfigureServices(IServiceCollection services)
		{
		    var connection = Configuration.GetConnectionString("Database");
			services.AddDbContext<TrackerContext>(options =>
			{
				options.UseSqlServer(connection);
				options.ReplaceService<IEntityMaterializerSource, CustomEntityMaterializerSource>();
			});
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<TrackerContext>()
				.AddDefaultTokenProviders();
			services.AddTransient<RoleInitializer>();
			services.AddAuthentication(options =>
				{
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidIssuer = Configuration["Tokens:Issuer"],
						ValidAudience = Configuration["Tokens:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
					    ClockSkew = TimeSpan.Zero
                    };
				})
				.AddCookie(options =>
				{
					options.SlidingExpiration = true;
				});
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IThreadService, ThreadService>();
			services.AddScoped<ICharacterService, CharacterService>();
			services.AddScoped<IExporterService, ExporterService>();
		    services.AddScoped<IPublicViewService, PublicViewService>();
            services.AddScoped<IEmailClient, SendGridEmailClient>();
			services.AddScoped<IRepository<Thread>, ThreadRepository>();
		    services.AddScoped<IRepository<Thread>, ThreadRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
		    services.AddSingleton(typeof(IDocumentRepository<>), typeof(BaseDocumentRepository<>));
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IEmailBuilder, EmailBuilder>();
			services.AddScoped<IPasswordHasher<IdentityUser>, CustomPasswordHasher>();
			services.AddScoped<GlobalExceptionHandlerAttribute>();
		    services.AddScoped<DisableDuringMaintenanceFilterAttribute>();
			services.AddCors();
			services.AddMvc();
			services.AddAutoMapper();
		}

        /// <summary>
        /// Configures the application's HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();

			app.AddNLogWeb();
			app.UseAuthentication();
			app.UseCors(builder =>
				builder.WithOrigins(Configuration["CorsUrl"]).AllowAnyHeader().AllowAnyMethod());
			app.UseMvc();
			LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("Database");
		}
	}
}
