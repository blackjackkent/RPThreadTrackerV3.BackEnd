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
	    protected readonly IJavaScriptExecutor _executor;

		protected BasePage(ChromeDriver driver, IConfigurationRoot config)
	    {
		    _driver = driver;
		    _config = config;
		    _executor = _driver;
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

	    public void WaitForElementToBeFocused(IWebElement element)
	    {
		    var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(5000));
		    wait.Until(WaitConditions.ElementIsFocused(element));
		}

	    public void WaitForElementToBeBlurred(IWebElement element)
	    {
		    var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(5000));
		    wait.Until(WaitConditions.ElementIsBlurred(element));
	    }

		public void TriggerFocus(IWebElement element)
	    {
		    _executor.ExecuteScript("arguments[0].focus(); return true", element);
		    WaitForElementToBeFocused(element);
	    }

	    public void TriggerBlur(IWebElement element)
	    {
		    _executor.ExecuteScript("arguments[0].blur(); return true", element);
			WaitForElementToBeBlurred(element);
	    }

		public By ByDataSpec(string value)
	    {
		    return By.CssSelector("[data-spec=\"" + value + "\"]");
	    }
	}
}
