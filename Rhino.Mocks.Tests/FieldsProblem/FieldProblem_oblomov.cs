using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Xunit;

	public interface IMyService
	{
		void Func1();
		void Func2();
		void Func3();
	}

	
	public class FieldProblem_oblomov : IDisposable
	{
		MockRepository mocks;
		IMyService service;

		public FieldProblem_oblomov()
		{
			mocks = new MockRepository();
			service = mocks.StrictMock<IMyService>();
		}
		public void Dispose()
		{
			mocks.VerifyAll();
		}
		[Fact]
		public void TestWorks()
		{
			using (mocks.Ordered())
			{
				using (mocks.Unordered())
				{
					service.Func1();
					service.Func2();
				}
				service.Func3();
			}
			mocks.ReplayAll();

			service.Func2();
			service.Func1();
			service.Func3();
		}

		[Fact]
		public void TestDoesnotWork()
		{
			using (mocks.Ordered())
			{
				using (mocks.Unordered())
				{
					//service.Func1();
					//service.Func2();
				}
				service.Func3();
			}
			mocks.ReplayAll();

			//service.Func2();
			//service.Func1();
			service.Func3();
		}

		[Fact]
		public void TestDoesnotWork2()
		{
			using (mocks.Ordered())
			{
				using (mocks.Ordered())
				{
					//service.Func1();
					//service.Func2();
				}
				service.Func3();
			}
			mocks.ReplayAll();

			//service.Func2();
			//service.Func1();
			service.Func3();
		}
	}
}
