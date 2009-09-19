using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Impl.RemotingMock
{
    internal class RemotingProxyDetector : IRemotingProxyOperation
    {
        bool _detected = false;

        public bool Detected
        {
            get { return _detected; }
        }

        public void Process(RemotingProxy proxy)
        {
            _detected = true;
        }
    }
}
