using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_DavidG
	{
		[Test]
		public void GenericConstrainedMethod()
		{
			MockRepository mocks = new MockRepository();
			IStore1 store1 = mocks.DynamicMock<IStore1>();
			IStore2 store2 = mocks.DynamicMock<IStore2>();
			Assert.IsNotNull(store2);
			Assert.IsNotNull(store1);
		}
	}

	public interface IStore1
	{
		R[] TestMethod<R>();
	}

	public interface IStore2
	{
		R[] TestMethod<R>() where R : DomainObject<R>;
	}

	public class MyTestObject : DomainObject<MyTestObject>
	{
	}

	public abstract class DomainObject<T> where T : DomainObject<T>
	{
	}
}
