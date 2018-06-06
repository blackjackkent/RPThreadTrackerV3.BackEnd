// <copyright file="InvalidCharacterException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions.Characters
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error validating a character object.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidCharacterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        public InvalidCharacterException()
            : base("The supplied character is invalid.")
        {
        }
    }
}
