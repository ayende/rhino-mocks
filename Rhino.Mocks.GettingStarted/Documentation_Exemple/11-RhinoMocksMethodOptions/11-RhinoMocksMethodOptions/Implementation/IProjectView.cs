using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksMethodOptions
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IProjectView
    {
        string Name { get; set; }
        object Ask(string arg1, string arg2);
    }
}
