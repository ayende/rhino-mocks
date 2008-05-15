namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Lopez
	{
		public interface GenericContainer<T>
		{
			T Item { get; set; }
		}


		[Test]
		public void PropertyBehaviorForSinglePropertyTypeOfString()
		{
			MockRepository mocks = new MockRepository();

			GenericContainer<string> stringContainer = mocks.StrictMock<GenericContainer<string>>();

			Expect.Call(stringContainer.Item).PropertyBehavior();

			mocks.Replay(stringContainer);

			for (int i = 1; i < 49; ++i)
			{
				string newItem = i.ToString();

				stringContainer.Item = newItem;

				Assert.AreEqual(newItem, stringContainer.Item);
			}

			mocks.Verify(stringContainer);
		}


		[Test]
		public void PropertyBehaviourForSinglePropertyTypeOfDateTime()
		{
			MockRepository mocks = new MockRepository();

			GenericContainer<DateTime> dateTimeContainer = mocks.StrictMock<GenericContainer<DateTime>>();

			Expect.Call(dateTimeContainer.Item).PropertyBehavior();

			mocks.Replay(dateTimeContainer);

			for (int i = 1; i < 12; i++)
			{
				DateTime date = new DateTime(2007, i, i);

				dateTimeContainer.Item = date;

				Assert.AreEqual(date, dateTimeContainer.Item);
			}

			mocks.Verify(dateTimeContainer);
		}


		[Test]
		public void PropertyBehaviourForSinglePropertyTypeOfInteger()
		{
			MockRepository mocks = new MockRepository();

			GenericContainer<int> dateTimeContainer = mocks.StrictMock<GenericContainer<int>>();

			Expect.Call(dateTimeContainer.Item).PropertyBehavior();

			mocks.Replay(dateTimeContainer);

			for (int i = 1; i < 49; i++)
			{
				dateTimeContainer.Item = i;

				Assert.AreEqual(i, dateTimeContainer.Item);
			}

			mocks.Verify(dateTimeContainer);
		}
	}
}