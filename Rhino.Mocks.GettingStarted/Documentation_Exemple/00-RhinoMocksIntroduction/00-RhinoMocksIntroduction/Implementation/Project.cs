using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class Project : IProject
    {
        public string Name { get; set; }

        public Project(string projectName)
        {
            this.Name = projectName;
        }
    }
}
