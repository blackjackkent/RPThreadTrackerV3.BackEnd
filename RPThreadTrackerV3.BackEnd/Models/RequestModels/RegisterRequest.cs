// <copyright file="RegisterRequest.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Models.RequestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Exceptions.Account;

    /// <summary>
    /// Request model containing data about a user's request to register a new account.
    /// </summary>
    public class RegisterRequest
	{
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirmed password.
        /// </summary>
        /// <value>
        /// The confirmed password.
        /// </value>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Throws an exception if the registration request is not valid.
        /// </summary>
        /// <exception cref="InvalidRegistrationException">Thrown if the registration request is not valid.</exception>
        public virtual void AssertIsValid()
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

	        if (!string.Equals(Password, ConfirmPassword, StringComparison.CurrentCulture))
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
