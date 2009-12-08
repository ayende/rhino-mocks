using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksEvents
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IEventSubscriber
    {
        void Handler(object sender, EventArgs e); 
    }
}
