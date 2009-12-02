using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksEvents
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class WithEvents : IWithEvents
    {
        #region IWithEvents Members
        public event System.EventHandler Blah;

        public void RaiseEvent()
        {
            if (Blah != null)
                Blah(this, EventArgs.Empty);
        }
        #endregion
    }  
}
