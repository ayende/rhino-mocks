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
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class DoHanlderTests
    {
        MockRepository mocks;
        IDemo demo;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            demo = (IDemo)mocks.CreateMock(typeof(IDemo));
        }

        [Test]
        public void CanModifyReturnValue()
        {
            Expect.Call(demo.EnumNoArgs()).Do(new GetDay(GetSunday));
            mocks.ReplayAll();
            Assert.AreEqual(DayOfWeek.Sunday, demo.EnumNoArgs());
            mocks.VerifyAll();
        }

        [Test]
        public void SayHelloWorld()
        {
            INameSource nameSource = (INameSource)mocks.CreateMock(typeof(INameSource));
            Expect.Call(nameSource.CreateName(null,null)).IgnoreArguments().
                    Do(new NameSourceDelegate(Formal));
            mocks.ReplayAll();
            string expected = "Hi, my name is Ayende Rahien";
            string actual = new Speaker("Ayende", "Rahien", nameSource).Introduce();
            Assert.AreEqual(expected, actual);
        }

        delegate string NameSourceDelegate(string first, string suranme);
        
        private string Formal(string first, string surname)
        {
            return first + " " +surname;
        }
        
        public class Speaker
        {
            private readonly string firstName;
            private readonly string surname;

            private INameSource nameSource ;

            public Speaker(string firstName, string surname, INameSource nameSource)
            {
                this.firstName = firstName;
                this.surname = surname;
                this.nameSource = nameSource;
            }
            
            public string Introduce()
            {
                string name = nameSource.CreateName(firstName, surname);
                return string.Format("Hi, my name is {0}", name);
            }
        }

        public interface INameSource
        {
            string CreateName(string firstName, string surname);
        }
        
        [Test]
        public void CanThrow()
        {
            Expect.Call(demo.EnumNoArgs()).Do(new GetDay(ThrowDay));
            mocks.ReplayAll();
            try
            {
                demo.EnumNoArgs();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Not a day", e.Message);
            }
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "The delegate return value should be assignable from System.Int32")]
        public void InvalidReturnValueThrows()
        {
            Expect.Call(demo.ReturnIntNoArgs()).Do(new GetDay(GetSunday));
            
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Callback arguments didn't match the method arguments")]
        public void InvalidDelegateThrows()
        {
            Expect.Call(demo.ReturnIntNoArgs()).Do(new IntDelegate(IntMethod));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Can set only a single return value or exception to throw or delegate to throw on the same method call.")]
        public void CanOnlySpecifyOnce()
        {
            Expect.Call(demo.EnumNoArgs()).Do(new GetDay(ThrowDay)).Return(DayOfWeek.Saturday);
        }

        public delegate DayOfWeek GetDay();

        private DayOfWeek GetSunday()
        {
            return DayOfWeek.Sunday;
        }

        private DayOfWeek ThrowDay()
        {
            throw new ArgumentException("Not a day");
        }

        public delegate int IntDelegate(int i);

        private int IntMethod(int i)
        {
            return i;
        }
    }
}
