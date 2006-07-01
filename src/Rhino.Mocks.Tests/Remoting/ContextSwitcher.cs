using System;

namespace Rhino.Mocks.Tests.Remoting
{

	public class ContextSwitcher : MarshalByRefObject
	{
		public void DoStuff(Rhino.Mocks.Tests.IDemo mock)
		{
			int n = mock.ReturnIntNoArgs();
			mock.VoidStringArg(n.ToString());
		}



		public int DoStuff(RemotableDemoClass mock)
		{
			return mock.Two();
		}



		public void DoStuff(Other.IDemo remotingDemo)
		{
			remotingDemo.ProcessString("in");
		}
	}

}
