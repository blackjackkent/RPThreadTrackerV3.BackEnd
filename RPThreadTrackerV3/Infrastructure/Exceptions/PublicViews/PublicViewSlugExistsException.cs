// <copyright file="PublicViewSlugExistsException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error creating or updating a public view
    /// because its slug is already used for another view.
    /// </summary>
    /// <seealso cref="Exception" />
    public class PublicViewSlugExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicViewSlugExistsException"/> class.
        /// </summary>
        public PublicViewSlugExistsException()
            : base("The supplied public view slug already exists.")
        {
        }
    }
}
