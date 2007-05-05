using System;
using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class StubTest
	{
		[Test]
		public void StubHasPropertyBehaviorForAllProperties()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			animal.Legs = 4;
			Assert.AreEqual(4, animal.Legs);

			animal.Name = "Rose";
			Assert.AreEqual("Rose", animal.Name);

			Assert.IsNull(animal.Species, "Should return default value if not set");
			animal.Species = "Caucasusian Shepherd";
			Assert.AreEqual("Caucasusian Shepherd", animal.Species);
		}

		[Test]
		public void CanRegisterToEventsAndRaiseThem()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			animal.Hungry += null; //Note, no expectation!
			IEventRaiser eventRaiser = LastCall.GetEventRaiser();

			bool raised = false;
			animal.Hungry += delegate
			{
				raised = true;
			};

			eventRaiser.Raise(animal, EventArgs.Empty);
			Assert.IsTrue(raised);
		}

		[Test]
		public void CallingMethodOnStubsDoesNotCreateExpectations()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			using (mocks.Record())
			{
				animal.Legs = 4;
				animal.Name = "Rose";
				animal.Species = "Caucasusian Shepherd";
				animal.GetMood();
			}
			mocks.VerifyAll();
		}

		[Test]
		public void CanCreateExpectationOnMethod()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			using (mocks.Record())
			{
				animal.Legs = 4;
				animal.Name = "Rose";
				animal.Species = "Caucasusian Shepherd";
				animal.GetMood();
				LastCall.Return("Happy");
			}
			Assert.AreEqual("Happy",  animal.GetMood());
			mocks.VerifyAll();
		}
	}

	public interface IAnimal
	{
		int Legs { get; set; }
		int Eyes { get; set; }
		string Name { get; set; }
		string Species { get; set; }

		event EventHandler Hungry;
		string GetMood();
	}
}