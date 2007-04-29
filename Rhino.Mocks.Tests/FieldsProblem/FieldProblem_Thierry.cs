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


using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Thierry
	{
		[Test]
		public void ReproducedWithOutArraysContainingMockedObject2()
		{
			MockRepository mocks = new MockRepository();
			IPlugin plugin = mocks.CreateMock<IPlugin>();
			IPlugin[] allPlugins;

			// PluginMng
			IPluginMng pluginMng = (IPluginMng) mocks.CreateMock(typeof (IPluginMng));
			pluginMng.GetPlugins(out allPlugins);

			LastCall.IgnoreArguments().OutRef(
				new object[] {new IPlugin[] {plugin}}
				);

			mocks.ReplayAll();

			pluginMng.GetPlugins(out allPlugins);

			Assert.AreEqual(1, allPlugins.Length);
			Assert.AreSame(plugin, allPlugins[0]);
		}

		[Test]
		public void MockGenericMethod1()
		{
			MockRepository mocks = new MockRepository();
			IWithGeneric1 stubbed = mocks.CreateMock<IWithGeneric1>();

			byte myValue = 3;
			int returnedValue = 3;

			Expect.Call(stubbed.DoNothing<byte>(myValue)).Return(returnedValue);
			mocks.ReplayAll();
			int x = stubbed.DoNothing<byte>(myValue);
			Assert.AreEqual(myValue, x);

			mocks.VerifyAll();
		}

		[Test]
		public void MockGenericMethod2()
		{
			MockRepository mocks = new MockRepository();
			IWithGeneric2 stubbed = mocks.CreateMock<IWithGeneric2>();

			byte myValue = 4;
			Expect.Call(stubbed.DoNothing<byte>(myValue)).Return(myValue);
			mocks.ReplayAll();
			byte x = stubbed.DoNothing<byte>(myValue);
			Assert.AreEqual(myValue, x);

			mocks.VerifyAll();
		}

		[Test]
		public void CanMockComplexReturnType()
		{
			MockRepository mocks = new MockRepository();
			IWithGeneric2 stubbed = mocks.CreateMock<IWithGeneric2>();

			byte myValue = 4;
			List<byte> bytes = new List<byte>();
			bytes.Add(myValue);
			Expect.Call(stubbed.DoNothing<IList<byte>>(null)).Return(bytes);
			mocks.ReplayAll();
			IList<byte> bytesResult = stubbed.DoNothing<IList<byte>>(null);
			Assert.AreEqual(bytes, bytesResult);

			mocks.VerifyAll();
		}
	}

	public interface IWithGeneric2
	{
		T DoNothing<T>(T x);
	}

	public interface IWithGeneric1
	{
		int DoNothing<T>(T x);
	}


	public interface IPluginMng
	{
		void GetPlugins(out IPlugin[] plugins);
	}

	public interface IPlugin
	{
	}
}