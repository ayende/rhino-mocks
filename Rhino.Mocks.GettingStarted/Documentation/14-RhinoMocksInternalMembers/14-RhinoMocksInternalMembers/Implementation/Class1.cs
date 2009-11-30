using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksInternalMembers
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    internal class Class1
    {
        internal virtual string TestMethod()
        {
            return "TestMethod";
        }

        internal virtual string TestProperty
        {
            get { return "TestProperty"; }
        }
    }
}
