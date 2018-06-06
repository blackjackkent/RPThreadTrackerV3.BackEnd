// <copyright file="InvalidRefreshTokenException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions.Account
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error processing a user's refresh token.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidRefreshTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRefreshTokenException"/> class.
        /// </summary>
        public InvalidRefreshTokenException()
            : base("The supplied refresh token is invalid.")
        {
        }
    }
}
