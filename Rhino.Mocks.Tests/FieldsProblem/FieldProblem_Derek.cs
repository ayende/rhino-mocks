using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture] 
	public class FieldProblem_Derek
	{
		private MockRepository mockRepository;


		[SetUp]
		public void Setup()
		{
			mockRepository = new MockRepository();
		}


		[TearDown]
		public void TearDown()

		{
			mockRepository.VerifyAll();
		}


		[Test]
		public void TestInvalidValue()
		{
			IMockInterface mockedInterface = mockRepository.CreateMock<IMockInterface>();
			Expect.Call(mockedInterface.InvalidValue).Return(100UL);
			mockRepository.ReplayAll();
			Assert.AreEqual(100UL, mockedInterface.InvalidValue);
		}
	}

	public interface IMockInterface
    {

        UInt64 InvalidValue { get; }

    }
}