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
using System.Data;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Tests.Callbacks;
using System.Collections.Generic;

namespace Rhino.Mocks.Tests
{
	// Interface to create mocks for
	public interface ITestInterface
	{
		event EventHandler<EventArgs> AnEvent;
		void RefOut(string str, out int i, string str2, ref int j, string str3);
		void VoidList(List<string> list);
		void VoidObject (object obj);
	}

	[TestFixture]
	public class ArgConstraintTests
	{
		private IDemo demoMock;
		ITestInterface testMock;
		private MockRepository mocks;
		private delegate string StringDelegateWithParams(int a, string b);
		
		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demoMock = this.mocks.StrictMock<IDemo>();
			testMock = mocks.StrictMock<ITestInterface>();
		}

		[Test]
		public void ThreeArgs_Pass()
		{
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Anything,
				Arg.Text.Contains("eine"),
				Arg<float>.Is.LessThan(2.5f));
			mocks.ReplayAll();
			demoMock.VoidThreeArgs(3, "Steinegger", 2.4f);
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException))]
		public void ThreeArgs_Fail()
		{
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Anything,
				Arg.Text.Contains("eine"),
				Arg<float>.Is.LessThan(2.5f));
			mocks.ReplayAll();
			demoMock.VoidThreeArgs(2, "Steinegger", 2.6f);
			mocks.VerifyAll();
		}

		[Test]
		public void Matches()
		{
			Expect.Call(delegate{
				demoMock.VoidStringArg(Arg<string>.Matches(Is.Equal("hallo") || Text.EndsWith("b")));})
				.Repeat.Times(3);
			mocks.ReplayAll();
			demoMock.VoidStringArg("hallo");
			demoMock.VoidStringArg("ab");
			demoMock.VoidStringArg("bb");
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidStringArg(contains \"World\"); Expected #1, Actual #0.")]
		public void ConstraintsThatWerentCallCauseVerifyFailure()
		{
			this.demoMock.VoidStringArg(Arg.Text.Contains("World"));
			this.mocks.Replay(this.demoMock);
			this.mocks.Verify(this.demoMock);
		}

		[Test]
		public void RefAndOutArgs()
		{
			testMock.RefOut(
				Arg<string>.Is.Anything,
				out Arg<int>.Out(3).Dummy,
				Arg<string>.Is.Equal("Steinegger"),
				ref Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				Arg<string>.Is.NotNull
			);
			mocks.ReplayAll();

			int iout = 0;
			int iref = 2;
			testMock.RefOut("hallo", out iout, "Steinegger", ref iref, "notnull");
			Assert.AreEqual(3, iout, "Mock did not return specified out value (Arg<T>.Out)");
			Assert.AreEqual(7, iref, "Mock did not return specified ref value (Arg<T>.Ref)");

			mocks.VerifyAll();
		}

		[Test]
		public void Event()
		{
			ITestInterface eventMock = mocks.StrictMock<ITestInterface>();
			eventMock.AnEvent += Arg<EventHandler<EventArgs>>.Is.Anything;
			mocks.ReplayAll();
			eventMock.AnEvent += handler;
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "ITestInterface.VoidList(System.Collections.Generic.List`1[System.String]); Expected #0, Actual #1.")]
		public void ListTest()
		{
			ITestInterface testMock = mocks.StrictMock<ITestInterface>();
			testMock.VoidList(Arg<List<string>>.List.Count(Is.GreaterThan(3)));
			testMock.VoidList(Arg<List<string>>.List.IsIn("hello"));

			mocks.ReplayAll();

			testMock.VoidList(new List<string>(new string[] { "1", "2", "4", "5" }));
			testMock.VoidList(new List<string>(new string[] { "1", "3" }));

			mocks.VerifyAll();
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException), "When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. 3 arguments expected, 2 have been defined.")]
		public void ConstraintWithTooFewArguments_ThrowsException()
		{
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Equal(4),
				Arg.Text.Contains("World"),
				3.14f);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Use Arg<T> ONLY within a mock method call while recording. 3 arguments expected, 4 have been defined.")]
		public void ConstraintToManyArgs_ThrowsException()
		{
			Arg<int>.Is.Equal(4);
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Equal(4),
				Arg.Text.Contains("World"),
				Arg<float>.Is.Equal(3.14f));
		}

		[Test]
		public void MockRepositoryClearsArgData()
		{
			Arg<int>.Is.Equal(4);
			Arg<int>.Is.Equal(4);

			// create new Mockrepositor yto see if the Arg data has been cleared
			mocks = new MockRepository();
			demoMock = this.mocks.StrictMock<IDemo>();
			
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Equal(4),
				Arg.Text.Contains("World"),
				Arg<float>.Is.Equal(3.14f));
		}
		

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. 5 arguments expected, 4 have been defined.")]
		public void TooFewOutArgs()
		{
			int iout = 2;
			testMock.RefOut(
				Arg<string>.Is.Anything,
				out iout,
				Arg.Text.Contains("Steinegger"),
				ref Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				Arg<string>.Is.NotNull
			);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Argument 1 must be defined as: out Arg<T>.Out(returnvalue).Dummy")]
		public void RefInsteadOfOutArg()
		{
			testMock.RefOut(
				Arg<string>.Is.Anything,
				out Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				Arg.Text.Contains("Steinegger"),
				ref Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				Arg<string>.Is.NotNull
			);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Argument 3 must be defined as: ref Arg<T>.Ref(constraint, returnvalue).Dummy")]
		public void OutInsteadOfRefArg()
		{
			testMock.RefOut(
				Arg<string>.Is.Anything,
				out Arg<int>.Out(7).Dummy,
				Arg.Text.Contains("Steinegger"),
				ref Arg<int>.Out(7).Dummy,
				Arg<string>.Is.NotNull
			);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Argument 0 must be defined using: Arg<T>.Is, Arg<T>.Text or Arg<T>.List")]
		public void OutInsteadOfInArg()
		{
			testMock.VoidObject(Arg<object>.Out(null));
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Don't use Equals() to define constraints, use Equal() instead")]
		public void Is_EqualsThrowsException()
		{
			Arg<object>.Is.Equals(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Don't use Equals() to define constraints, use Equal() instead")]
		public void List_EqualsThrowsException()
		{
			Arg<object>.List.Equals(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Don't use Equals() to define constraints, use Equal() instead")]
		public void Text_EqualsThrowsException()
		{
			Arg.Text.Equals(null);
		}
		
		/// <summary>
		/// Adapted from MockingDelegatesTests.MockStringDelegateWithParams
		/// </summary>
		[Test]
		public void MockStringDelegateWithParams()
		{
			StringDelegateWithParams d = (StringDelegateWithParams)mocks.StrictMock(typeof(StringDelegateWithParams));

			Expect.On(d).Call(
				d(
					Arg<int>.Is.Equal(1),
					Arg<string>.Is.Equal("111")))
				.Return("abc");
			Expect.On(d).Call(
				d(
					Arg<int>.Is.Equal(2),
					Arg<string>.Is.Equal("222")))
				.Return("def");

			mocks.Replay(d);

			Assert.AreEqual("abc", d(1, "111"));
			Assert.AreEqual("def", d(2, "222"));

			try
			{
				d(3, "333");
				Assert.Fail("Expected an expectation violation to occur.");
			}
			catch (ExpectationViolationException)
			{
				// Expected.
			}
		}
		
		private void handler(object o, EventArgs e)
		{
			
		}
		
	}
}
