﻿using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RPThreadTrackerV3
{
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;
	using AutoMapper;
	using Infrastructure.Entities;
	using Infrastructure.Identity;
	using Infrastructure.Providers;
	using Infrastructure.Services;
	using Interfaces.Services;
	using Microsoft.AspNetCore.Authentication.Cookies;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Diagnostics;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Microsoft.IdentityModel.Tokens;
	using NLog;
	using NLog.Extensions.Logging;
	using NLog.Web;

	public class Startup
	{
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var connection = Configuration["Data:ConnectionString"];
			services.AddDbContext<TrackerContext>(options => options.UseSqlServer(connection));
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<TrackerContext>();
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
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
					};
				})
				.AddCookie(options =>
				{
					options.SlidingExpiration = true;
				});
			services.AddScoped<IAuthService, AuthService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IPasswordHasher<IdentityUser>, CustomPasswordHasher>();
			services.AddScoped<GlobalExceptionHandler>();
			services.AddCors();
			services.AddMvc();
			services.AddAutoMapper();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();

			app.AddNLogWeb();
			app.UseAuthentication();
			app.UseCors(builder =>
				builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod());
			app.UseMvc();
			LogManager.Configuration.Variables["connectionString"] = Configuration["Data:ConnectionString"];
		}
	}
}