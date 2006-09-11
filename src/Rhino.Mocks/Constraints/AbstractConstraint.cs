namespace Rhino.Mocks.Constraints
{
	/// <summary>
	/// Interface for constraints
	/// </summary>
	public abstract class AbstractConstraint
	{
		/// <summary>
		/// determains if the object pass the constraints
		/// </summary>
		public abstract bool Eval(object obj);

		/// <summary>
		/// Gets the message for this constraint
		/// </summary>
		/// <value></value>
		public abstract string Message { get; }

		/// <summary>
		/// And operator for constraints
		/// </summary>
		public static AbstractConstraint operator &(AbstractConstraint c1, AbstractConstraint c2)
		{
			return new And(c1, c2);
		}

		/// <summary>
		/// Not operator for constraints
		/// </summary>
		public static AbstractConstraint operator !(AbstractConstraint c1)
		{
			return new Not(c1);
		}


		/// <summary>
		/// Or operator for constraints
		/// </summary>
		public static AbstractConstraint operator |(AbstractConstraint c1, AbstractConstraint c2)
		{
			return new Or(c1, c2);
		}

		/// <summary>
		/// Allow overriding of || or &amp;&amp;
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static bool operator false(AbstractConstraint c)
		{
			return false;
		}
		/// <summary>
		/// Allow overriding of || or &amp;&amp;
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static bool operator true(AbstractConstraint c)
		{
			return false;
		}
	}
}