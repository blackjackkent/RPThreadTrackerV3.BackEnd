// <copyright file="BaseRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Interfaces.Data;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class BaseRepository<T> : IRepository<T>
	    where T : class, IEntity
	{
        /// <summary>
        /// The database context.
        /// </summary>
        protected readonly TrackerContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public BaseRepository(TrackerContext context)
		{
			_context = context;
		}

	    /// <inheritdoc />
	    public T Create(T entity)
		{
			_context.Add(entity);
			_context.SaveChanges();
			return entity;
		}

	    /// <inheritdoc />
	    public bool Delete(T entity)
		{
			_context.Remove(entity);
			var rowsAffected = _context.SaveChanges();
			return rowsAffected > 0;
		}

	    /// <inheritdoc />
	    public bool ExistsWhere(Expression<Func<T, bool>> filter)
		{
			var query = _context.Set<T>().AsQueryable();
			return query.Where(filter).Any();
		}

	    /// <inheritdoc />
	    public IEnumerable<T> GetAll(List<string> navigationProperties = null)
		{
			var query = _context.Set<T>().AsQueryable();
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
			var query = _context.Set<T>().AsQueryable();
			if (navigationProperties == null)
			{
				return query.Where(filter).ToList();
			}

			query = navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
			return query.Where(filter).ToList();
		}

	    /// <inheritdoc />
	    public virtual T Update(string id, T entity)
		{
			_context.Update(entity);
			_context.SaveChanges();
			return entity;
		}
	}
}
