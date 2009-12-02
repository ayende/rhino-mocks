using System;
using System.Collections.Generic;
using System.Text;

namespace RhinoMocksStubs
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IAnimal
    {
        int Legs { get; set; }
        int Eyes { get; set; }
        string Name { get; set; }
        string Species { get; set; }

        event EventHandler Hungry;
        string GetMood();
    }
}
