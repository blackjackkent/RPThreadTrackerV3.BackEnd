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
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Linq;
    using Newtonsoft.Json;
    using IDocumentClient = Interfaces.Data.IDocumentClient;

    /// <inheritdoc cref="IDocumentRepository{T}" />
    public class BaseDocumentRepository<T> : IDocumentRepository<T>, IDisposable
        where T : Resource, IDocument
    {
        private readonly IDocumentClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDocumentRepository{T}"/> class.
        /// </summary>
        /// <param name="client">Wrapper for document database client.</param>
        public BaseDocumentRepository(IDocumentClient client)
        {
            _client = client;
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        /// <inheritdoc />
        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var document = await _client.ReadDocumentAsync(id);
                return JsonConvert.DeserializeObject<T>(document.ToString());
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _client.CreateDocumentQuery<T>()
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        /// <inheritdoc />
        public async Task<T> CreateItemAsync(T item)
        {
            var result = await _client.CreateDocumentAsync(item);
            return JsonConvert.DeserializeObject<T>(result.ToString());
        }

        /// <inheritdoc />
        public async Task<T> UpdateItemAsync(string id, T item)
        {
            var result = await _client.ReplaceDocumentAsync(id, item);
            return JsonConvert.DeserializeObject<T>(result.ToString());
        }

        /// <inheritdoc />
        public async Task DeleteItemAsync(string id)
        {
            await _client.DeleteDocumentAsync(id);
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

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.AssertDatabaseExists();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync();
                }
                else
                {
                    throw new DocumentDatabaseInitializationException(e.Message, e);
                }
            }
            catch (Exception e)
            {
                throw new DocumentDatabaseInitializationException(e.Message, e);
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.AssertCollectionExists();
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync();
                }
                else
                {
                    throw new DocumentDatabaseInitializationException(e.Message, e);
                }
            }
            catch (Exception e)
            {
                throw new DocumentDatabaseInitializationException(e.Message, e);
            }
        }
    }
}
