namespace Rhino.Mocks.Impl.RemotingMock
{
    using Rhino.Mocks.Interfaces;

    class RemotingProxyMockedObjectGetter : IRemotingProxyOperation
    {
        IMockedObject _mockedObject;

        public IMockedObject MockedObject
        {
            get { return _mockedObject; }
        }

        public void Process(RemotingProxy proxy)
        {
            _mockedObject = proxy.MockedObject;
        }
    }
}
