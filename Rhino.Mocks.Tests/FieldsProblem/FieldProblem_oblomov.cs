using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	public interface IMyService
	{
		void Func1();
		void Func2();
		void Func3();
	}

	[TestFixture]
	public class FieldProblem_oblomov
	{
		MockRepository mocks;
		IMyService service;

		[SetUp]
		public void Init()
		{
			mocks = new MockRepository();
			service = mocks.CreateMock<IMyService>();
		}
		[TearDown]
		public void Verify()
		{
			mocks.VerifyAll();
		}
		[Test]
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

		[Test]
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

		[Test]
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
