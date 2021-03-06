﻿// <copyright file="BaseDocumentRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.BackEnd.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Exceptions;
    using Interfaces.Data;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using IDocumentClient = Interfaces.Data.IDocumentClient;

    /// <inheritdoc />
    public class BaseDocumentRepository<T> : IDocumentRepository<T>
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

                throw new DocumentDatabaseException(e.Message, e);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _client.CreateDocumentQuery(predicate);

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
                    throw new DocumentDatabaseException(e.Message, e);
                }
            }
            catch (Exception e)
            {
                throw new DocumentDatabaseException(e.Message, e);
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
                    throw new DocumentDatabaseException(e.Message, e);
                }
            }
            catch (Exception e)
            {
                throw new DocumentDatabaseException(e.Message, e);
            }
        }
    }
}
