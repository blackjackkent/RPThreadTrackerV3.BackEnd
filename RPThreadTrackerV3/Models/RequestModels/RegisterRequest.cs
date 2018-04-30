namespace RPThreadTrackerV3.Models.RequestModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Exceptions.Account;

    public class RegisterRequest
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

	    public void AssertIsValid()
	    {
	        var errors = new List<string>();
	        if (string.IsNullOrWhiteSpace(Username))
	        {
                errors.Add("You must provide a username.");
	        }

	        if (string.IsNullOrWhiteSpace(Email))
	        {
                errors.Add("You must provide a valid email address.");
	        }

	        if (string.IsNullOrWhiteSpace(Password)
	            || string.IsNullOrWhiteSpace(ConfirmPassword))
	        {
	            errors.Add("You must provide a password.");
            }

	        if (!string.Equals(Password, ConfirmPassword))
	        {
                errors.Add("Your passwords must match.");
	        }

	        if (errors.Any())
	        {
                throw new InvalidRegistrationException(errors);
	        }
	    }
	}
}
