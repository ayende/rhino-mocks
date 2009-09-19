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
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks
{
    /// <summary>
    /// Allows easier access to MockRepository, works closely with Mocker.Current to
    /// allow access to a context where the mock repository is automatially verified at
    /// the end of the code block.
    /// </summary>
    public static class With
    {
        /// <summary>
        /// A method with no arguments and no return value that will be called under the mock context.
        /// </summary>
        public delegate void Proc();
        
        /// <summary>
        /// Initialize a code block where Mocker.Current is initialized.
        /// At the end of the code block, all the expectation will be verified.
        /// This overload will create a new MockRepository.
        /// </summary>
        /// <param name="methodCallThatHasMocks">The code that will be executed under the mock context</param>
        public static void Mocks(Proc methodCallThatHasMocks)
        {
            MockRepository mocks = new MockRepository();
            Mocks(mocks, methodCallThatHasMocks);
        }

        /// <summary>
        /// Initialize a code block where Mocker.Current is initialized.
        /// At the end of the code block, all the expectation will be verified.
        /// This overload will create a new MockRepository.
        /// </summary>
        /// <param name="mocks">The mock repository to use, at the end of the code block, VerifyAll() will be called on the repository.</param>
        /// <param name="methodCallThatHasMocks">The code that will be executed under the mock context</param>
        public static void Mocks(MockRepository mocks, Proc methodCallThatHasMocks)
        {
            Mocker.Current = mocks;
            try
            {
                methodCallThatHasMocks();
                mocks.VerifyAll();
            }
            finally
            {
                Mocker.Current = null;
            }
        }

        /// <summary>
        /// Create a FluentMocker
        /// </summary>
        /// <param name="mocks">The mock repository to use.</param>
        public static FluentMocker Mocks(MockRepository mocks)
        {
            return new FluentMocker(mocks);
        }

        /// <summary>
        /// FluentMocker implements some kind of fluent interface attempt
        /// for saying "With the Mocks [mocks], Expecting (in same order) [things] verify [that]."
        /// </summary>
        public class FluentMocker: IMockVerifier
        {
            private MockRepository _mocks;

            internal FluentMocker(MockRepository mocks)
            {
                _mocks = mocks;
            }

            /// <summary>
            /// Defines unordered expectations
            /// </summary>
            /// <param name="methodCallsDescribingExpectations">A delegate describing the expectations</param>
            /// <returns>an IMockVerifier</returns>
            public IMockVerifier Expecting(Proc methodCallsDescribingExpectations)
            {
                methodCallsDescribingExpectations();
                _mocks.ReplayAll();
                return this;
            }
            /// <summary>
            /// Defines ordered expectations
            /// </summary>
            /// <param name="methodCallsDescribingExpectations">A delegate describing the expectations</param>
            /// <returns>an IMockVerifier</returns>
            public IMockVerifier ExpectingInSameOrder(Proc methodCallsDescribingExpectations)
            {
                using (_mocks.Ordered())
                {
                    methodCallsDescribingExpectations();
                }
                _mocks.ReplayAll();
                return this;
            }

            /// <summary>
            /// Verifies previously defined expectations
            /// </summary>
            public void Verify(Proc methodCallsToBeVerified)
            {
                methodCallsToBeVerified();
                _mocks.VerifyAll();
            }
        }

        /// <summary>
        /// Interface to verify previously defined expectations
        /// </summary>
        public interface IMockVerifier
        {
            /// <summary>
            /// Verifies if a piece of code
            /// </summary>
            void Verify(Proc methodCallsToBeVerified);
        }
    }
}
