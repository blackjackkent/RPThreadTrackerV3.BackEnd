namespace RPThreadTrackerV3.Selenium
{
    using FluentAssertions;
	using Utility;
	using Utility.PageObjects;
    using Xunit;

    [Trait("Category", "Register")]
	public class RegisterTests : BaseTest
	{
		public class PreventsFormSubmissionWithEmptyUsername : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
                page.EnterUsername("");
				page.Button.Click();
                page.WaitForElementToBeVisible(page.UsernameError);
				page.UsernameError.Text.Should().Be("You must enter a username.");
			}
	    }

	    public class PreventsFormSubmissionWithInvalidUsername : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterUsername("aa");
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.UsernameError);
	            page.UsernameError.Text.Should().Be("Your username must be more than 3 characters.");
	        }
	    }

        public class PreventsFormSubmissionWithEmptyEmail : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
                page.EnterEmail("");
                page.Button.Click();
                page.WaitForElementToBeVisible(page.EmailError);
				page.EmailError.Text.Should().Be("You must enter an email.");
			}
	    }

	    public class PreventsFormSubmissionWithInvalidEmail : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterEmail("aaa");
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.EmailError);
	            page.EmailError.Text.Should().Be("Please enter a valid email.");
	        }
	    }

        public class PreventsFormSubmissionWithEmptyPassword : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
				page.EnterPassword("");
                page.Button.Click();
                page.WaitForElementToBeVisible(page.PasswordError);
				page.PasswordError.Text.Should().Be("You must enter a password.");
			}
	    }

	    public class PreventsFormSubmissionWithInvalidPassword : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterPassword("aaaaa");
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.PasswordError);
	            page.PasswordError.Text.Should().Be("Your password must be longer than 6 characters.");
	        }
	    }

        public class PreventsFormSubmissionWithEmptyConfirmPassword : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
                page.EnterConfirmPassword("");
                page.Button.Click();
                page.WaitForElementToBeVisible(page.ConfirmPasswordError);
				page.ConfirmPasswordError.Text.Should().Be("You must confirm your password.");
			}
	    }

	    public class PreventsFormSubmissionWithNonMatchingPasswords : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterPassword("Test123a!");
                page.EnterConfirmPassword("Test123a");
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.ConfirmPasswordError);
	            page.ConfirmPasswordError.Text.Should().Be("Your passwords must match.");
	        }
	    }

	    public class PreventsRegistrationWithExistingUsername : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
                page.EnterUsername(_config["accountUsername"]);
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.ServerError);
	            page.ServerError.Text.Should().Be("Error creating account. An account with some or all of this information may already exist.");
	        }
	    }

	    public class PreventsRegistrationWithExistingEmail : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterEmail(_config["accountEmail"]);
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.ServerError);
	            page.ServerError.Text.Should().Be("Error creating account. An account with some or all of this information may already exist.");
	        }
	    }

	    public class PreventsRegistrationWithInsecurePassword : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            page.PopulateValidForm();
	            page.EnterPassword("testtest");
                page.EnterConfirmPassword("testtest");
	            page.Button.Click();
	            page.WaitForElementToBeVisible(page.ServerError);
	            page.ServerError.Text.Should().Be("Passwords must have at least one non alphanumeric character.");
	        }
	    }

	    public class RegistersSuccessfullyWithValidRegistration : RegisterTests
	    {
	        [Fact]
	        public void Test()
	        {
	            var page = new RegisterPage(_driver, _config);
	            var model = page.PopulateValidForm();
	            page.Button.Click();
	            var dashboard = new DashboardPage(_driver, _config);
	            dashboard.WaitForElementToHaveText(dashboard.LoggedInUserName);
	            _driver.CaptureScreenshot("registration");
                dashboard.LoggedInUserName.Text.Should().Be(model.Username);
	        }
	    }
    }
}
