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