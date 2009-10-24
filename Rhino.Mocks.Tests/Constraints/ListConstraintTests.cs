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
using System.Collections.Generic;
using System.Collections;

namespace Rhino.Mocks.Tests.Constraints
{
	
	public class ListConstraintTests
	{
		[Fact]
		public void InIs()
		{
			AbstractConstraint list = List.IsIn('a');
			Assert.True(list.Eval("ayende"));
			Assert.False(list.Eval("sheep"));
			Assert.Equal("list contains [a]", list.Message);
		}

		[Fact]
		public void OneOf()
		{
			AbstractConstraint list = List.OneOf(new string[] {"Ayende", "Rahien", "Hello", "World"});
			Assert.True(list.Eval("Ayende"));
            Assert.False(list.Eval("sheep"));
			Assert.Equal("one of [Ayende, Rahien, Hello, World]", list.Message);
		}
        
		[Fact]
		public void Equal()
		{
			AbstractConstraint list = List.Equal(new string[] {"Ayende", "Rahien", "Hello", "World"});
			Assert.True(list.Eval(new string[] {"Ayende", "Rahien", "Hello", "World"}));
			Assert.False(list.Eval(new string[] {"Ayende", "Rahien", "World", "Hello"}));
			Assert.False(list.Eval(new string[] {"Ayende", "Rahien", "World"}));
			Assert.False(list.Eval(5));
			Assert.Equal("equal to collection [Ayende, Rahien, Hello, World]", list.Message);

		}

        [Fact]
        public void Count()
        {
            AbstractConstraint list = List.Count(Is.Equal(4));
            Assert.True(list.Eval(new string[] { "Ayende", "Rahien", "Hello", "World" }));
            Assert.False(list.Eval(new string[] { "Ayende", "Rahien", "World" }));
            Assert.False(list.Eval(5));
            Assert.Equal("collection count equal to 4", list.Message);
        }

        [Fact]
        public void Element()
        {
            AbstractConstraint list = List.Element(2, Is.Equal("Hello"));
            Assert.True(list.Eval(new string[] { "Ayende", "Rahien", "Hello", "World" }));
            Assert.False(list.Eval(new string[] { "Ayende", "Rahien", "World", "Hello" }));
            Assert.False(list.Eval(new string[] { "Ayende", "Rahien" }));
            Assert.False(list.Eval(5));
            Assert.Equal("element at index 2 equal to Hello", list.Message);
        }
#if DOTNET35
        [Fact]
        public void StringKeyedElement()
        {
            AbstractConstraint list = List.Element<string>("Color", Is.Equal("Red"));
            Assert.True(list.Eval(new Dictionary<string, string>() { { "Name", "Ayende" }, { "Color", "Red" } }));
            Assert.False(list.Eval(new Dictionary<string, string>() { { "Name", "Ayende" }, { "Color", "Blue" } }));
            Assert.False(list.Eval(new Dictionary<string, string>() { { "Name", "Ayende" } }));
            Assert.Equal("element at key Color equal to Red", list.Message);
        }
#endif

        [Fact]
        public void ContainsAll()
        {
            AbstractConstraint list = List.ContainsAll(new string[] {"Ayende", "Rahien", "Hello", "World"});
            Assert.True(list.Eval(new string[] {"Ayende", "Rahien", "Hello", "World"}));
            Assert.False(list.Eval(new string[] { "Baaaah"}));
            Assert.False(list.Eval(5));
            list = List.ContainsAll(new string[] { "Ayende", "Rahien", "Hello", "World" });
            Assert.False(list.Eval(new string[] { "Ayende", "Rahien" }));
            Assert.Equal("list missing [Hello, World]", list.Message);
        }

        private class FailsOnEqual
        {
            public override bool Equals(object obj)
            {
                Assert.False(true, "Iteration over the collection was not expected.");
                return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [Fact]
        public void Equal_BothListsAreICollectionWithDifferentSizes_DoesNotIterateOverCollections()
        {
            AbstractConstraint list = List.Equal(new FailsOnEqual[] {new FailsOnEqual(),
             new FailsOnEqual()});
            Assert.False(list.Eval(new FailsOnEqual[]{new FailsOnEqual()}));
        }

        private IEnumerable NameList()
        {
            yield return "doron";
            yield return "hi";
            yield return "there";

        }

        [Fact]
        public void Equal_ConstraintIsNotICollection_StillWorks()
        {
            AbstractConstraint list = List.Equal(NameList());
            Assert.True(list.Eval(new string[] { "doron", "hi", "there" }));
            Assert.True(list.Eval(NameList()));
            
            Assert.False(list.Eval(new string[]{"doron","there", "hi"}));
            Assert.False(list.Eval(new string[] { "doron", "hi" }));
            Assert.False(list.Eval(6));

            Assert.Equal("equal to collection [doron, hi, there]", list.Message);
        }
	}
}
