using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RhinoMocksRecordPlaybackSyntax
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    class ComponentImplementation : IComponent
    {
        public virtual ISite Site { get; set; }
        public IDependency Dependency { get; set; }
        public event EventHandler Disposed;

        public ComponentImplementation(IDependency dependency)
        {
            this.Dependency = dependency;
        }

        public virtual void Dispose()
        {
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        public object TestMethod()
        {
            return Dependency.SomeMethod();
        }
    }
}
