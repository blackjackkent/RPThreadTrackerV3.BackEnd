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
		private IHostingEnvironment _env;
		private IConfigurationRoot _config { get; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			_env = env;
			_config = builder.Build();
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddSingleton(_config);
			var connection = _config["Data:ConnectionString"];
			services.AddDbContext<BudgetContext>(options => options.UseSqlServer(connection));
			//services.AddScoped<IRepository<Budget>, BudgetRepository>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IPasswordHasher<IdentityUser>, CustomPasswordHasher>(); 
			services.AddScoped<GlobalExceptionHandler>();
			services.AddCors();
			services.AddMvc();
			services.AddAutoMapper();

			services.AddIdentity<IdentityUser, IdentityRole>(config =>
				{
					config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
					{
						OnRedirectToLogin = (ctx) =>
						{
							if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
							{
								ctx.Response.StatusCode = 401;
							}
							return Task.CompletedTask;
						},
						OnRedirectToAccessDenied = (ctx) =>
						{
							if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
							{
								ctx.Response.StatusCode = 403;
							}
							return Task.CompletedTask;
						}
					};
				})
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<BudgetContext>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();

			app.AddNLogWeb();
			app.UseIdentity();
			app.UseJwtBearerAuthentication(new JwtBearerOptions
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = _config["Tokens:Issuer"],
					ValidAudience = _config["Tokens:Audience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])),
					ValidateLifetime = true
				}
			});
			app.UseCors(builder =>
				builder.WithOrigins("http://localhost:1989").AllowAnyHeader().AllowAnyMethod());
			app.UseMvc();
			LogManager.Configuration.Variables["connectionString"] = _config["Data:ConnectionString"];
			//roleInitializer.Seed().Wait();
		}
	}
}
