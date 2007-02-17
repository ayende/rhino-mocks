using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Michael
	{
		public interface IProvider
		{
			char GetChar();
			int GetInt32();
		}
		
		[Test]
		public void TestChar()
		{
			MockRepository mocks = new MockRepository();
			IProvider mockProvider = (IProvider)mocks.CreateMock(typeof(IProvider));
			SetupResult.For(mockProvider.GetChar()).Return('X');
			mocks.ReplayAll();
			Assert.AreEqual('X', mockProvider.GetChar()); // actual is a random char
		}

		[Test]
		public void TestInt32()
		{
			MockRepository mocks = new MockRepository();
			IProvider mockProvider = (IProvider)mocks.CreateMock(typeof(IProvider));
			SetupResult.For(mockProvider.GetInt32()).Return(100);
			mocks.ReplayAll();
			Assert.AreEqual(100, mockProvider.GetInt32()); // actual is 100
		}

	}
}
