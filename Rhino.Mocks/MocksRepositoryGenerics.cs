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


#if dotNet2
using System;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks
{
    /*
     * Class: MockRepository Generic Methods
     */
    /// <summary>
	/// Creates proxied instances of types.
	/// </summary>
	public partial class MockRepository
    {
        /*
          * Method: CreateMock<T>
          * Create a mock object of type T with strict semantics.
          * Strict semantics means that any call that wasn't explicitly recorded is considered an
          * error and would cause an exception to be thrown. 
          */
        /// <summary>
        /// Creates a mock for the spesified type.
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T CreateMock<T>(params object[] argumentsForConstructor)
        {
            CreateMockState factory = new CreateMockState(CreateRecordState);
            return (T)CreateMockObject(typeof(T), factory, new Type[0], argumentsForConstructor);
        }

        /*
         * Method: DynamicMock<T>
         * Create a mock object of type T with dynamic semantics.
         * Dynamic semantics means that any call that wasn't explicitly recorded is accepted and a
         * null or zero is returned (if there is a return value).
         */
        /// <summary>
        /// Creates a dynamic mock for the specified type.
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T DynamicMock<T>(params object[] argumentsForConstructor)
        {
            CreateMockState factory = new CreateMockState(CreateDynamicRecordState);
            return (T)CreateMockObject(typeof(T), factory, new Type[0], argumentsForConstructor);
        }
        
        /// <summary>
        /// Creates a mock object from several types.
        /// </summary>
        public T CreateMultiMock<T>(params Type[] extraTypes)
        {
            return (T)CreateMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with dynamic semantics.
        /// </summary>
        public T DynamicMultiMock<T>(params Type[] extraTypes)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with partial semantics.
        /// </summary>
        public T PartialMultiMock<T>(params Type[] extraTypes)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with strict semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T CreateMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)CreateMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Create a mock object from several types with dynamic semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Create a mock object from several types with partial semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /*
		 * Method: PartialMock
		 * Create a mock object with from a class that defaults to calling the class methods
         * if no expectation is set on the method.
         * 
		 */
        /// <summary>
        /// Create a mock object with from a class that defaults to calling the class methods
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T PartialMock<T>( params object[] argumentsForConstructor) where T : class
        {
        	return (T) PartialMock(typeof (T), argumentsForConstructor);
        }
    }
}
#endif
