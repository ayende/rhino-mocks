namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using Interfaces;
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Keith
	{
		public class Model
		{
			private string userName;
			public string UserName 
			{ 
				get { return userName; } 
				set { userName = value; } 
			}
		}

		public interface IView
		{
			Model Model { get; set; }
			event EventHandler ClickButton;
		}

		public class Controller
		{
			Model _model = null;

			public Controller(IView view)
			{
				// The controller owns the model, in this example its only
				// used by one view but in real live the reference to the
				// Model can be used by mutiple views. Given this I dont want
				// to send it in via the constructor.
				_model = new Model();
				view.Model = _model;

				view.ClickButton += new EventHandler(View_ClickButton);
			}

			void View_ClickButton(object sender, EventArgs e)
			{
				_model.UserName = "Keith here :)";
			}
		}

		[Test]
		public void Test_View_Events_WiredUp()
		{
			MockRepository mocks = new MockRepository();

			IView view = mocks.StrictMock<IView>();

			// expect that the model is set on the view
			// NOTE: if I move this Expect.Call above
			// the above Expect.Call, Rhino mocks blows up on with an
			// "This method has already been set to ArgsEqualExpectation."
			// not sure why. Its a side issue.
			Expect.Call(view.Model = Arg<Model>.Is.NotNull);

			// expect the event ClickButton to be wired up
			IEventRaiser clickButtonEvent =
					Expect.Call(delegate
					{
						view.ClickButton += null;
					}).IgnoreArguments().GetEventRaiser();

			// Q: How do i set an expectation that checks that the controller
			// correctly updates the model in the event handler.
			// i.e. above we know that the controller executes
			// _model.UserName = "Keith here :)"
			// but how can I verify it?
			// The following wont work, because Model is null:
			// Expect.Call(view.Model.UserName = Arg<String>.Is.Anything);

			mocks.ReplayAll();

			Controller controller = new Controller(view);
			clickButtonEvent.Raise(null, null);

			mocks.VerifyAll();
		}
	}
}