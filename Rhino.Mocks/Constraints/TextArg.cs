using System;
using System.Text;

namespace Rhino.Mocks.Constraints
{
	/// <summary>
	/// Provides access to the constraintes defined in the class <see cref="Text"/> to be used in context
	/// with the <see cref="Arg"/> syntax.
	/// </summary>
	public class TextArg
	{
		internal TextArg() { ;}

		/// <summary>
		/// Constrain the argument to starts with the specified string
		/// </summary>
		/// <returns></returns>
		public string StartsWith(string start)
		{
			ArgManager.AddInArgument(Text.StartsWith(start));
			return null;
		}

		/// <summary>
		/// Constrain the argument to end with the specified string
		/// </summary>
		public string EndsWith(string end)
		{
			ArgManager.AddInArgument(Text.EndsWith(end));
			return null;
		}

		/// <summary>
		/// Constrain the argument to contain the specified string
		/// </summary>
		public string Contains(string innerString)
		{
			ArgManager.AddInArgument(Text.Contains(innerString));
			return null;
		}

		/// <summary>
		/// Constrain the argument to validate according to regex pattern
		/// </summary>
		public string Like(string pattern)
		{
			ArgManager.AddInArgument(Text.Like(pattern));
			return null;
		}

		/// <summary>
		/// Throws NotSupportedException. Don't use Equals to define constraints. Use Equal instead.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("Don't use Equals() to define constraints, use Equal() instead");
		}

		/* implement GetHashCode to avoid compiler warning */
		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
