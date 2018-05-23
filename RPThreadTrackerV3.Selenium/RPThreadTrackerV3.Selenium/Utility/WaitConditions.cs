namespace RPThreadTrackerV3.Selenium.Utility
{
	using System;
	using OpenQA.Selenium;

	public class WaitConditions
    {
	    public static Func<IWebDriver, bool> ElementIsVisible(IWebElement element)
	    {
		    return (driver) =>
		    {
			    try
			    {
				    return element.Displayed;
			    }
			    catch (Exception)
			    {
				    return false;
			    }
		    };
	    }

	    public static Func<IWebDriver, bool> ElementHasValue(IWebElement element, string expectedValue)
	    {
		    return (driver) =>
		    {
			    try
			    {
				    return element.GetAttribute("value") == expectedValue;
			    }
			    catch (Exception)
			    {
				    return false;
			    }
		    };
	    }
	}
}
