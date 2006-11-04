using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Security.Permissions;
using NUnit.Framework;

[assembly:EnvironmentPermissionAttribute(SecurityAction.RequestMinimum)]


namespace Rhino.Mocks.Tests.Remoting
{

	/// <summary>
	/// Test scenarios where mock objects are called from different
	/// application domain.
	/// </summary>
	///
	[TestFixture]
	public class ContextSwitchTests
	{
		private AppDomain otherDomain;
		private ContextSwitcher contextSwitcher;



		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			System.IO.FileInfo assemblyFile = new System.IO.FileInfo(
				System.Reflection.Assembly.GetExecutingAssembly().Location);

			otherDomain = AppDomain.CreateDomain("other domain", null,
				assemblyFile.DirectoryName, null, false);

			contextSwitcher = (ContextSwitcher)otherDomain.CreateInstanceAndUnwrap(
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
				typeof(ContextSwitcher).FullName);
		}



		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			AppDomain.Unload(otherDomain);
		}



		[Test]
		public void MockInterface()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Return(54);
			demo.VoidStringArg("54");
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
			mocks.VerifyAll();
		}



		[Test]
		public void MockInterfaceWithSameName()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Return(54);
			demo.VoidStringArg("54");
			Other.IDemo remotingDemo = (Other.IDemo)mocks.CreateMock(typeof(Other.IDemo));
			remotingDemo.ProcessString("in");
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
			contextSwitcher.DoStuff(remotingDemo);
			mocks.VerifyAll();
		}



		[Test, ExpectedException(typeof(Exception), "That was expected.")]
		public void MockInterfaceExpectException()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Throw(new Exception("That was expected."));
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test, ExpectedException(typeof(Exceptions.ExpectationViolationException),
						 "IDemo.VoidStringArg(\"34\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(\"bang\"); Expected #1, Actual #0.")]
		public void MockInterfaceUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Return(34);
			demo.VoidStringArg("bang");
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test]
		public void MockClass()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.CreateMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Two()).Return(44);
			mocks.ReplayAll();
			Assert.AreEqual(44, contextSwitcher.DoStuff(demo));
			mocks.VerifyAll();
		}



		[Test, ExpectedException(typeof(Exception), "That was expected for class.")]
		public void MockClassExpectException()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.CreateMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Two()).Throw(new Exception("That was expected for class."));
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test, ExpectedException(typeof(Exceptions.ExpectationViolationException),
						 "RemotableDemoClass.Two(); Expected #0, Actual #1.")]
		public void MockClassUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.CreateMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Prop).Return(11);
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}
	}

}
