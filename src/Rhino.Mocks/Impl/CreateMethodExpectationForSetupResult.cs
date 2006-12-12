using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Allows to call a method and immediatly get it's options.
	/// Set the expected number for the call to Any() 
	/// </summary>
	public class CreateMethodExpectationForSetupResult : CreateMethodExpectation
	{
		/// <summary>
		/// Creates a new <see cref="CreateMethodExpectationForSetupResult"/> instance.
		/// </summary>
		/// <param name="mockedObject">Proxy.</param>
		/// <param name="mockedInstance">Mocked instance.</param>
		public CreateMethodExpectationForSetupResult(IMockedObject mockedObject, object mockedInstance) : base(mockedObject, mockedInstance)
		{
		}

		/// <summary>
		/// Get the method options for the call
		/// </summary>
		/// <param name="ignored">The method call should go here, the return value is ignored</param>
		public override IMethodOptions Call(object ignored)
		{
			return base.Call(ignored).Repeat.Any();
		}
	}
}