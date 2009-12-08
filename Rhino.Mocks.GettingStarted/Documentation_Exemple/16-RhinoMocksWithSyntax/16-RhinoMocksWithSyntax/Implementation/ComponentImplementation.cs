using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RhinoMocksWithSyntax
{
    /// <summary>
    /// Minimal implementation to compile
    /// </summary>
    class ComponentImplementation : IComponent
    {
        public virtual ISite Site { get; set; }
        public IDependency Dependency { get; set; }
        public IAnotherDependency AnotherDependency { get; set; }
        public event EventHandler Disposed;

        public ComponentImplementation(IDependency dependency)
        {
            this.Dependency = dependency;
        }

        public ComponentImplementation(IDependency dependency, IAnotherDependency anotherDependency)
        {
            this.Dependency = dependency;
            this.AnotherDependency = anotherDependency;
        }

        public virtual void Dispose()
        {
            //There is nothing to clean.
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        public object TestMethod()
        {
            object obj = Dependency.SomeMethod();

            if (AnotherDependency != null)
                AnotherDependency.SomeOtherMethod();

            return obj;
        }
    }
}
