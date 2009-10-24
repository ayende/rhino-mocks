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
using Rhino.Mocks.Impl;
using System.Collections;

namespace Rhino.Mocks.Tests.Impl
{
	
	public class ValidateTests
	{
		[Fact]
		public void IsNotNullWhenNotNull()
		{
			Validate.IsNotNull(new object(), "test");
		}

		[Fact]
		public void IsNotNullWhenNullThrows()
		{
			Assert.Throws<ArgumentNullException>(
				"Value cannot be null.\r\nParameter name: test",
				() => Validate.IsNotNull(null, "test"));
		}

		[Fact]
		public void ArgsEqualWhenNoArgs()
		{
			Assert.True(Validate.ArgsEqual(new object[0], new object[0]));
		}

		[Fact]
		public void HandlingArraysWithNull()
		{
			Assert.False(Validate.ArgsEqual(new object[] {1, null}, new object[] {1, "43"}));
			Assert.False(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, null}));
			Assert.True(Validate.ArgsEqual(new object[] {null, "43"}, new object[] {null, "43"}));
	
		}

		[Fact]
		public void ArgsEqualWithDifferentNumberOfParameters()
		{
			Assert.False(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, "43"}));
		}

		[Fact]
		public void ArgsEqualWhenArgsMatch()
		{
			Assert.True(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, "43", 5.2f}));
		}

		[Fact]
		public void ArgsEqualWhenArgsMismatch()
		{
			Assert.False(Validate.ArgsEqual(new object[] {1, "43", 5.1f}, new object[] {1, "43", 6.4f}));
		}

		[Fact]
		public void ArgsEqualWithArrayReferenceEqual()
		{
			object[] arr = new object[3] {"1", 2, 4.5f};
			Assert.True(Validate.ArgsEqual(new object[] {1, arr}, new object[] {1, arr}));
		}

		[Fact]
		public void ArgsEqualWithArrayContentEqual()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 2, 4.5f};
			Assert.True(Validate.ArgsEqual(new object[] {1, arr2}, new object[] {1, arr1}));
		}

		[Fact]
		public void ArgsEqualWithArrayContentDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 5, 4.5f};
			Assert.False(Validate.ArgsEqual(new object[] {1, arr1}, new object[] {1, arr2}));

		}

		[Fact]
		public void ArgsEqualWithArrayContentLengthDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[2] {"1", 5};
			Assert.False(Validate.ArgsEqual(new object[] {1, arr1}, new object[] {1, arr2}));
		}

		[Fact]
		public void ArgsEqualWithStringArray()
		{
			string[] str1 = new string[] {"", "1", "1234"},
				str2 = new string[] {"1", "1234", "54321"};
			Assert.False(Validate.ArgsEqual(str1, str2));
		}

		[Fact]
		public void ArgsEqualWithCollectionReferenceEqual()
		{
			Queue queue = new Queue(3);
			queue.Enqueue("1");
			queue.Enqueue(2);
			queue.Enqueue(4.5f);
			Assert.True(Validate.ArgsEqual(new object[] { 1, queue }, new object[] { 1, queue }));
		}

		[Fact]
		public void ArgsEqualWithCollectionContentEqual()
		{
			Queue queue1 = new Queue(3);
			queue1.Enqueue("1");
			queue1.Enqueue(2);
			queue1.Enqueue(4.5f);
			Queue queue2 = new Queue(3);
			queue2.Enqueue("1");
			queue2.Enqueue(2);
			queue2.Enqueue(4.5f);
			Assert.True(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}

		[Fact]
		public void ArgsEqualWithCollectionContentDifferent()
		{
			Queue queue1 = new Queue(3);
			queue1.Enqueue("1");
			queue1.Enqueue(2);
			queue1.Enqueue(4.5f);
			Queue queue2 = new Queue(3);
			queue2.Enqueue("1");
			queue2.Enqueue(5);
			queue2.Enqueue(4.5f);
			Assert.False(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}

		[Fact]
		public void ArgsEqualWithCollectionContentLengthDifferent()
		{
			Queue queue1 = new Queue(3);
			queue1.Enqueue("1");
			queue1.Enqueue(2);
			queue1.Enqueue(4.5f);
			Queue queue2 = new Queue(2);
			queue2.Enqueue("1");
			queue2.Enqueue(5);
			Assert.False(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}
	}
}
