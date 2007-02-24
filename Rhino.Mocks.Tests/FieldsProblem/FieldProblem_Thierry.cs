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