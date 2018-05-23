namespace RPThreadTrackerV3.Selenium.Utility
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;

	public static class DriverExtensionMethods
    {
	    public static IWebElement FindElementByDataSpec(this ChromeDriver driver, string value)
	    {
		    return driver.FindElementByCssSelector("[data-spec=\"" + value + "\"]");
	    }
	}
}
