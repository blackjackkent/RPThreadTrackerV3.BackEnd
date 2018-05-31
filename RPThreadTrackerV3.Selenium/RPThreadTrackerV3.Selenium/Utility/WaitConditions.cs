namespace RPThreadTrackerV3.Selenium.Utility
{
	using System;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;

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

	    public static Func<IWebDriver, bool> ElementHasValue(IWebElement element, string expectedValue = null)
	    {
		    return (driver) =>
		    {
			    try
			    {
			        if (expectedValue == null)
			        {
			            return !string.IsNullOrEmpty(element.GetAttribute("value"));
			        }
				    return element.GetAttribute("value") == expectedValue;
			    }
			    catch (Exception)
			    {
				    return false;
			    }
		    };
	    }

        public static Func<IWebDriver, bool> ElementHasText(IWebElement element, string expectedValue = null)
        {
            return (driver) =>
            {
                try
                {
                    if (expectedValue == null)
                    {
                        return !string.IsNullOrEmpty(element.Text);
                    }
                    return element.Text == expectedValue;
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }
    }
}
