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


using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	
	public class StringConstraintsTests
	{
		[Fact]
		public void StartsWith()
		{
			Assert.True(Text.StartsWith("Hello").Eval("Hello World"));
			Assert.False(Text.StartsWith("Hello").Eval("World"));
			Assert.Equal("starts with \"Hello\"", Text.StartsWith("Hello").Message);
		}

		[Fact]
		public void EndsWith()
		{
			Assert.True(Text.EndsWith("World").Eval("Hello World"));
			Assert.False(Text.EndsWith("Hello").Eval("World"));
			Assert.Equal("ends with \"Hello\"", Text.EndsWith("Hello").Message);
		}

		[Fact]
		public void Contains()
		{
			AbstractConstraint c = Text.Contains("Ayende");
			Assert.True(c.Eval("Ayende Rahien"));
			Assert.False(c.Eval("Hello World"));
			Assert.Equal("contains \"Ayende\"", c.Message);
		}

		[Fact]
		public void Like()
		{
			AbstractConstraint c = Text.Like("[Rr]ahien");
			Assert.True(c.Eval("Ayende Rahien"));
			Assert.False(c.Eval("Hello World"));
			Assert.Equal("like \"[Rr]ahien\"", c.Message);
		}

	}
}