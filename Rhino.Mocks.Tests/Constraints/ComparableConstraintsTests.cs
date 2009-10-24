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
using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	
	public class ComparableConstraintsTests
	{
		[Fact]
		public void GreaterThan()
		{
			Assert.True(Is.GreaterThan(1).Eval(3));
			Assert.False(Is.GreaterThan(1.5).Eval(1.0));
			Assert.Equal("greater than 5", Is.GreaterThan(5).Message);
		}

		[Fact]
		public void LessThan()
		{
			Assert.False(Is.LessThan(1).Eval(3));
			Assert.True(Is.LessThan(1.5).Eval(1.0));
			Assert.Equal("less than 5", Is.LessThan(5).Message);
		}

		[Fact]
		public void LessThanOrEqual()
		{
			Assert.False(Is.LessThanOrEqual(1).Eval(3));
			Assert.True(Is.LessThanOrEqual(4.5f).Eval(4.5f));
			Assert.True(Is.LessThanOrEqual(1.5).Eval(1.0));
			Assert.Equal("less than or equal to 5", Is.LessThanOrEqual(5).Message);
		}

		[Fact]
		public void GreaterThanOrEqual()
		{
			Assert.True(Is.GreaterThanOrEqual(1).Eval(3));
			Assert.True(Is.GreaterThanOrEqual(3L).Eval(3L));
			Assert.False(Is.GreaterThanOrEqual(1.5).Eval(1.0));
			Assert.Equal("greater than or equal to 5", Is.GreaterThanOrEqual(5).Message);
		}

		[Fact]
		public void Equal()
		{
			Assert.False(Is.Equal(1).Eval(3));
			Assert.True(Is.Equal(3L).Eval(3L));
			Assert.False(Is.Equal("Hi").Eval("Bye"));
			Assert.True(Is.Equal("Bye").Eval("Bye"));
			Assert.Equal("equal to Rahien", Is.Equal("Rahien").Message);
		}

		[Fact]
		public void NotEqual()
		{
			Assert.True(Is.NotEqual(1).Eval(3));
			Assert.False(Is.NotEqual(3L).Eval(3L));
			Assert.True(Is.NotEqual("Hi").Eval("Bye"));
			Assert.False(Is.NotEqual("Bye").Eval("Bye"));
			Assert.Equal("not equal to Rahien", Is.NotEqual("Rahien").Message);
		}

		[Fact]
		public void IsNull()
		{
			Assert.True(Is.Null().Eval(null));
			Assert.False(Is.Null().Eval(""));
			Assert.Equal("equal to null", Is.Null().Message);
		}

		[Fact]
		public void IsNotNull()
		{
			Assert.False(Is.NotNull().Eval(null));
			Assert.True(Is.NotNull().Eval(""));
			Assert.Equal("not equal to null", Is.NotNull().Message);
		}
	    
	     [Fact]
        public void Same()
        {
            object o1 = new object();
            object o2 = new object();

            Assert.True(Is.Same(o1).Eval(o1));
            Assert.False(Is.Same(o1).Eval(o2));

            // Now assert that two different objects that are .Equal to each other are the same
            o1 = 5;
            o2 = 5;
            Assert.False(Object.ReferenceEquals(o1, o2), "Internal test failure: o1 and o2 should be different objects");
            Assert.True(Is.Same(o1).Eval(o1));
            Assert.False(Is.Same(o1).Eval(o2));

            Assert.Equal("same as FooBar", Is.Same("FooBar").Message);
        }

        [Fact]
        public void NotSame()
        {
            object o1 = new object();
            object o2 = new object();

            Assert.True(Is.NotSame(o1).Eval(o2));
            Assert.False(Is.NotSame(o1).Eval(o1));

            // Now assert that two different objects that are .Equal to each other are not the same
            o1 = 5;
            o2 = 5;
            Assert.False(Object.ReferenceEquals(o1, o2), "Internal test failure: o1 and o2 should be different objects");
            Assert.True(Is.NotSame(o1).Eval(o2));
            Assert.False(Is.NotSame(o1).Eval(o1));
        }
	}

}
