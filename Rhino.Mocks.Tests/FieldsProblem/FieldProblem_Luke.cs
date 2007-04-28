using System;
using System.Runtime.InteropServices;
using MbUnit.Framework;
using mshtml;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Luke
	{
		[Test]
		public void CanMockIE()
		{
			MockRepository mockRepository = new MockRepository();
			IHTMLEventObj2 mock = mockRepository.CreateMock<IHTMLEventObj2>();
			Assert.IsNotNull(mock);
		}

		[Test]
		public void CanMockComInterface()
		{
			MockRepository mocks = new MockRepository();
			IServiceProvider serviceProvider = (IServiceProvider)
											   mocks.CreateMultiMock(typeof(IServiceProvider), typeof(IHTMLDataTransfer));
			Assert.IsNotNull(serviceProvider);
		}

		[Test] 
		public void TryToMockClassWithProtectedInternalAbstractClass()
		{
			MockRepository mockRepository = new MockRepository();
			mockRepository.CreateMock<SomeClassWithProtectedInternalAbstractClass>();
		}

		[Test] 
		public void TryToMockClassWithProtectedAbstractClass()
		{
			MockRepository mockRepository = new MockRepository();
			mockRepository.CreateMock<SomeClassWithProtectedAbstractClass>();
		}

		public abstract class
			SomeClassWithProtectedInternalAbstractClass
		{
			protected internal abstract void Quack();
		}

		public abstract class SomeClassWithProtectedAbstractClass
		{
			protected abstract void Quack();
		}
	}

	[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IServiceProvider
	{
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object QueryService([In] ref Guid guidService, [In] ref Guid riid);
	}

}