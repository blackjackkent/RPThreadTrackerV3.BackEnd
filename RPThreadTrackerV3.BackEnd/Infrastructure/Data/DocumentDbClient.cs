// <copyright file="DocumentDbClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Interfaces.Data;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.Extensions.Options;
    using Models.Configuration;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class DocumentDbClient<T> : IDocumentClient<T>
        where T : IDocument
    {
        private readonly CosmosClient _client;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly string _partitionKey;
        private Container _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbClient"/> class.
        /// </summary>
        /// <param name="options">The application settings.</param>
        public DocumentDbClient(IOptions<AppSettings> options)
        {
            var config = options.Value;
            var key = config.Secure.Documents.Key;
            var endpoint = config.Secure.Documents.Endpoint;
            _databaseId = config.Secure.Documents.DatabaseId;
            _collectionId = config.Secure.Documents.CollectionId;
            _partitionKey = config.Secure.Documents.PartitionKey;
            _client = new CosmosClient(endpoint, key);
        }
        /// <inheritdoc />
        public async Task<IEnumerable<T>> CreateDocumentQuery<T>(Expression<Func<T, bool>> predicate)
            where T : IDocument
        {
            var container = await GetContainer();
            using FeedIterator<T> setIterator = container.GetItemLinqQueryable<T>()
                .Where(predicate)
                .ToFeedIterator<T>();
            List<T> results = new List<T>();
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                {
                    {
                        results.Add(item);
                    }
                }
            }
            return results;
        }

        /// <inheritdoc />
        public async Task<T> CreateDocumentAsync(object item)
        {
            var container = await GetContainer();
            var result = await container.CreateItemAsync<T>((T)item);
            return result.Resource;
        }

        /// <inheritdoc />
        public async Task<T> ReplaceDocumentAsync(string id, object item)
        {
            var container = await GetContainer();
            var result = await container.ReplaceItemAsync<T>((T)item, id);
            return result.Resource;
        }

        /// <inheritdoc />
        public async Task DeleteDocumentAsync(string id)
        {
            var container = await GetContainer();
            await container.DeleteItemAsync<T>(id, PartitionKey.None);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        private async Task<Container> GetContainer()
        {
            if (_container != null)
            {
                return _container;
            }
            Database database = await _client.CreateDatabaseIfNotExistsAsync(_databaseId);
            _container = await database.CreateContainerIfNotExistsAsync(_collectionId, _partitionKey, 400);
            return _container;
        }
    }
}
