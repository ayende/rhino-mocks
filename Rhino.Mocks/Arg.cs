#if DOTNET35
using System;
using System.Linq.Expressions;

namespace Rhino.Mocks
{
	/// <summary>
	/// Allow you to specify constraints using lambda expressions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class Arg<T>
	{
		/// <summary>
		/// Register the predicate as a constraint for the current call.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>default(T)</returns>
		/// <example>
		/// Allow you to use code to create constraints
		/// <code>
		/// demo.AssertWasCalled(x => x.Bar(Arg{string}.Matches(a => a.StartsWith("b") &amp;&amp; a.Contains("ba"))));
		/// </code>
		/// </example>
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
