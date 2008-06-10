#if DOTNET35
using System;
using System.Linq.Expressions;

namespace Rhino.Mocks
{
	public static class Arg<T>
	{
		public static T Matches(Expression<Predicate<T>> predicate)
		{
			if (RhinoMocksExtensions.argumentPredicates == null)
			{
				throw new InvalidOperationException("Cannot pass explicit delegate to setup the expectation and also use Arg<T>.Matches");
			}
			RhinoMocksExtensions.argumentPredicates.Add(predicate);

			return default(T);
		}
	}
}

#endif
