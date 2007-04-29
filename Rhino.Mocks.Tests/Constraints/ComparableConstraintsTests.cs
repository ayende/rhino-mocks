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
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	[TestFixture]
	public class ComparableConstraintsTests
	{
		[Test]
		public void GreaterThan()
		{
			Assert.IsTrue(Is.GreaterThan(1).Eval(3));
			Assert.IsFalse(Is.GreaterThan(1.5).Eval(1.0));
			Assert.AreEqual("greater than 5", Is.GreaterThan(5).Message);
		}

		[Test]
		public void LessThan()
		{
			Assert.IsFalse(Is.LessThan(1).Eval(3));
			Assert.IsTrue(Is.LessThan(1.5).Eval(1.0));
			Assert.AreEqual("less than 5", Is.LessThan(5).Message);
		}

		[Test]
		public void LessThanOrEqual()
		{
			Assert.IsFalse(Is.LessThanOrEqual(1).Eval(3));
			Assert.IsTrue(Is.LessThanOrEqual(4.5f).Eval(4.5f));
			Assert.IsTrue(Is.LessThanOrEqual(1.5).Eval(1.0));
			Assert.AreEqual("less than or equal to 5", Is.LessThanOrEqual(5).Message);
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			Assert.IsTrue(Is.GreaterThanOrEqual(1).Eval(3));
			Assert.IsTrue(Is.GreaterThanOrEqual(3L).Eval(3L));
			Assert.IsFalse(Is.GreaterThanOrEqual(1.5).Eval(1.0));
			Assert.AreEqual("greater than or equal to 5", Is.GreaterThanOrEqual(5).Message);
		}

		[Test]
		public void Equal()
		{
			Assert.IsFalse(Is.Equal(1).Eval(3));
			Assert.IsTrue(Is.Equal(3L).Eval(3L));
			Assert.IsFalse(Is.Equal("Hi").Eval("Bye"));
			Assert.IsTrue(Is.Equal("Bye").Eval("Bye"));
			Assert.AreEqual("equal to Rahien", Is.Equal("Rahien").Message);
		}

		[Test]
		public void NotEqual()
		{
			Assert.IsTrue(Is.NotEqual(1).Eval(3));
			Assert.IsFalse(Is.NotEqual(3L).Eval(3L));
			Assert.IsTrue(Is.NotEqual("Hi").Eval("Bye"));
			Assert.IsFalse(Is.NotEqual("Bye").Eval("Bye"));
			Assert.AreEqual("not equal to Rahien", Is.NotEqual("Rahien").Message);
		}

		[Test]
		public void IsNull()
		{
			Assert.IsTrue(Is.Null().Eval(null));
			Assert.IsFalse(Is.Null().Eval(""));
			Assert.AreEqual("equal to null", Is.Null().Message);
		}

		[Test]
		public void IsNotNull()
		{
			Assert.IsFalse(Is.NotNull().Eval(null));
			Assert.IsTrue(Is.NotNull().Eval(""));
			Assert.AreEqual("not equal to null", Is.NotNull().Message);
		}
	    
	     [Test]
        public void Same()
        {
            object o1 = new object();
            object o2 = new object();

            Assert.IsTrue(Is.Same(o1).Eval(o1));
            Assert.IsFalse(Is.Same(o1).Eval(o2));

            // Now assert that two different objects that are .Equal to each other are the same
            o1 = 5;
            o2 = 5;
            Assert.IsFalse(Object.ReferenceEquals(o1, o2), "Internal test failure: o1 and o2 should be different objects");
            Assert.IsTrue(Is.Same(o1).Eval(o1));
            Assert.IsFalse(Is.Same(o1).Eval(o2));

            Assert.AreEqual("same as FooBar", Is.Same("FooBar").Message);
        }

        [Test]
        public void NotSame()
        {
            object o1 = new object();
            object o2 = new object();

            Assert.IsTrue(Is.NotSame(o1).Eval(o2));
            Assert.IsFalse(Is.NotSame(o1).Eval(o1));

            // Now assert that two different objects that are .Equal to each other are not the same
            o1 = 5;
            o2 = 5;
            Assert.IsFalse(Object.ReferenceEquals(o1, o2), "Internal test failure: o1 and o2 should be different objects");
            Assert.IsTrue(Is.NotSame(o1).Eval(o2));
            Assert.IsFalse(Is.NotSame(o1).Eval(o1));
        }
	}

}
