// <copyright file="DocumentDatabaseException.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Exceptions
{
    using System;

    /// <summary>
    /// The exception that is thrown when there is an error interacting with the document database.
    /// </summary>
    public class DocumentDatabaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDatabaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The inner exception.</param>
        public DocumentDatabaseException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}
