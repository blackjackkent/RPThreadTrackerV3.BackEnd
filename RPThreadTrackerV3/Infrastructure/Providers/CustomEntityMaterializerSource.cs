﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPThreadTrackerV3.Infrastructure.Providers
{
	using System.Linq.Expressions;
	using System.Reflection;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Internal;

	public class CustomEntityMaterializerSource : EntityMaterializerSource
	{
	    private static readonly MethodInfo NormalizeMethod = typeof(DateTimeMapper).GetTypeInfo().GetMethod(nameof(DateTimeMapper.Normalize));

	    public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, IProperty property = null)
	    {
		    if (type == typeof(DateTime) || type == typeof(DateTime?))
		    {
			    return Expression.Call(
				    NormalizeMethod,
				    base.CreateReadValueExpression(valueBuffer, type, index, property)
			    );
		    }

		    return base.CreateReadValueExpression(valueBuffer, type, index, property);
	    }
	}

	public static class DateTimeMapper
	{
		public static DateTime? Normalize(DateTime? value)
		{
			if (value == null)
			{
				return null;
			}
			return DateTime.SpecifyKind(value.GetValueOrDefault(), DateTimeKind.Utc);
		}
	}
}
