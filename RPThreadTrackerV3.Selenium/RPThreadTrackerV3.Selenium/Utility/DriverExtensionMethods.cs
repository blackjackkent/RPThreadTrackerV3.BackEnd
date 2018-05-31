namespace RPThreadTrackerV3.Selenium.Utility
{
    using System;
    using System.Collections.ObjectModel;
    using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;

    public static class DriverExtensionMethods
    {
	    public static ReadOnlyCollection<IWebElement> FindElementsByDataSpec(this ChromeDriver driver, string value)
	    {
		    return driver.FindElementsByCssSelector("[data-spec=\"" + value + "\"]");
	    }

        public static string CaptureScreenshot(this ChromeDriver driver, String screenshotName)
        {
            try
            {
                var screenshotDriver = (ITakesScreenshot) driver;
                if (screenshotDriver == null)
                {
                    return null;
                }

                var source = screenshotDriver.GetScreenshot();
                var dest = "C:\\Users\\rwills\\" + screenshotName + DateTime.UtcNow.Ticks + ".png";
                source.SaveAsFile(dest);
                return dest;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
