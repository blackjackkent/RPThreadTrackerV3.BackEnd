// <copyright file="InvalidRegistrationException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The exception that is thrown when there was an error registering a new user.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidRegistrationException : Exception
    {
        /// <summary>
        /// Gets or sets the errors resulting from the account registration failure.
        /// </summary>
        /// <value>
        /// The errors resulting from the account registration failure.
        /// </value>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRegistrationException"/> class.
        /// </summary>
        /// <param name="errors">The errors resulting from the account registration failure.</param>
        public InvalidRegistrationException(List<string> errors)
            : base("The supplied registration information is invalid.")
        {
            Errors = errors;
        }
    }
}
