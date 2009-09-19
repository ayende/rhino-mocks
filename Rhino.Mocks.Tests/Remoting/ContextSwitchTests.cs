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
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
[assembly:EnvironmentPermission(SecurityAction.RequestMinimum)]


namespace Rhino.Mocks.Tests.Remoting
{

	/// <summary>
	/// Test scenarios where mock objects are called from different
	/// application domain.
	/// </summary>
	[TestFixture]
	public class ContextSwitchTests
	{
		private AppDomain otherDomain;
		private ContextSwitcher contextSwitcher;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			FileInfo assemblyFile = new FileInfo(
				Assembly.GetExecutingAssembly().Location);

			otherDomain = AppDomain.CreateDomain("other domain", null,
				AppDomain.CurrentDomain.BaseDirectory, null, false);

			contextSwitcher = (ContextSwitcher)otherDomain.CreateInstanceAndUnwrap(
				Assembly.GetExecutingAssembly().GetName().Name,
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
			IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
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
			IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Return(54);
			demo.VoidStringArg("54");
			Other.IDemo remotingDemo = (Other.IDemo)mocks.StrictMock(typeof(Other.IDemo));
			remotingDemo.ProcessString("in");
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
			contextSwitcher.DoStuff(remotingDemo);
			mocks.VerifyAll();
		}



		[Test, ExpectedException(typeof(InvalidOperationException), "That was expected.")]
		public void MockInterfaceExpectException()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Throw(new InvalidOperationException("That was expected."));
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test, ExpectedException(typeof(ExpectationViolationException),
						 "IDemo.VoidStringArg(\"34\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(\"bang\"); Expected #1, Actual #0.")]
		public void MockInterfaceUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
			Expect.Call(demo.ReturnIntNoArgs()).Return(34);
			demo.VoidStringArg("bang");
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test]
		public void MockClass()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.StrictMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Two()).Return(44);
			mocks.ReplayAll();
			Assert.AreEqual(44, contextSwitcher.DoStuff(demo));
			mocks.VerifyAll();
		}



		[Test, ExpectedException(typeof(InvalidOperationException), "That was expected for class.")]
		public void MockClassExpectException()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.StrictMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Two()).Throw(new InvalidOperationException("That was expected for class."));
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}



		[Test, ExpectedException(typeof(ExpectationViolationException),
						 "RemotableDemoClass.Two(); Expected #0, Actual #1.")]
		public void MockClassUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			RemotableDemoClass demo = (RemotableDemoClass)mocks.StrictMock(typeof(RemotableDemoClass));
			Expect.Call(demo.Prop).Return(11);
			mocks.ReplayAll();
			contextSwitcher.DoStuff(demo);
		}
	}

}
