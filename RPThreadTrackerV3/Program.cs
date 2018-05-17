namespace RPThreadTrackerV3
{
	using System;
	using Infrastructure.Data.Seeders;
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	public class Program
	{
		public static void Main(string[] args)
		{
			var host = BuildWebHost(args);
			SeedDatabase(host);
			host.Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					IHostingEnvironment env = builderContext.HostingEnvironment;
					config.Sources.Clear();
					config
					    .AddJsonFile("appsettings.json", false, true)
						.AddJsonFile("appsettings.secure.json", true, true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
					    .AddEnvironmentVariables();
				})
				.Build();

		private static void SeedDatabase(IWebHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
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
}
