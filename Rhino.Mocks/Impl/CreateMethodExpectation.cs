using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Allows to call a method and immediatly get it's options.
	/// </summary>
	public class CreateMethodExpectation : ICreateMethodExpectation
	{
		private IMockedObject mockedObject;
		private readonly object mockedInstance;

		/// <summary>
		/// Creates a new <see cref="CreateMethodExpectation"/> instance.
		/// </summary>
		public CreateMethodExpectation(IMockedObject mockedObject, object mockedInstance)
		{
			this.mockedObject = mockedObject;
			this.mockedInstance = mockedInstance;
		}

		/// <summary>
		/// Get the method options for the call
		/// </summary>
		/// <param name="ignored">The method call should go here, the return value is ignored</param>
		public virtual IMethodOptions Call(object ignored)
		{
			return mockedObject.Repository.LastMethodCall(mockedInstance);
		}
	}
}