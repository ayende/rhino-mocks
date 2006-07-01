using System;

namespace Rhino.Mocks.Tests.Remoting
{

	[Serializable]
	public class RemotableDemoClass
	{
		int _prop;

		public virtual int Prop
		{
			get { return _prop; }
			set { _prop = value; }
		}

		public int One()
		{
			return 1;
		}

		public virtual int Two()
		{
			return 2;
		}
	}

}
