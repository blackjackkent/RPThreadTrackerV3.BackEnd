// <copyright file="InvalidThreadException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Thread
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error validating a thread object.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidThreadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidThreadException"/> class.
        /// </summary>
        public InvalidThreadException()
            : base("The supplied thread was invalid.")
        {
        }
    }
}
