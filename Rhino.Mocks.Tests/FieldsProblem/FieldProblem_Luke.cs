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
				mocks.CreateMultiMock(typeof(IServiceProvider), typeof (IHTMLDataTransfer));
			Assert.IsNotNull(serviceProvider);
		}
	}

	[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
	 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IServiceProvider
	{
		[return : MarshalAs(UnmanagedType.IUnknown)]
		object QueryService([In] ref Guid guidService, [In] ref Guid riid);
	}
}