using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace RhinoMocksConstraints
{
    /// <summary>
    /// Constraints are a way to verify that a method's arguments match a certain criteria.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Constraints.ashx"/>
    public class RhinoMocksConstraintsTest
    {
        [Test]
        public void ConstraintsGroupingTest()
        {
            MockRepository mocks = new MockRepository();
            IValidationMessagesLogger loggerMock = mocks.DynamicMock<IValidationMessagesLogger>(null);
            IValidator Validator = new Validator();

            using (mocks.Record())
            {
                loggerMock.LogMessage(null);
                LastCall.Constraints(/*LastCall notation is used because LogMessage(...) does not return anything (is void).*/
                  new PropertyConstraint("Text", Rhino.Mocks.Constraints.Text.Contains("Whoa!")) &&
                  Property.Value("MessageKind", ValidationMessageKind.Error))
                    .Repeat.Once();
            }//implicitly calls mocks.ReplayAll()

            Validator.Validate("input", loggerMock);//a validator that uses logger internally to log validation messages.

            mocks.VerifyAll();
        }

        [Test]
        public void ConstraintsGroupingTest_AAA()
        {
            //Arrange
            IValidationMessagesLogger loggerMock = MockRepository.GenerateMock<IValidationMessagesLogger>(null);

            loggerMock.Expect(l => l.LogMessage(null)).IgnoreArguments().Constraints(
                new PropertyConstraint("Text", Rhino.Mocks.Constraints.Text.Contains("Whoa!")) &&
              Property.Value("MessageKind", ValidationMessageKind.Error))
                .Repeat.Once();

            //Act
            IValidator Validator = new Validator();
            Validator.Validate("input", loggerMock);//a validator that uses logger internally to log validation messages.

            //Assert
            loggerMock.VerifyAllExpectations();
        }
    }
}
