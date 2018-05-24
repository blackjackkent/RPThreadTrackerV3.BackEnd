// <copyright file="PublicViewNotFoundException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Exceptions.PublicViews
{
    using System;

    /// <summary>
    /// The exception that is thrown when there was an error retrieving a public view.
    /// </summary>
    /// <seealso cref="Exception" />
    public class PublicViewNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicViewNotFoundException"/> class.
        /// </summary>
        public PublicViewNotFoundException()
            : base("The requested public view configuration does not exist for the current user.")
        {
        }
    }
}
