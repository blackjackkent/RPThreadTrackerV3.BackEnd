// <copyright file="Program.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Infrastructure.Data.Seeders;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

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
			var host = BuildWebHost(args);
			SeedDatabase(host);
			host.Run();
		}

        /// <summary>
        /// Builds the web host.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        /// <returns>Web host instance.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					var env = builderContext.HostingEnvironment;
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
