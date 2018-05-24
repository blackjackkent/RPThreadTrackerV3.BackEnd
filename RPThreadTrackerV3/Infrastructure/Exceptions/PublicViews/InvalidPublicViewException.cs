// <copyright file="InvalidPublicViewException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error validating a public view object.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidPublicViewException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPublicViewException"/> class.
        /// </summary>
        public InvalidPublicViewException()
            : base("The supplied public view configuration is invalid.")
        {
        }
    }
}
