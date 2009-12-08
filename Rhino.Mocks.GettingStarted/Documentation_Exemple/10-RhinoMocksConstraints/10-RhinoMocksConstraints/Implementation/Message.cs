using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksConstraints
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class Message
    {
        public string Text { get; set; }
        public ValidationMessageKind MessageKind { get; set; }
    }
}
