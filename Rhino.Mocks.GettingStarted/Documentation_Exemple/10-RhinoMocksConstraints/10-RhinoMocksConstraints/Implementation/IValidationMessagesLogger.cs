using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksConstraints
{
    public enum ValidationMessageKind { Error, Valid };

    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IValidationMessagesLogger
    {
        void LogMessage(Message arg1);
    }
}
