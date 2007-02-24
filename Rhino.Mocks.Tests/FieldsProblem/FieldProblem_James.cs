using System;
using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_James
	{
		private MockRepository m_mockery;

		[SetUp]
		public void Setup()
		{
			m_mockery = new MockRepository();
		}

		[Test]
		public void ShouldBeAbleToMockGenericMethod()
		{
			ILookupMapper<int> mapper = m_mockery.CreateMock<ILookupMapper<int>>();
			List<Foo<int>> retval = new List<Foo<int>>();
			retval.Add(new Foo<int>());
			Expect.Call(mapper.FindAllFoo()).Return(retval);
			m_mockery.ReplayAll();
			IList<Foo<int>> listOfFoo = mapper.FindAllFoo();
			m_mockery.VerifyAll();
		}

		[Test]
		public void ShouldBeAbleToMockGenericMethod2()
		{
			ILookupMapper<int> mapper = m_mockery.CreateMock<ILookupMapper<int>>();
			Foo<int> retval = new Foo<int>();
			Expect.Call(mapper.FindOneFoo()).Return(retval);
			m_mockery.ReplayAll();
			Foo<int> oneFoo = mapper.FindOneFoo();
			m_mockery.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Type 'Rhino.Mocks.Tests.FieldsProblem.Foo`1[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]' doesn't match the return type 'Rhino.Mocks.Tests.FieldsProblem.Foo`1[System.Int32]' for method 'ILookupMapper`1.FindOneFoo();'")]
		public void ShouldGetValidErrorWhenGenericTypeMismatchOccurs()
		{
			ILookupMapper<int> mapper = m_mockery.CreateMock<ILookupMapper<int>>();
			Foo<string> retval = new Foo<string>();
			Expect.Call(mapper.FindOneFoo()).Return(retval);
		}
	}

	public interface ILookupMapper<T>
	{
		IList<Foo<T>> FindAllFoo();
		Foo<T> FindOneFoo();
	}

	public class Foo<T>
	{
		public T GetOne()
		{
			return default(T);
		}
	}
}