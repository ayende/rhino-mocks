using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksIEventRaiser
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class Presenter
    {
        public bool OnLoadCalled = false;

        public Presenter(IView view)
        {
            view.Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            OnLoadCalled = true;
            //the code that we would like to test
        }
    }
}
