using Xunit;

namespace RPThreadTrackerV3.Selenium
{
	using System;
	using FluentAssertions;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Support.UI;
	using Utility;
	using Utility.PageObjects;

	[Trait("Category", "Register")]
	public class RegisterTests : BaseTest
	{
		public class ShowsFeedbackOnBlurForEmptyUsername : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.TriggerFocus(page.UsernameField);
				page.TriggerBlur(page.UsernameField);
				page.UsernameError.Text.Should().Be("You must enter a username.");
			}
		}
		
		public class ShowsFeedbackOnBlurForUsernameTooShort : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterUsername("aa");
				page.TriggerBlur(page.UsernameField);
				page.UsernameError.Text.Should().Be("Your username must be more than 3 characters.");
			}
		}

		public class HidesFeedbackOnBlurForValidUsername : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterUsername("aaaa");
				page.TriggerBlur(page.UsernameField);
				page.UsernameError.Should().BeNull();
			}
		}

		public class ShowsFeedbackOnBlurForEmptyEmail : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.TriggerFocus(page.EmailField);
				page.TriggerBlur(page.EmailField);
				page.EmailError.Text.Should().Be("You must enter an email.");
			}
		}

		public class ShowsFeedbackOnBlurForInvalidEmail : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterEmail("aa");
				page.TriggerBlur(page.EmailField);
				page.EmailError.Text.Should().Be("Please enter a valid email.");
			}
		}

		public class HidesFeedbackOnBlurForValidEmail : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterEmail("test@test.com");
				page.TriggerBlur(page.EmailField);
				page.EmailError.Should().BeNull();
			}
		}

		public class ShowsFeedbackOnBlurForEmptyPassword : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.TriggerFocus(page.PasswordField);
				page.TriggerBlur(page.PasswordField);
				page.PasswordError.Text.Should().Be("You must enter a password.");
			}
		}

		public class ShowsFeedbackOnBlurForPasswordTooShort : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterPassword("aaaaa");
				page.TriggerBlur(page.PasswordField);
				page.PasswordError.Text.Should().Be("Your password must be longer than 6 characters.");
			}
		}

		public class ShowsFeedbackOnBlurForNonMatchingPasswords : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterPassword("Test123a!");
				page.EnterConfirmPassword("Test123a");
				page.TriggerBlur(page.ConfirmPasswordField);
				page.ConfirmPasswordError.Text.Should().Be("Your passwords must match.");
			}
		}

		public class HidesFeedbackOnBlurForValidPasswords : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.EnterPassword("Test123a!");
				page.EnterConfirmPassword("Test123a!");
				page.TriggerBlur(page.PasswordField);
				page.PasswordError.Should().BeNull();
				page.ConfirmPasswordError.Should().BeNull();
			}
		}

		public class PreventsFormSubmissionWithEmptyUsername : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
				page.UsernameField.Clear();
				page.Button.Click();
				page.UsernameError.Text.Should().Be("You must enter a username.");
			}
		}

		public class PreventsFormSubmissionWithEmptyEmail : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
				page.EmailField.Clear();
				page.Button.Click();
				page.EmailError.Text.Should().Be("You must enter an email.");
			}
		}

		public class PreventsFormSubmissionWithEmptyPassword : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
				page.PasswordField.Clear();
				page.Button.Click();
				page.PasswordError.Text.Should().Be("You must enter a password.");
			}
		}

		public class PreventsFormSubmissionWithEmptyConfirmPassword : RegisterTests
		{
			[Fact]
			public void Test()
			{
				var page = new RegisterPage(_driver, _config);
				page.PopulateValidForm();
				page.ConfirmPasswordField.Clear();
				page.Button.Click();
				page.ConfirmPasswordError.Text.Should().Be("You must confirm your password.");
			}
		}
	}
}
