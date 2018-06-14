// <copyright file="DocumentDbClient.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces.Data;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Extensions.Options;
    using Models.Configuration;
    using Newtonsoft.Json;

    /// <inheritdoc />
    public class DocumentDbClient<T> : IDocumentClient<T>
        where T : Resource, IDocument
    {
        private readonly DocumentClient _client;
        private readonly string _databaseId;
        private readonly string _collectionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbClient{T}"/> class.
        /// </summary>
        /// <param name="options">The application settings.</param>
        public DocumentDbClient(IOptions<AppSettings> options)
        {
            var config = options.Value;
            var key = config.Secure.Documents.Key;
            var endpoint = config.Secure.Documents.Endpoint;
            _databaseId = config.Secure.Documents.DatabaseId;
            _collectionId = config.Secure.Documents.CollectionId;
            _client = new DocumentClient(new Uri(endpoint), key);
        }

        /// <inheritdoc />
        public async Task<T> ReadDocumentAsync(string id)
        {
            var document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
            return JsonConvert.DeserializeObject<T>(document.Resource.ToString());
        }

        /// <inheritdoc />
        public IOrderedQueryable<T> CreateDocumentQuery()
        {
            return _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                new FeedOptions { MaxItemCount = -1 });
        }

        /// <inheritdoc />
        public async Task<T> CreateDocumentAsync(T item)
        {
            var document = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
            return JsonConvert.DeserializeObject<T>(document.Resource.ToString());
        }

        /// <inheritdoc />
        public async Task<T> ReplaceDocumentAsync(string id, T item)
        {
            var document = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
            return JsonConvert.DeserializeObject<T>(document.Resource.ToString());
        }

        /// <inheritdoc />
        public async Task DeleteDocumentAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }

        /// <inheritdoc />
        public async Task AssertDatabaseExists()
        {
            await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
        }

        /// <inheritdoc />
        public async Task CreateDatabaseAsync()
        {
            await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
        }

        /// <inheritdoc />
        public async Task AssertCollectionExists()
        {
            await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
        }

        /// <inheritdoc />
        public async Task CreateDocumentCollectionAsync()
        {
            await _client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(_databaseId),
                new DocumentCollection { Id = _collectionId },
                new RequestOptions { OfferThroughput = 1000 });
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
    }
}
