// <copyright file="IDocumentClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Interfaces.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Wrapper for document database client functionality.
    /// </summary>
    /// <typeparam name="T">The type of document which this client interacts with.</typeparam>
    public interface IDocumentClient<T> : IDisposable
        where T : Resource, IDocument
    {
        /// <summary>
        /// Throws an exception if the database collection does not exist.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AssertCollectionExists();

        /// <summary>
        /// Throws an exception if the database does not exist.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AssertDatabaseExists();

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task CreateDatabaseAsync();

        /// <summary>
        /// Creates an entry for the passed document in the database.
        /// </summary>
        /// <param name="item">The item for which to create a document entry.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created document.
        /// </returns>
        Task<T> CreateDocumentAsync(T item);

        /// <summary>
        /// Creates the document collection.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task CreateDocumentCollectionAsync();

        /// <summary>
        /// Creates a document query for a particular document type.
        /// </summary>
        /// <returns>A queryable object of type <c>T</c>.</returns>
        IOrderedQueryable<T> CreateDocumentQuery();

        /// <summary>
        /// Deletes the document with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the document to be deleted.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task DeleteDocumentAsync(string id);

        /// <summary>
        /// Retrieves information about the document with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the document to be retrieved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the retrieved document.
        /// </returns>
        Task<T> ReadDocumentAsync(string id);

        /// <summary>
        /// Updates the document with the passed ID.
        /// </summary>
        /// <param name="id">The ID of the document to be updated.</param>
        /// <param name="item">The contents with which the document should be updated.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated document.
        /// </returns>
        Task<T> ReplaceDocumentAsync(string id, T item);
    }
}
