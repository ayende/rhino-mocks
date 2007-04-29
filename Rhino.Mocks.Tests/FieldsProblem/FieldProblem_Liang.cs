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
using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public interface IView
	{
		event EventHandler Init;
		event EventHandler Load;
		bool IsPostBack { get; }
		void DataBind();
		bool IsValid { get; }
		void Alert(string message);
	}

	public interface IPresenter<IView>
	{
		void Initialize();
		void Load();
	}

	public abstract class PresenterBase<TView> : IPresenter<IView>
		where TView : IView
	{
		protected TView view;

		public PresenterBase()
		{
		}

		public PresenterBase(TView view)
		{
			this.view = view;
			view.Init += new EventHandler(OnViewInit);
			view.Load += new EventHandler(OnViewLoad);
		}


		protected void OnViewInit(object sender, EventArgs e)
		{
			Initialize();
		}

		protected void OnViewLoad(object sender, EventArgs e)
		{
			if (!view.IsPostBack)
			{
				Load();
			}
		}

		public virtual void Initialize()
		{
		}

		public virtual void Load()
		{
		}
	}

	[TestFixture]
	public class PresenterBaseTestFixture
	{
		private MockRepository mocks;
		private IView viewMocks;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			viewMocks = (IView) mocks.DynamicMock(typeof (IView));
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void TestEventInitialization()
		{
			viewMocks.Init += null; //also set expectation
			IEventRaiser init = LastCall.IgnoreArguments().GetEventRaiser();
			viewMocks.Load += null; //also set expectation
			IEventRaiser load = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.Replay(viewMocks); //we move just this to replay state.
			PresenterBase<IView> presenterBase =
				mocks.CreateMock<PresenterBase<IView>>(viewMocks);
			presenterBase.Initialize();
			presenterBase.Load();
			mocks.ReplayAll();
			init.Raise(viewMocks, EventArgs.Empty); //raise Init method
			load.Raise(viewMocks, EventArgs.Empty); //raise Load method
		}
	}
}