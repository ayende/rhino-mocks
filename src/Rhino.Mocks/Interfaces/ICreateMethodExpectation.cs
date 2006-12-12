namespace Rhino.Mocks.Interfaces
{
	/// <summary>
	/// Interface to allows to call a method and immediatly get it's options.
	/// </summary>
	public interface ICreateMethodExpectation
	{
		/// <summary>
		/// Get the method options for the call
		/// </summary>
		/// <param name="ignored">The method call should go here, the return value is ignored</param>
		IMethodOptions Call(object ignored);
	}
}