using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksSetupResult
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IDemo
    {
        string Prop { get; set; }
        void VoidNoArgs();
    }
}
