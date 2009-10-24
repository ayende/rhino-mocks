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
using Xunit;
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

	
	public class ArgConstraintTests
	{
		private IDemo demoMock;
		ITestInterface testMock;
		private MockRepository mocks;
		private delegate string StringDelegateWithParams(int a, string b);

		public ArgConstraintTests()
		{
			mocks = new MockRepository();
			demoMock = this.mocks.StrictMock<IDemo>();
			testMock = mocks.StrictMock<ITestInterface>();
		}

		[Fact]
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

		[Fact]
		public void ThreeArgs_Fail()
		{
			demoMock.VoidThreeArgs(
				Arg<int>.Is.Anything,
				Arg.Text.Contains("eine"),
				Arg<float>.Is.LessThan(2.5f));
			mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>(() => demoMock.VoidThreeArgs(2, "Steinegger", 2.6f));
		}

		[Fact]
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

		[Fact]
		public void ConstraintsThatWerentCallCauseVerifyFailure()
		{
			this.demoMock.VoidStringArg(Arg.Text.Contains("World"));
			this.mocks.Replay(this.demoMock);
			Assert.Throws<ExpectationViolationException>("IDemo.VoidStringArg(contains \"World\"); Expected #1, Actual #0.",
			                                             () => this.mocks.Verify(this.demoMock));
		}

		[Fact]
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
			Assert.Equal(3, iout);
			Assert.Equal(7, iref);

			mocks.VerifyAll();
		}

		[Fact]
		public void Event()
		{
			ITestInterface eventMock = mocks.StrictMock<ITestInterface>();
			eventMock.AnEvent += Arg<EventHandler<EventArgs>>.Is.Anything;
			mocks.ReplayAll();
			eventMock.AnEvent += handler;
			mocks.VerifyAll();
		}

		[Fact]
		public void ListTest()
		{
			ITestInterface testMock = mocks.StrictMock<ITestInterface>();
			testMock.VoidList(Arg<List<string>>.List.Count(Is.GreaterThan(3)));
			testMock.VoidList(Arg<List<string>>.List.IsIn("hello"));

			mocks.ReplayAll();

			testMock.VoidList(new List<string>(new string[] { "1", "2", "4", "5" }));
			
			Assert.Throws<ExpectationViolationException>(
				"ITestInterface.VoidList(System.Collections.Generic.List`1[System.String]); Expected #0, Actual #1.",
				() => testMock.VoidList(new List<string>(new string[] { "1", "3" })));
		}
		
		[Fact]
		public void ConstraintWithTooFewArguments_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(
				"When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. 3 arguments expected, 2 have been defined.",
				() => demoMock.VoidThreeArgs(
				      	Arg<int>.Is.Equal(4),
				      	Arg.Text.Contains("World"),
				      	3.14f));
		}

		[Fact]
		public void ConstraintToManyArgs_ThrowsException()
		{
			Arg<int>.Is.Equal(4);
			Assert.Throws<InvalidOperationException>(
				"Use Arg<T> ONLY within a mock method call while recording. 3 arguments expected, 4 have been defined.",
				() =>
				demoMock.VoidThreeArgs(
					Arg<int>.Is.Equal(4),
					Arg.Text.Contains("World"),
					Arg<float>.Is.Equal(3.14f)));
		}

		[Fact]
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
		

		[Fact]
		public void TooFewOutArgs()
		{
			int iout = 2;
			Assert.Throws<InvalidOperationException>(
				"When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. 5 arguments expected, 4 have been defined.",
				() => testMock.RefOut(
				      	Arg<string>.Is.Anything,
				      	out iout,
				      	Arg.Text.Contains("Steinegger"),
				      	ref Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				      	Arg<string>.Is.NotNull
				      	));
		}

		[Fact]
		public void RefInsteadOfOutArg()
		{
			Assert.Throws<InvalidOperationException>(
				"Argument 1 must be defined as: out Arg<T>.Out(returnvalue).Dummy",
				() => testMock.RefOut(
				      	Arg<string>.Is.Anything,
				      	out Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				      	Arg.Text.Contains("Steinegger"),
				      	ref Arg<int>.Ref(Is.Equal(2), 7).Dummy,
				      	Arg<string>.Is.NotNull
				      	));
		}

		[Fact]
		public void OutInsteadOfRefArg()
		{
			Assert.Throws<InvalidOperationException>(
				"Argument 3 must be defined as: ref Arg<T>.Ref(constraint, returnvalue).Dummy",
				() => testMock.RefOut(
				      	Arg<string>.Is.Anything,
				      	out Arg<int>.Out(7).Dummy,
				      	Arg.Text.Contains("Steinegger"),
				      	ref Arg<int>.Out(7).Dummy,
				      	Arg<string>.Is.NotNull
				      	));
		}

		[Fact]
		public void OutInsteadOfInArg()
		{
			Assert.Throws<InvalidOperationException>(
				"Argument 0 must be defined using: Arg<T>.Is, Arg<T>.Text or Arg<T>.List",
				() => testMock.VoidObject(Arg<object>.Out(null)));
		}
		
		[Fact]
		public void Is_EqualsThrowsException()
		{
			Assert.Throws<InvalidOperationException>(
				"Don't use Equals() to define constraints, use Equal() instead",
				() => Arg<object>.Is.Equals(null));
		}

		[Fact]
		public void List_EqualsThrowsException()
		{
			Assert.Throws<InvalidOperationException>(
				"Don't use Equals() to define constraints, use Equal() instead",
				() => Arg<object>.List.Equals(null));
		}

		[Fact]
		public void Text_EqualsThrowsException()
		{
			Assert.Throws<InvalidOperationException>(
				"Don't use Equals() to define constraints, use Equal() instead",
				() => Arg.Text.Equals(null));
		}
		
		/// <summary>
		/// Adapted from MockingDelegatesTests.MockStringDelegateWithParams
		/// </summary>
		[Fact]
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

			Assert.Equal("abc", d(1, "111"));
			Assert.Equal("def", d(2, "222"));

			try
			{
				d(3, "333");
				Assert.False(true, "Expected an expectation violation to occur.");
			}
			catch (ExpectationViolationException)
			{
				// Expected.
			}
		}
		
		private void handler(object o, EventArgs e)
		{
			
		}


        [Fact]
        public void Mock_object_using_ExpectMethod_with_ArgConstraints_allow_for_multiple_calls_as_default_behavior()
        {
            // Arrange
            var mock = MockRepository.GenerateMock<IDemo>();
            mock.Expect(x => x.StringArgString(Arg<string>.Is.Equal("input"))).Return("output");

            // Act
            var firstCallResult = mock.StringArgString("input");
            var secondCallResult = mock.StringArgString("input");

            // Assert
            Assert.Equal("output", firstCallResult);
            Assert.Equal(firstCallResult, secondCallResult);
        }

        [Fact]
        public void Stub_object_using_ExpectMethod_with_ArgConstraints_allow_for_multiple_calls_as_default_behavior()
        {
            // Arrange
            var mock = MockRepository.GenerateStub<IDemo>();
            mock.Expect(x => x.StringArgString(Arg<string>.Is.Equal("input"))).Return("output");

            // Act
            var firstCallResult = mock.StringArgString("input");
            var secondCallResult = mock.StringArgString("input");

            // Assert
            Assert.Equal("output", firstCallResult);
            Assert.Equal(firstCallResult, secondCallResult);
        }

        [Fact]
        public void Stub_object_using_StubMethod_with_ArgConstraints_allow_for_multiple_calls_as_default_behavior()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<IDemo>();
            stub.Stub(x => x.StringArgString(Arg<string>.Is.Equal("input"))).Return("output");

            // Act
            var firstCallResult = stub.StringArgString("input");
            var secondCallResult = stub.StringArgString("input");

            // Assert
            Assert.Equal("output", firstCallResult);
            Assert.Equal(firstCallResult, secondCallResult);
        }

        [Fact]
        public void Mock_object_using_StubMethod_with_ArgConstraints_allow_for_multiple_calls_as_default_behavior()
        {
            // Arrange
            var mock = MockRepository.GenerateMock<IDemo>();
            mock.Stub(x => x.StringArgString(Arg<string>.Is.Equal("input"))).Return("output");

            // Act
            var firstCallResult = mock.StringArgString("input");
            var secondCallResult = mock.StringArgString("input");

            // Assert
            Assert.Equal("output", firstCallResult);
            Assert.Equal(firstCallResult, secondCallResult);
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsEqual()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.Equal(1))).Return("test"); // 1 is inferred as Int32 (not Int64)

            // Assert
            Assert.Equal(null, stub.GetUser(0));
            Assert.Equal("test", stub.GetUser(1));
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsNotEqual()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.NotEqual(1))).Return("test"); // 1 is inferred as Int32 (not Int64)

            var actual = stub.GetUser(0);

            // Assert
            Assert.Equal("test", actual);
            Assert.Equal(null, stub.GetUser(1));
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsGreaterThan()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.GreaterThan(1))).Return("test"); // 1 is inferred as Int32 (not Int64)

            // Assert
            Assert.Equal(null, stub.GetUser(0));
            Assert.Equal(null, stub.GetUser(1));
            Assert.Equal("test", stub.GetUser(2));
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsGreaterThanOrEqual()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.GreaterThanOrEqual(2))).Return("test"); // 1 is inferred as Int32 (not Int64)

            // Assert
            Assert.Equal(null, stub.GetUser(1));
            Assert.Equal("test", stub.GetUser(2));
            Assert.Equal("test", stub.GetUser(3));
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsLessThan()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.LessThan(2))).Return("test"); // 1 is inferred as Int32 (not Int64)

            // Assert
            Assert.Equal("test", stub.GetUser(1));
            Assert.Equal(null, stub.GetUser(2));
            Assert.Equal(null, stub.GetUser(3));
        }

        [Fact]
        public void ImplicitlyConverted_parameter_is_properly_compared_when_using_IsLessThanOrEqual()
        {
            // Arrange
            var stub = MockRepository.GenerateStub<ITestService>();
            stub.Stub(x => x.GetUser(Arg<long>.Is.LessThanOrEqual(2))).Return("test"); // 1 is inferred as Int32 (not Int64)

            // Assert
            Assert.Equal("test", stub.GetUser(1));
            Assert.Equal("test", stub.GetUser(2));
            Assert.Equal(null, stub.GetUser(3));
        }

        public interface ITestService
        {
            string GetUser(long id);
        }
	}
}
