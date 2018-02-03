using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RPThreadTrackerV3.TumblrClient
{
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
			            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
						.AddJsonFile($"secureAppSettings.json", true, true);
	            })
				.Build();
    }
}
