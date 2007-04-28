using System;
using MbUnit.Framework;
using Rhino.Mocks.Impl;
using System.Collections;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class ValidateTests
	{
		[Test]
		public void IsNotNullWhenNotNull()
		{
			Validate.IsNotNull(new object(), "test");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: test")]
		public void IsNotNullWhenNullThrows()
		{
			Validate.IsNotNull(null, "test");
		}

		[Test]
		public void ArgsEqualWhenNoArgs()
		{
			Assert.IsTrue(Validate.ArgsEqual(new object[0], new object[0]));
		}

		[Test]
		public void HandlingArraysWithNull()
		{
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, null}, new object[] {1, "43"}));
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, null}));
			Assert.IsTrue(Validate.ArgsEqual(new object[] {null, "43"}, new object[] {null, "43"}));
	
		}

		[Test]
		public void ArgsEqualWithDifferentNumberOfParameters()
		{
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, "43"}));
		}

		[Test]
		public void ArgsEqualWhenArgsMatch()
		{
			Assert.IsTrue(Validate.ArgsEqual(new object[] {1, "43", 5.2f}, new object[] {1, "43", 5.2f}));
		}

		[Test]
		public void ArgsEqualWhenArgsMismatch()
		{
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, "43", 5.1f}, new object[] {1, "43", 6.4f}));
		}

		[Test]
		public void ArgsEqualWithArrayReferenceEqual()
		{
			object[] arr = new object[3] {"1", 2, 4.5f};
			Assert.IsTrue(Validate.ArgsEqual(new object[] {1, arr}, new object[] {1, arr}));
		}

		[Test]
		public void ArgsEqualWithArrayContentEqual()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 2, 4.5f};
			Assert.IsTrue(Validate.ArgsEqual(new object[] {1, arr2}, new object[] {1, arr1}));
		}

		[Test]
		public void ArgsEqualWithArrayContentDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 5, 4.5f};
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, arr1}, new object[] {1, arr2}));

		}

		[Test]
		public void ArgsEqualWithArrayContentLengthDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[2] {"1", 5};
			Assert.IsFalse(Validate.ArgsEqual(new object[] {1, arr1}, new object[] {1, arr2}));
		}

		[Test]
		public void ArgsEqualWithStringArray()
		{
			string[] str1 = new string[] {"", "1", "1234"},
				str2 = new string[] {"1", "1234", "54321"};
			Assert.IsFalse(Validate.ArgsEqual(str1, str2));
		}

		[Test]
		public void ArgsEqualWithCollectionReferenceEqual()
		{
			Queue queue = new Queue(3);
			queue.Enqueue("1");
			queue.Enqueue(2);
			queue.Enqueue(4.5f);
			Assert.IsTrue(Validate.ArgsEqual(new object[] { 1, queue }, new object[] { 1, queue }));
		}

		[Test]
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
			Assert.IsTrue(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}

		[Test]
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
			Assert.IsFalse(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}

		[Test]
		public void ArgsEqualWithCollectionContentLengthDifferent()
		{
			Queue queue1 = new Queue(3);
			queue1.Enqueue("1");
			queue1.Enqueue(2);
			queue1.Enqueue(4.5f);
			Queue queue2 = new Queue(2);
			queue2.Enqueue("1");
			queue2.Enqueue(5);
			Assert.IsFalse(Validate.ArgsEqual(new object[] { 1, queue1 }, new object[] { 1, queue2 }));
		}
	}
}
