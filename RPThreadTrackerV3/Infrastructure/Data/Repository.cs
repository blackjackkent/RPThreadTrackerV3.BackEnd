namespace RPThreadTrackerV3.Infrastructure.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Microsoft.EntityFrameworkCore;
	using RPThreadTrackerV3.Interfaces.Data;

	public class Repository<T> : IRepository<T> where T : class, IEntity
	{
		private readonly TrackerContext _context;

		public Repository(TrackerContext context)
		{
			_context = context;
		}
		public T Create(T entity)
		{
			_context.Add(entity);
			_context.SaveChanges();
			return entity;
		}

		public bool Delete(T entity)
		{
			_context.Remove(entity);
			var rowsAffected = _context.SaveChanges();
			return rowsAffected > 0;
		}

		public IEnumerable<T> GetAll(List<string> navigationProperties = null)
		{
			var query = _context.Set<T>().AsQueryable();
			if (navigationProperties == null)
			{
				return query.ToList();
			}
			query = navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
			return query.ToList();
		}

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

		public T Update(string id, T entity)
		{
			_context.Update(entity);
			_context.SaveChanges();
			return entity;
		}
	}
}
