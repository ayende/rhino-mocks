using System;
using System.Text;
using Rhino.Mocks;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;

public class Metric : IMetric
{
  IApplicationSession m_objIApplicationSession;
  Metric(IApplicationSession pv_objIApplicationSession, DataSet pv_objDataSet)
  {
    m_objIApplicationSession = pv_objIApplicationSession;
  }



  public IMetric Numerator
  {
    get
    {
      return Metric.GetByName(m_objIApplicationSession,"Numerator");
    }
  }

  public static IMetric GetByName(IApplicationSession pv_objIApplicationSession, string pv_strMetricName)
  {
    return new Metric(pv_objIApplicationSession, pv_objIApplicationSession.IMetricBroker.FetchMetric(pv_strMetricName));
  }

  public static IMetric GetByID(IApplicationSession pv_objIApplicationSession, long pv_lMetricID)
  {
    return new Metric(pv_objIApplicationSession, pv_objIApplicationSession.IMetricBroker.FetchMetric(pv_lMetricID));
  }
}


public interface IMetric
{
  IMetric Numerator { get;}
}

public interface IApplicationSession
{
  IMetricBroker IMetricBroker { get;}
}

public interface IMetricBroker
{
  DataSet FetchMetric(string MetricName);
  DataSet FetchMetric(long MetricID);
}



#region TestFixture
[TestFixture]
public class MyTestFixture
{
  MockRepository m_objMockRepository;
  IApplicationSession m_objIApplication;
  IMetricBroker m_objIMetricBroker;

  DataSet m_objDSRatioMetric;

  IMetric m_objIMetric;

  [TestFixtureSetUp]
  public void TFsetup()
  {
    #region Mock Objects
    m_objMockRepository = new MockRepository();
    m_objIApplication = (IApplicationSession)m_objMockRepository.CreateMock(typeof(IApplicationSession));
    m_objIMetricBroker = (IMetricBroker)m_objMockRepository.CreateMock(typeof(IMetricBroker));
    #endregion

    #region DataSets

    m_objDSRatioMetric = new DataSet();

    using (m_objMockRepository.Ordered())
    {
      Rhino.Mocks.Expect.Call(m_objIApplication.IMetricBroker).Return(m_objIMetricBroker);
      Rhino.Mocks.Expect.Call(m_objIMetricBroker.FetchMetric("Risk")).Return(m_objDSRatioMetric);
    }
    m_objMockRepository.ReplayAll();

    #endregion

    m_objIMetric = Metric.GetByName(m_objIApplication,"Risk");
    m_objMockRepository.VerifyAll();
  }
  [Test]
	[ExpectedException(typeof(ExpectationViolationException), "IApplicationSession.get_IMetricBroker(); Expected #0, Actual #1.")]
  public void T001_SavingShouldNotInvalidateOtherCachedSingleObjects()
  {
    m_objMockRepository.BackToRecord(m_objIApplication);
    m_objMockRepository.BackToRecord(m_objIMetricBroker);
    long lOtherID = 200;
    DataSet objDSOtherMetric = new DataSet();

    using (m_objMockRepository.Ordered())
    {
      Rhino.Mocks.Expect.Call(m_objIApplication.IMetricBroker).Return(m_objIMetricBroker);
      Rhino.Mocks.Expect.Call(m_objIMetricBroker.FetchMetric(lOtherID)).Return(objDSOtherMetric);
    }
    m_objMockRepository.ReplayAll();

    IMetric objOtherMetric = Metric.GetByID(m_objIApplication, lOtherID);
    m_objMockRepository.VerifyAll();

    m_objMockRepository.BackToRecord(m_objIApplication);
    m_objMockRepository.BackToRecord(m_objIMetricBroker);

    m_objMockRepository.ReplayAll();
    //missing expectations here. 

    //cause a stack overflow error
    IMetric objMetric = m_objIMetric.Numerator;
    m_objMockRepository.VerifyAll();
  }

}

#endregion