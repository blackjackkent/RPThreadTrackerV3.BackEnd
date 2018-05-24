// <copyright file="IDocumentRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Interfaces.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Generic repository for retrieving documents from a document store.
    /// </summary>
    /// <typeparam name="T">The type to which retrieved documents should be cast.</typeparam>
    public interface IDocumentRepository<T>
        where T : class, IDocument
    {
        /// <summary>
        /// Creates a new document representing an object of type <c>T</c>.
        /// </summary>
        /// <param name="item">The object to be represented in a document.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created document.
        /// </returns>
        Task<Document> CreateItemAsync(T item);

        /// <summary>
        /// Deletes the document with the given ID from the document store.
        /// </summary>
        /// <param name="id">The unique ID of the document to be deleted.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteItemAsync(string id);

        /// <summary>
        /// Gets the document with the given ID from the document store.
        /// </summary>
        /// <param name="id">The unique ID of the document to be fetched.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the fetched document cast to type <c>T</c>.
        /// </returns>
        Task<T> GetItemAsync(string id);

        /// <summary>
        /// Gets documents matching the given predicate from the document store.
        /// </summary>
        /// <param name="predicate">The predicate to which documents in the store should be compared.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the fetched documents cast to type <c>T</c>.
        /// </returns>
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Updates the document with the given ID with the information in <c>item</c>.
        /// </summary>
        /// <param name="id">The unique ID of the document to be updated.</param>
        /// <param name="item">Object of type <c>T</c> containing information that should be populated in the document.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated document.
        /// </returns>
        Task<Document> UpdateItemAsync(string id, T item);
    }
}
