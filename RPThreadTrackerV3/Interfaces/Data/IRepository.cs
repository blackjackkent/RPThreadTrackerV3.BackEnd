namespace RPThreadTrackerV3.Interfaces.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public interface IRepository<T> where T : IEntity
	{
		IEnumerable<T> GetAll(List<string> navigationProperties = null);
		IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter, List<string> navigationProperties = null);
		T Create(T entity);
		T Update(string id, T entity);
		bool Delete(T entity);
	}
}
