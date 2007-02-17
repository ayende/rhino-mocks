using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Thierry
	{
		[Test]
		public void ReproducePbWithOutArraysContainingMockedObject2()
		{
			MockRepository mocks = new MockRepository();
			IPlugin plugin = mocks.CreateMock<IPlugin>();
			IPlugin[] allPlugins;

			// PluginMng
			IPluginMng pluginMng = (IPluginMng)mocks.CreateMock(typeof(IPluginMng));
			pluginMng.GetPlugins(out allPlugins);

			LastCall.IgnoreArguments().OutRef(
				new object[]{ new IPlugin[]{plugin} }
				);

			mocks.ReplayAll();

			pluginMng.GetPlugins(out allPlugins);

			Assert.AreEqual(1, allPlugins.Length);
			Assert.AreSame(plugin, allPlugins[0]);
		}
	}

	public interface IPluginMng
	{
		void GetPlugins(out IPlugin[] plugins);
	}

	public interface IPlugin
	{
		
	}
}