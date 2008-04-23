using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class FieldProblem_Will
    {
        [Test]
        public void HostingMockedService()
        {
            MockRepository mocks = new MockRepository();
            IServiceClassInterface mock = mocks.CreateMock<ServiceClassImpl>();

            ServiceHost host = new ServiceHost(mock, new Uri("net.tcp://localhost:9876/MyService"));
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IServiceClassInterface), new NetTcpBinding(), "net.tcp://localhost:9876/MyService");
            KeyedByTypeCollection<IEndpointBehavior> behaviors = endpoint.Behaviors;
            host.Open();
            host.Close();

        }
    }
    [ServiceContract]
    public interface IServiceClassInterface
    {
        [OperationContract]
        void Foo();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceClassImpl : IServiceClassInterface
    {
        public virtual void Foo()
        {
            
        }
    }
}