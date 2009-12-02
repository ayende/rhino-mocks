using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksIEventRaiser
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IView
    {
        event EventHandler Load;
    }
}
