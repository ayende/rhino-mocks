using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	/// <summary>
	/// Setup method calls to repeat any number of times.
	/// </summary>
	public
#if dotNet2
 static
#else
    sealed
#endif
        class SetupResult
	{
		/*
		 * Method: For
		 * Sets the last method call to repeat any number of times and return the method options 
		 * for the last method call, which usually will be a method call
		 * or property that is located inside the <For>. 
		 * See Expected Usage.
		 * 
		 * Expected usage:
		 * (start code)
		 * SetupResult.For(mockObject.SomeCall()).Return(new Something());
		 * SetupResult.For(mockList.Count).Return(50);
		 * (end)
		 * 
		 * Thread safety:
		 * This method is *not* safe for multi threading scenarios!
		 * If you need to record in a multi threading environment, use the <On> method, which _can_
		 * handle multi threading scenarios.
		 * 
		 */ 
		/// <summary>
		/// Get the method options and set the last method call to repeat 
		/// any number of times.
		/// This also means that the method would transcend ordering
		/// </summary>
		public static IMethodOptions For(object ignored)
		{
			return LastCall.Options.Repeat.Any();
		}

		/*
		 * Method: On
		 * Sets the last method call to repeat any number of times and return the method options 
		 * for the last method call on the mockInstance.
		 * Unless you're recording in multiply threads, you are probably better off
		 * using <For>
		 * 
		 * Expected usage:
		 * SetupResult.On(mockList).Call(mockList.Count).Return(50);
		 * 
		 * Thread safety:
		 * This method can be used in mutli threading scenarios.
		 */
		/// <summary>
		/// Get the method options for the last method call on the mockInstance and set it
		/// to repeat any number of times.
		/// This also means that the method would transcend ordering
		/// </summary>
		public static ICreateMethodExpectation On(object mockedInstace)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mockedInstace);
			return new CreateMethodExpectationForSetupResult(mockedObject, mockedInstace);
		}
	}
}