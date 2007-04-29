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


using System;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_JohnOpincar
	{
		public interface IDaSchedulerView
		{
			DateTime DateOf { set; }
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDaSchedulerView.set_DateOf(08/08/2006 00:00:00); Expected #1, Actual #0.")]
		public void CanGetExpectationExceptionFromPropertySetter()
		{
			MockRepository m_mocks;
			IDaSchedulerView m_view;
			m_mocks = new MockRepository();
			m_view = (IDaSchedulerView)
			m_mocks.CreateMock(typeof(IDaSchedulerView));
//			DaSchedulerPresenter presenter = new DaSchedulerPresenter(m_view, new TestScheduleLoader(0)); 
			m_view.DateOf = new DateTime(2006,8,8); 
			//LastCall.IgnoreArguments(); 
			m_mocks.ReplayAll(); 
			//presenter.Initialize(); 
			m_mocks.VerifyAll();
		}
	}
}
