namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Range for expected method calls
	/// </summary>
	public class Range
	{
		private int min, max;

		/// <summary>
		/// Creates a new <see cref="Range"/> instance.
		/// </summary>
		/// <param name="min">Min.</param>
		/// <param name="max">Max.</param>
		public Range(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Gets or sets the min.
		/// </summary>
		/// <value></value>
		public int Min
		{
			get { return min; }
		}

		/// <summary>
		/// Gets or sets the max.
		/// </summary>
		/// <value></value>
		public int Max
		{
			get { return max; }
		}

		/// <summary>
		/// Return the string representation of this range.
		/// </summary>
		public override string ToString()
		{
			if (min == 0)
				return max.ToString();
			if (min != max)
				return min + ".." + max;
			else
				return min.ToString();
		}


	}
}