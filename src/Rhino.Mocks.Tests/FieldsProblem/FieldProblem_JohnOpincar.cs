using System;
using NUnit.Framework;
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
			m_view.DateOf = DateTime.Today; 
			//LastCall.IgnoreArguments(); 
			m_mocks.ReplayAll(); 
			//presenter.Initialize(); 
			m_mocks.VerifyAll();
		}
	}
}
