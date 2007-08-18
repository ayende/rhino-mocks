using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Maurice
	{
		[Test]
		public void TwoGenericParametersWithConstraints()
		{
			MockRepository mocks = new MockRepository();
			IDemo2 demo = mocks.CreateMock<IDemo2>();
			Assert.IsNotNull(demo);
		}
	}

	public interface IDemo2
	{
		T CreateRequest<T, R>()
			where T : Request<R>
			where R : Response;
	}

	public class Request<T> where T : Response
	{
	}

	public class Response
	{
	}
}
