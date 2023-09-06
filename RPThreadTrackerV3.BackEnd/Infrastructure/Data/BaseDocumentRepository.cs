// <copyright file="BaseDocumentRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Exceptions;
    using Interfaces.Data;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    /// <inheritdoc />
    public class BaseDocumentRepository<T> : IDocumentRepository<T>
        where T : IDocument
    {
        private readonly IDocumentClient<T> _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDocumentRepository{T}"/> class.
        /// </summary>
        /// <param name="client">Wrapper for document database client.</param>
        public BaseDocumentRepository(IDocumentClient<T> client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var document = await _client.CreateDocumentQuery<T>(doc => doc.id == id);
                return document.FirstOrDefault();
            } catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                throw new DocumentDatabaseException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var results = await _client.CreateDocumentQuery(predicate);
            return results;
        }

        /// <inheritdoc />
        public async Task<T> CreateItemAsync(T item)
        {
            var result = await _client.CreateDocumentAsync(item);
            return result;
        }

        /// <inheritdoc />
        public async Task<T> UpdateItemAsync(string id, T item)
        {
            var result = await _client.ReplaceDocumentAsync(id, item);
            return result;
        }

        /// <inheritdoc />
        public async Task DeleteItemAsync(string id)
        {
            await _client.DeleteDocumentAsync(id);
        }
    }
}
