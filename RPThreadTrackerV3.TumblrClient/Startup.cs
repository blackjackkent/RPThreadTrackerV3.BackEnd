namespace RPThreadTrackerV3.TumblrClient
{
	using Infrastructure.Interfaces;
	using Infrastructure.Providers;
	using Infrastructure.Services;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using NLog;
	using NLog.Extensions.Logging;
	using NLog.Web;

	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<ITumblrClient, TumblrSharpClient>(); 
			services.AddScoped<GlobalExceptionHandler>();
			services.AddMvc();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
	        loggerFactory.AddNLog();
	        app.AddNLogWeb();
			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
	        LogManager.Configuration.Variables["connectionString"] = Configuration["Data:ConnectionString"];
		}
    }
}
