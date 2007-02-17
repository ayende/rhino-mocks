using System;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests.Utilities
{
	[TestFixture]
	public class MethodCallTests
	{
		[Test]
		public void MethodCallToString()
		{
			string actual = MethodCallUtil.StringPresentation(null, GetMethodInfo("StartsWith", ""), new object[] {"abcd"});
			Assert.AreEqual("String.StartsWith(\"abcd\");", actual);
		}

		[Test]
		public void MethodCallToStringWithSeveralArguments()
		{
			string actual = MethodCallUtil.StringPresentation(null,GetMethodInfo("IndexOf", "abcd", 4), new object[] {"abcd", 4});
			Assert.AreEqual("String.IndexOf(\"abcd\", 4);", actual);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void MethodCallCtorWontAcceptNullMethod()
		{
			MethodCallUtil.StringPresentation(null,null, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: args")]
		public void MethodCallCtorWontAcceptNullArgs()
		{
            MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
			MethodCallUtil.StringPresentation(null,method, null);
		}

		[Test]
		public void MethodCallWithArgumentsMissing()
		{
            MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            Assert.AreEqual("String.StartsWith(missing parameter);", MethodCallUtil.StringPresentation(null,method, new object[0]));

		}

		#region Implementation

		private static Type[] TypesFromArgs(object[] args)
		{
			Type[] types = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				types[i] = args[i].GetType();
			}
			return types;
		}

		public static MethodInfo GetMethodInfo(string name, params object[] args)
		{
			Type[] types = TypesFromArgs(args);
			MethodInfo method = typeof (string).GetMethod(name, types);
			return method;
		}

		#endregion
	}
}