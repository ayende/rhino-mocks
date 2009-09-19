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


using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	/// <summary>
	/// Setup method calls to repeat any number of times.
	/// </summary>
	public static class SetupResult
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
		public static IMethodOptions<T> For<T>(T ignored)
		{
			return LastCall.GetOptions<T>().Repeat.Any();
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