#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


#if DOTNET35

using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Kythorn
	{
		[Fact]
		public void CallingAssertWasCalledOnAnObjectThatIsInRecordModeShouldResultInFailure()
		{
			var service = MockRepository.GenerateStub<IService>();
			var view = new MockRepository().Stub<IView>();
			service.Stub(x => x.GetString()).Return("Test");
			var presenter = new Presenter(view, service);
			presenter.OnViewLoaded();
			Assert.Throws<InvalidOperationException>(
				"Cannot assert on an object that is not in replay mode. Did you forget to call ReplayAll() ?",
				() => view.AssertWasCalled(x => x.Message = "Test"));
		}

		[Fact]
		public void CanUseStubSyntaxOnMocksInRecordMode()
		{
			MockRepository mocks = new MockRepository();
			var service = mocks.Stub<IService>();
			var view = mocks.Stub<IView>();
			service.Stub(x => x.GetString()).Return("Test");
			var presenter = new Presenter(view, service);
			mocks.ReplayAll();
			presenter.OnViewLoaded();
			view.AssertWasCalled(x => x.Message = "Test");
		}

		[Fact]
		public void Success()
		{
			var service = MockRepository.GenerateStub<IService>();
			var view = MockRepository.GenerateStub<IView>();
			service.Stub(x => x.GetString()).Return("Test");
			view.Expect(x => x.Message = "Test");
			var presenter = new Presenter(view, service);
			presenter.OnViewLoaded();
			view.VerifyAllExpectations();
		}

		public interface IService
		{
			string GetString();
		}

		public interface IView
		{
			string Message { set; }
		}
		public class Presenter
		{
			private readonly IService service;
			private readonly IView view;

			public Presenter(IView view, IService service)
			{
				this.view = view;
				this.service = service;
			}


			public void OnViewLoaded()
			{
				view.Message = service.GetString();
			}
		}
	}

	
}
#endif
