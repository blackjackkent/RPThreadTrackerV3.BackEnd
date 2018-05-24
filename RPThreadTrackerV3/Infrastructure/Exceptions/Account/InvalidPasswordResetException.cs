// <copyright file="InvalidPasswordResetException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The exception that is thrown when there was an error resetting an unauthenticated user's password.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidPasswordResetException : Exception
    {
        /// <summary>
        /// Gets the errors resulting from the password update failure.
        /// </summary>
        /// <value>
        /// The errors resulting from the password update failure.
        /// </value>
		public List<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordResetException"/> class.
        /// </summary>
        /// <param name="errors">The errors resulting from the password update failure.</param>
        public InvalidPasswordResetException(List<string> errors)
		    : base("There was an error resetting the user's password.")
		{
			Errors = errors;
		}
	}
}
