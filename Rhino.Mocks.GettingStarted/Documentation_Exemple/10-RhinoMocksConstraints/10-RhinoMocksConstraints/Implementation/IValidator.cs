using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksConstraints
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IValidator
    {
        void Validate(string p, IValidationMessagesLogger loggerMock);
    }
}
