// <copyright file="IDocumentClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Wrapper for document database client functionality.
    /// </summary>
    /// <typeparam name="T">The type to which retrieved documents should be cast.</typeparam>
    public interface IDocumentClient<T> : IDisposable
        where T : IDocument
    {
        /// <summary>
        /// Creates an entry for the passed document in the database.
        /// </summary>
        /// <param name="item">The item for which to create a document entry.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created document.
        /// </returns>
        Task<T> CreateDocumentAsync(object item);

        /// <summary>
        /// Creates a query statement for retrieving objects of type <c>T</c> from the database matching the passed predicate.
        /// </summary>
        /// <typeparam name="T">The type for which the query object should search.</typeparam>
        /// <param name="predicate">The predicate which queried objects should match.</param>
        /// <returns>A query statement for retrieving objects of type <c>T</c> from the database matching the predicate.</returns>
        Task<IEnumerable<T>> CreateDocumentQuery<T>(Expression<Func<T, bool>> predicate)
            where T : IDocument;

        /// <summary>
        /// Deletes the document with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the document to be deleted.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteDocumentAsync(string id);

        /// <summary>
        /// Updates the document with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the document to be updated.</param>
        /// <param name="item">The contents with which the document should be updated.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated document.
        /// </returns>
        Task<T> ReplaceDocumentAsync(string id, object item);
    }
}
