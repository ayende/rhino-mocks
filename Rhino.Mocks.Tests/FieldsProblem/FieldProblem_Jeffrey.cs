using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Jeffrey
	{
		[Test]
		public void DelegateToGenericMock()
		{
			MockRepository mocks = new MockRepository();
			IEMailFormatter<string> formatterMock = mocks.CreateMock<IEMailFormatter<string>>();
			SmtpEMailSenderBase<string> senderMock = (SmtpEMailSenderBase<string>)mocks.CreateMock(typeof(SmtpEMailSenderBase<string>));
			senderMock.SetFormatter(formatterMock);
			LastCall.Do((Action<IEMailFormatter<string>>)delegate(IEMailFormatter<string> formatter)
			{
				Assert.IsNotNull(formatter);
			});
			mocks.ReplayAll();

			senderMock.SetFormatter( formatterMock );

			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Callback arguments didn't match the method arguments")]
		public void Invalid_DelegateToGenericMock()
		{
			MockRepository mocks = new MockRepository();
			IEMailFormatter<string> formatterMock = mocks.CreateMock<IEMailFormatter<string>>();
			SmtpEMailSenderBase<string> senderMock = (SmtpEMailSenderBase<string>)mocks.CreateMock(typeof(SmtpEMailSenderBase<string>));
			senderMock.SetFormatter(formatterMock);
			LastCall.Do((Action<IEMailFormatter<int>>)delegate(IEMailFormatter<int> formatter)
			{
				Assert.IsNotNull(formatter);
			});
		}
	}
	public interface IEMailFormatter<T>
	{
		
	}
	public abstract class SmtpEMailSenderBase<T>
    {

		public virtual void SetFormatter(IEMailFormatter<T> formatter)
        {
        }
    }
}
