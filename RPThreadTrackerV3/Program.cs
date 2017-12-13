namespace RPThreadTrackerV3
{
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;

	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					IHostingEnvironment env = builderContext.HostingEnvironment;
					config.Sources.Clear();
					config.AddJsonFile("appsettings.json", false, true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
				})
				.Build();
	}
}
