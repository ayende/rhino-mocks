using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksConstraints
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class Validator : IValidator
    {
        #region IValidator Members

        public void Validate(string p, IValidationMessagesLogger loggerMock)
        {
            Message msg = new Message();

            msg.Text = "Whoa!";
            msg.MessageKind = ValidationMessageKind.Error;

            loggerMock.LogMessage(msg);
        }

        #endregion
    }
}
