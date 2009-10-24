namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using System.Collections.Generic;
	using Xunit;
	using Rhino.Mocks.Constraints;

	
	public class FieldProblem_Lee
	{
		[Fact]
		public void IgnoringArgumentsOnGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			IHaveGenericMethod mock = mocks.StrictMock<IHaveGenericMethod>();
			
			mock.Foo(15);
			LastCall.IgnoreArguments().Return(true);

			mocks.ReplayAll();

			bool result = mock.Foo(16);
			Assert.True(result);
			mocks.VerifyAll();
		}


		[Fact]
		public void WithGenericMethods()
		{
			MockRepository mocks = new MockRepository();
			IFunkyList<int> list = mocks.DynamicMock<IFunkyList<int>>();
			Assert.NotNull(list);
			List<Guid> results = new List<Guid>();
			Expect.Call(list.FunkItUp<Guid>(null, null))
				.IgnoreArguments()
				.Constraints(Is.Equal("1"), Is.Equal(2))
				.Return(results);
			mocks.ReplayAll();
			Assert.Same(results, list.FunkItUp<Guid>("1", 2));
		}
    }

	public interface IFunkyList<T> : IList<T>
	{
		ICollection<T2> FunkItUp<T2>(object arg1, object arg2);
	}

	public interface IHaveGenericMethod
	{
		bool Foo<T>(T obj);
	}
}