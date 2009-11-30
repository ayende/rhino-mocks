using System;
using System.Collections.Generic;
using System.Text;

namespace RhinoMocksPartialMocks
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public abstract class ProcessorBase
    {
        public int Register;

        public virtual int Inc()
        {
            Register = Add(1);
            return Register;
        }

        public abstract int Add(int i);
    }
}
