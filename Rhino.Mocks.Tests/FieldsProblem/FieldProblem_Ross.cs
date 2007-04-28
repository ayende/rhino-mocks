using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Ross
	{
		[Test]
		public void GenericMethodWithConstrait()
		{
			MockRepository mocks = new MockRepository();

			IClass1 class1 = mocks.CreateMock<IClass1>();
			IClass2 class2 = mocks.CreateMock<IClass2>();

			class1.Method1<int>(1);
			class2.Method2(mocks);
		}

		public interface IClass1
		{
			void Method1<T1>(T1 t1);
		}

		public interface IClass2
		{
			void Method2<T2>(T2 t2) where T2 : class;
		}

	}
}
