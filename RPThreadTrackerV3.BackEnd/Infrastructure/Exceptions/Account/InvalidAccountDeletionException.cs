// <copyright file="InvalidAccountDeletionException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions.Account
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The exception that is thrown when there was an error deleting a user's account information.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidAccountDeletionException : Exception
    {
        /// <summary>
        /// Gets or sets the errors resulting from the account deletion failure.
        /// </summary>
        /// <value>
        /// The errors resulting from the account deletion failure.
        /// </value>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountDeletionException"/> class.
        /// </summary>
        /// <param name="errors">The errors resulting from the account deletion failure.</param>
        public InvalidAccountDeletionException(List<string> errors)
            : base("There was an error deleting the users's account information.")
        {
            Errors = errors;
        }
    }
}
