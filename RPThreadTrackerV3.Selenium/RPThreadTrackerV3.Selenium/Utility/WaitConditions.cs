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
				    // If element is null, stale or if it cannot be located
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

	    public static Func<IWebDriver, bool> ElementIsFocused(IWebElement element)
	    {
		    return (driver) =>
		    {
			    try
			    {
				    return element.Equals(driver.SwitchTo().ActiveElement());
			    }
			    catch (Exception)
			    {
				    return false;
			    }
		    };
		}

	    public static Func<IWebDriver, bool> ElementIsBlurred(IWebElement element)
	    {
		    return (driver) =>
		    {
			    try
			    {
				    return !element.Equals(driver.SwitchTo().ActiveElement());
			    }
			    catch (Exception)
			    {
				    return false;
			    }
		    };
	    }
	}
}
