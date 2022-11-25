// <copyright file="BaseRepository.cs" company="Rosalind Wills">
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
    using Interfaces.Data;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class BaseRepository<T> : IRepository<T>
	    where T : class, IEntity
	{
	    /// <summary>
	    /// Gets the database context.
	    /// </summary>
	    protected TrackerContext Context { get; }

	    /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public BaseRepository(TrackerContext context)
		{
			Context = context;
		}

	    /// <inheritdoc />
	    public T Create(T entity)
		{
			Context.Add(entity);
			Context.SaveChanges();
			return entity;
		}

	    /// <inheritdoc />
	    public bool Delete(T entity)
		{
			Context.Remove(entity);
			var rowsAffected = Context.SaveChanges();
			return rowsAffected > 0;
		}

	    /// <inheritdoc />
	    public bool ExistsWhere(Expression<Func<T, bool>> filter)
		{
			var query = Context.Set<T>().AsQueryable();
			return query.Where(filter).Any();
		}

	    /// <inheritdoc />
	    public IEnumerable<T> GetAll(List<string> navigationProperties = null)
		{
			var query = Context.Set<T>().AsQueryable();
			if (navigationProperties == null)
			{
				return query.ToList();
			}
			query = navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
			return query.AsNoTracking().ToList();
		}

	    /// <inheritdoc />
	    public IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter, List<string> navigationProperties = null)
		{
			var query = Context.Set<T>().AsQueryable();
			if (navigationProperties == null)
			{
				var result = query.Where(filter);
				return result.ToList();
			}

			query = navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
			return query.Where(filter).ToList();
		}

	    /// <inheritdoc />
	    public virtual T Update(string id, T entity)
		{
			Context.Update(entity);
			Context.SaveChanges();
			return entity;
		}
	}
}
