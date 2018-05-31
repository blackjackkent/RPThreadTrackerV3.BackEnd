namespace RPThreadTrackerV3.Selenium.Utility.PageObjects
{
	using System;
	using System.Linq;
	using Microsoft.Extensions.Configuration;
	using Models;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;

    public class RegisterPage : BasePage
	{
		#region Elements
		public IWebElement UsernameField
		{
			get
			{
				var username = _driver.FindElementsByDataSpec("username-field").FirstOrDefault();
				return username?.FindElements(By.TagName("input")).FirstOrDefault();
			}
		}

		public IWebElement UsernameError
		{
			get
			{
				var username = _driver.FindElementsByDataSpec("username-field").FirstOrDefault();
				return username?.FindElements(By.ClassName("form-control-feedback")).FirstOrDefault();
			}
		}

		public IWebElement EmailField
		{
			get
			{
				var email = _driver.FindElementsByDataSpec("email-field").FirstOrDefault();
				return email?.FindElements(By.TagName("input")).FirstOrDefault();
			}
		}

		public IWebElement EmailError
		{
			get
			{
				var email = _driver.FindElementsByDataSpec("email-field").FirstOrDefault();
				return email?.FindElements(By.ClassName("form-control-feedback")).FirstOrDefault();
			}
		}

		public IWebElement PasswordField
		{
			get
			{
				var password = _driver.FindElementsByDataSpec("password-field").FirstOrDefault();
				return password?.FindElements(By.TagName("input")).FirstOrDefault();
			}
		}

		public IWebElement PasswordError
		{
			get
			{
				var password = _driver.FindElementsByDataSpec("password-field").FirstOrDefault();
				return password?.FindElements(By.ClassName("form-control-feedback")).FirstOrDefault();
			}
		}

		public IWebElement ConfirmPasswordField
		{
			get
			{
				var confirmPassword = _driver.FindElementsByDataSpec("confirm-password-field").FirstOrDefault();
				return confirmPassword?.FindElements(By.TagName("input")).FirstOrDefault();
			}
		}

		public IWebElement ConfirmPasswordError
		{
			get
			{
				var confirmPassword = _driver.FindElementsByDataSpec("confirm-password-field").FirstOrDefault();
				return confirmPassword?.FindElements(By.ClassName("form-control-feedback")).FirstOrDefault();
			}
		}

	    public IWebElement ServerError
	    {
	        get
	        {
	            var confirmPassword = _driver.FindElementsByDataSpec("register-server-error").FirstOrDefault();
	            return confirmPassword?.FindElements(By.TagName("span")).FirstOrDefault();
            }
	    }

		public IWebElement Button => _driver.FindElementsByClassName("btn-primary").FirstOrDefault(); 
		#endregion

		public RegisterPage(ChromeDriver driver, IConfigurationRoot config) : base(driver, config)
	    {
		    _driver.Url = config["BaseUrl"] + "/register";
		}

		public void EnterUsername(string value)
		{
			ClearField(UsernameField);
			UsernameField.SendKeys(value);
			WaitForElementToHaveValue(UsernameField, value);
		}

		public void EnterEmail(string value)
		{
			ClearField(EmailField);
			EmailField.SendKeys(value);
			WaitForElementToHaveValue(EmailField, value);
		}
		
		public void EnterPassword(string value)
		{
            ClearField(PasswordField);
			PasswordField.SendKeys(value);
			WaitForElementToHaveValue(PasswordField, value);
		}

		public void EnterConfirmPassword(string value)
		{
            ClearField(ConfirmPasswordField);
			ConfirmPasswordField.SendKeys(value);
			WaitForElementToHaveValue(ConfirmPasswordField, value);
		}

		public RegistrationModel PopulateValidForm()
		{
		    var ticks = DateTime.UtcNow.Ticks;
			var username = "blackjackkent" + ticks;
			var email = "rosalind.m.wills+" + ticks +"@gmail.com";
			EnterUsername(username);
			EnterEmail(email);
			EnterPassword(_config["accountPassword"]);
			EnterConfirmPassword(_config["accountPassword"]);
		    return new RegistrationModel
		    {
		        Username = username,
		        Email = email
		    };
		}
	}
}
