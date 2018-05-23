namespace RPThreadTrackerV3.Selenium.Utility.PageObjects
{
	using System;
	using Microsoft.Extensions.Configuration;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Support.UI;

	public class BasePage
    {
	    protected readonly ChromeDriver _driver;
	    protected readonly IConfigurationRoot _config;

		protected BasePage(ChromeDriver driver, IConfigurationRoot config)
	    {
		    _driver = driver;
		    _config = config;
		}

	    public void WaitForElementToExist(IWebElement element)
	    {
		    var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(5000));
		    wait.Until(WaitConditions.ElementIsVisible(element));
		}

	    public void WaitForElementToHaveValue(IWebElement element, string value)
	    {
		    var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(5000));
		    wait.Until(WaitConditions.ElementHasValue(element, value));
		}

        public void ClearField(IWebElement element)
        {
            element.SendKeys(Keys.Control + "a" + Keys.Control);
            element.SendKeys(Keys.Delete);
        }

		public By ByDataSpec(string value)
	    {
		    return By.CssSelector("[data-spec=\"" + value + "\"]");
	    }
	}
}
