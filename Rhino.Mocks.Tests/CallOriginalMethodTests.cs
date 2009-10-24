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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
    using Xunit;

    
    public class CallOriginalMethodTests
    {

        [Fact]
        public void CallOriginalMethodOnPropGetAndSet()
        {
            MockRepository mocks = new MockRepository();
            MockingClassesTests.DemoClass demo = (MockingClassesTests.DemoClass)
                mocks.StrictMock(typeof(MockingClassesTests.DemoClass));

            SetupResult.For(demo.Prop).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            SetupResult.For(demo.Prop = 0).CallOriginalMethod(OriginalCallOptions.NoExpectation);

            mocks.ReplayAll();

            for (int i = 0; i < 10; i++)
            {
                demo.Prop = i;
                Assert.Equal(i, demo.Prop);
            }
            mocks.VerifyAll();
        }

        [Fact]
        public void CantCallOriginalMethodOnInterface()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
        	Assert.Throws<InvalidOperationException>(
        		"Can't use CallOriginalMethod on method ReturnIntNoArgs because the method is abstract.",
        		() => SetupResult.For(demo.ReturnIntNoArgs()).CallOriginalMethod(OriginalCallOptions.CreateExpectation));
        }

        [Fact]
        public void CantCallOriginalMethodOnAbstractMethod()
        {
            MockRepository mocks = new MockRepository();
            MockingClassesTests.AbstractDemo demo = (MockingClassesTests.AbstractDemo)mocks.StrictMock(typeof(MockingClassesTests.AbstractDemo));
        	Assert.Throws<InvalidOperationException>(
        		"Can't use CallOriginalMethod on method Six because the method is abstract.",
        		() => SetupResult.For(demo.Six()).CallOriginalMethod(OriginalCallOptions.CreateExpectation));
        }

    }
}
