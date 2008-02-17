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
	public class MockWithRefAndOutParams
	{
		MockRepository mocks;
		IRefAndOut target;
		private RemotingProxyWithOutRef remotingTarget;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			target = (IRefAndOut)mocks.CreateMock(typeof(IRefAndOut));
			remotingTarget = mocks.CreateMock<RemotingProxyWithOutRef>();
		}

		[Test]
		public void RefString()
		{
			string s = "";
			target.RefStr(ref s);
			LastCall.Do(new RefStrDel(SayHello));
			mocks.ReplayAll();
			target.RefStr(ref s);
			Assert.AreEqual("Hello", s);
		}

		[Test]
		public void OutString()
		{
			string s = "";
			target.OutStr(out s);
			LastCall.Do(new OutStrDel(OutSayHello));
			mocks.ReplayAll();
			target.OutStr(out s);
			Assert.AreEqual("Hello", s);
		}

		[Test]
		public void OutInt()
		{
			int i = 0;
			target.OutInt(out i);
			LastCall.Do(new OutIntDel(OutFive));
			mocks.ReplayAll();
			target.OutInt(out i);
			Assert.AreEqual(5, i);
		}

		[Test]
		public void RefInt()
		{
			int i = 0;
			target.RefInt(ref i);
			LastCall.Do(new RefIntDel(RefFive));
			mocks.ReplayAll();
			target.RefInt(ref i);
			Assert.AreEqual(5, i);
		}


		[Test]
		public void RemotingRefString()
		{
			string s = "";
			remotingTarget.RefStr(ref s);
			LastCall.Do(new RefStrDel(SayHello));
			mocks.ReplayAll();
			remotingTarget.RefStr(ref s);
			Assert.AreEqual("Hello", s);
		}

		[Test]
		public void RemotingOutString()
		{
			string s = "";
			remotingTarget.OutStr(out s);
			LastCall.Do(new OutStrDel(OutSayHello));
			mocks.ReplayAll();
			remotingTarget.OutStr(out s);
			Assert.AreEqual("Hello", s);
		}

		[Test]
		public void RemotingOutInt()
		{
			int i = 0;
			remotingTarget.OutInt(out i);
			LastCall.Do(new OutIntDel(OutFive));
			mocks.ReplayAll();
			remotingTarget.OutInt(out i);
			Assert.AreEqual(5, i);
		}

		[Test]
		public void RemotingRefInt()
		{
			int i = 0;
			remotingTarget.RefInt(ref i);
			LastCall.Do(new RefIntDel(RefFive));
			mocks.ReplayAll();
			remotingTarget.RefInt(ref i);
			Assert.AreEqual(5, i);
		}

		private void RefFive(ref int i)
		{
			i = 5;
		}

		private void SayHello(ref string s)
		{
			s = "Hello";
		}

		private void OutFive(out int i)
		{
			i = 5;
		}

		private void OutSayHello(out string s)
		{
			s = "Hello";
		}

		public delegate void RefStrDel(ref string s);
		public delegate void RefIntDel(ref int i);
		public delegate void OutStrDel(out string s);
		public delegate void OutIntDel(out int i);

	}

	public interface IRefAndOut
	{
		void RefInt(ref int i);
		void RefStr(ref string s);

		void OutStr(out string s);
		void OutInt(out int i);
	}

	public class RemotingProxyWithOutRef : MarshalByRefObject
	{
		public void RefInt(ref int i)
		{
			i = 2;
		}

		public void RefStr(ref string s)
		{
			s = "b";
		}

		public void OutStr(out string s)
		{
			s = "a";
		}
		public void OutInt(out int i)
		{
			i = 1;
		}
	}
}
