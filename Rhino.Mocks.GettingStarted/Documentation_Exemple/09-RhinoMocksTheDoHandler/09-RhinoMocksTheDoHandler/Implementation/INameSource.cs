using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksTheDoHandler
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface INameSource
    {
        string CreateName(string firstName, string surname);
    }
}
