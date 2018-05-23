namespace RPThreadTrackerV3.Selenium.Utility
{
	using System;
	using System.Reflection;
	using Microsoft.Extensions.Configuration;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Remote;

	public class BaseTest : IDisposable
    {
	    protected readonly ChromeDriver _driver;
	    protected IConfigurationRoot _config;

	    public BaseTest()
	    {
		    _config = new ConfigurationBuilder()
			    .AddJsonFile("appsettings.json")
			    .Build();
			_driver = new ChromeDriver(_config["chromeDriverPath"]);
		    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
			FixDriverCommandExecutionDelay(_driver);
	    }

	    public static void FixDriverCommandExecutionDelay(RemoteWebDriver driver)
	    {
		    PropertyInfo commandExecutorProperty = typeof(RemoteWebDriver).GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
		    ICommandExecutor commandExecutor = (ICommandExecutor)commandExecutorProperty.GetValue(driver);

		    FieldInfo remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);

		    if (remoteServerUriField == null)
		    {
			    FieldInfo internalExecutorField = commandExecutor.GetType().GetField("internalExecutor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
			    commandExecutor = (ICommandExecutor)internalExecutorField.GetValue(commandExecutor);
			    remoteServerUriField = commandExecutor.GetType().GetField("remoteServerUri", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
		    }

		    if (remoteServerUriField != null)
		    {
			    string remoteServerUri = remoteServerUriField.GetValue(commandExecutor).ToString();

			    string localhostUriPrefix = "http://localhost";

			    if (remoteServerUri.StartsWith(localhostUriPrefix))
			    {
				    remoteServerUri = remoteServerUri.Replace(localhostUriPrefix, "http://127.0.0.1");

				    remoteServerUriField.SetValue(commandExecutor, new Uri(remoteServerUri));
			    }
		    }
	    }

		public void Dispose()
	    {
		    _driver?.Dispose();
	    }
	}
}
