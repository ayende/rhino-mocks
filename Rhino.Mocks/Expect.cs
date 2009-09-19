#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
	public static class Expect
	{
		///<summary>
		///A delegate that can be used to get better syntax on Expect.Call(delegate { foo.DoSomething(); });
		///</summary>
		public delegate void Action();
	
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
		public static IMethodOptions<T> Call<T>(T ignored)
		{
			return LastCall.GetOptions<T>();
		}

		/// <summary>
		/// Accepts a delegate that will execute inside the method, and then return the resulting
		/// <see cref="IMethodOptions{T}"/> instance.
		/// It is expected to be used with anonymous delegates / lambda expressions and only one
		/// method should be called.
		/// </summary>
		/// <example>
		/// IService mockSrv = mocks.CreateMock(typeof(IService)) as IService;
		/// Expect.Call(delegate{ mockSrv.Start(); }).Throw(new NetworkException());
		/// ...
		/// </example>
		public static IMethodOptions<Action> Call(Action actionToExecute)
		{
			if (actionToExecute == null)
				throw new ArgumentNullException("actionToExecute", "The action to execute cannot be null");
			actionToExecute();
			return LastCall.GetOptions<Action>();
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