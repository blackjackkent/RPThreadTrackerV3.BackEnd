// <copyright file="IRepository.cs" company="Rosalind Wills">
// Copyright (c) Rosalind Wills. All rights reserved.
// Licensed under the GPL v3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RPThreadTrackerV3.Interfaces.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	public interface IRepository<T>
	    where T : IEntity
	{
		bool ExistsWhere(Expression<Func<T, bool>> filter);
		IEnumerable<T> GetAll(List<string> navigationProperties = null);
		IEnumerable<T> GetWhere(Expression<Func<T, bool>> filter, List<string> navigationProperties = null);
		T Create(T entity);
		T Update(string id, T entity);
		bool Delete(T entity);
	}
}
