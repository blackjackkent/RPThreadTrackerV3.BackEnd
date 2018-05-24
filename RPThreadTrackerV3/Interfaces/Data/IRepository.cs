// <copyright file="IRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Interfaces.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

    /// <summary>
    /// Generic repository for retrieving entities from an entity store.
    /// </summary>
    /// <typeparam name="T">The type to which retrieved entities should be cast.</typeparam>
    public interface IRepository<T>
	    where T : IEntity
	{
        /// <summary>
        /// Determines whether an entity or entity of type <c>T</c> exists matching the passed predicate.
        /// </summary>
        /// <param name="filter">The predicate against which entities in the store should be compared.</param>
        /// <returns><c>true</c> if an entity or entities exists matching <c>filter</c>, otherwise <c>false</c>.</returns>
        bool ExistsWhere(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Gets all entities of type <c>T</c> in the data store.
        /// </summary>
        /// <param name="navigationProperties">List of strings corresponding to navigation properties
        /// which should be included in the returned dataset.</param>
        /// <returns>List of entities cast type type <c>T</c>.</returns>
        IEnumerable<T> GetAll(List<string> navigationProperties = null);

        /// <summary>
        /// Gets all entities of type <c>T</c> matching the passed predicate.
        /// </summary>
        /// <param name="filter">The predicate against which entities in the store should be compared.</param>
        /// <param name="navigationProperties">List of strings corresponding to navigation properties
        /// which should be included in the returned dataset.</param>
        /// <returns>List of entities cast type type <c>T</c>.</returns>
        IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter, List<string> navigationProperties = null);

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be created.</param>
        /// <returns>The created entity, cast to type <c>T</c>.</returns>
        T Create(T entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="id">The unique ID of the entity to be updated.</param>
        /// <param name="entity">The information with which the entity should be updated.</param>
        /// <returns>The updated entity, cast to type <c>T</c>.</returns>
        T Update(string id, T entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns><c>true</c> if the deletion was successful, otherwise <c>false</c>.</returns>
        bool Delete(T entity);
	}
}
