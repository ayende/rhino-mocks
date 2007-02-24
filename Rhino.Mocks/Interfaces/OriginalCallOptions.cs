namespace Rhino.Mocks.Interfaces
{
	/// <summary>
	/// Options for CallOriginalMethod
	/// </summary>
	public enum OriginalCallOptions
	{
		/// <summary>
		/// No expectation is created, the method will be called directly
		/// </summary>
		NoExpectation,
		/// <summary>
		/// Normal expectation is created, but when the method is later called, it will also call the original method
		/// </summary>
		CreateExpectation
	}
}