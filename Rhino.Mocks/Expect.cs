using System;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	/*
	 * class: Expect
	 * Allows to set expectation on methods that has return values.
	 * 
	 * note:
	 * For methods with void return value, you need to use <LastCall>
	 */
	/// <summary>
	/// Allows to set expectation on methods that has return values.
	/// For methods with void return value, you need to use LastCall
	/// </summary>
	public
#if dotNet2
 static
#else
    sealed
#endif
        class Expect
	{
		/*
		 * Method: Call
		 * Get the method options for the last method call, which usually will be a method call
		 * or property that is located inside the <Call>. 
		 * See Expected Usage.
		 * 
		 * Expected usage:
		 * (start code)
		 * Expect.Call(mockObject.SomeCall()).Return(new Something());
		 * Expect.Call(mockList.Count).Return(50);
		 * (end)
		 * 
		 * Thread safety:
		 * This method is *not* safe for multi threading scenarios!
		 * If you need to record in a multi threading environment, use the <On> method, which _can_
		 * handle multi threading scenarios.
		 */ 
		/// <summary>
		/// The method options for the last call on /any/ proxy on /any/ repository on the current thread.
		/// This method if not safe for multi threading scenarios, use <see cref="On"/>.
		/// </summary>
		public static IMethodOptions Call(object ignored)
		{
			return LastCall.Options;
		}

		/*
		 * Method: On
		 * Get the method options for the last method call on the mockInstance.
		 * Unless you're recording in multiply threads, you are probably better off
		 * using <Call>
		 * 
		 * Expected usage:
		 * Expect.On(mockList).Call(mockList.Count).Return(50);
		 * 
		 * Thread safety:
		 * This method can be used in mutli threading scenarios.
		 */
		/// <summary>
		/// Get the method options for the last method call on the mockInstance.
		/// </summary>
		public static ICreateMethodExpectation On(object mockedInstace)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mockedInstace);
			return new CreateMethodExpectation(mockedObject, mockedInstace);
		}
	}
}