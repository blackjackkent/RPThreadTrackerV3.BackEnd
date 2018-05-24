// <copyright file="InvalidAccountInfoUpdateException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The exception that is thrown when there was an error updating a user's account information.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidAccountInfoUpdateException : Exception
    {
        /// <summary>
        /// Gets the errors resulting from the account update failure.
        /// </summary>
        /// <value>
        /// The errors resulting from the account update failure.
        /// </value>
        public List<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountInfoUpdateException"/> class.
        /// </summary>
        /// <param name="errors">The errors resulting from the account update failure.</param>
        public InvalidAccountInfoUpdateException(List<string> errors)
            : base("There was an error updating the users's account information.")
        {
		    Errors = errors;
	    }
	}
}
