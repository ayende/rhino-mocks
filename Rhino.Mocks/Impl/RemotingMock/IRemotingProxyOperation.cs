using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Impl.RemotingMock
{
    /// <summary>
    /// Operation on a remoting proxy
    /// </summary>
    /// <remarks>
    /// It is not possible to directly communicate to a real proxy via transparent proxy.
    /// Transparent proxy impersonates a user type and only methods of that user type are callable.
    /// The only methods that are guaranteed to exist on any transparent proxy are methods defined
    /// in Object: namely ToString(), GetHashCode(), and Equals()).
    /// 
    /// These three methods are the only way to tell the real proxy to do something.
    /// Equals() is the most suitable of all, since it accepts an arbitrary object parameter.
    /// The RemotingProxy code is built so that if it is compared to an IRemotingProxyOperation,
    /// transparentProxy.Equals(operation) will call operation.Process(realProxy).
    /// This way we can retrieve a real proxy from transparent proxy and perform
    /// arbitrary operation on it. 
    /// </remarks>
    internal interface IRemotingProxyOperation
    {
        void Process(RemotingProxy proxy);
    }
}
