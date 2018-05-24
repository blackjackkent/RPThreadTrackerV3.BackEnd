// <copyright file="ThreadNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.Thread
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error retrieving a thread.
    /// </summary>
    /// <seealso cref="Exception" />
    public class ThreadNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadNotFoundException"/> class.
        /// </summary>
        public ThreadNotFoundException()
            : base("The requested thread does not exist for the current user.")
        {
        }
    }
}
