#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks
{
    /// <summary>
    /// Allows easier access to MockRepository, works closely with Mocker.Current to
    /// allow access to a context where the mock repository is automatially verified at
    /// the end of the code block.
    /// </summary>
    public static class With
    {
        /// <summary>
        /// A method with no arguments and no return value that will be called under the mock context.
        /// </summary>
        public delegate void Proc();
        
        /// <summary>
        /// Initialize a code block where Mocker.Current is initialized.
        /// At the end of the code block, all the expectation will be verified.
        /// This overload will create a new MockRepository.
        /// </summary>
        /// <param name="methodCallThatHasMocks">The code that will be executed under the mock context</param>
        public static void Mocks(Proc methodCallThatHasMocks)
        {
            MockRepository mocks = new MockRepository();
            Mocks(mocks, methodCallThatHasMocks);
        }

        /// <summary>
        /// Initialize a code block where Mocker.Current is initialized.
        /// At the end of the code block, all the expectation will be verified.
        /// This overload will create a new MockRepository.
        /// </summary>
        /// <param name="mocks">The mock repository to use, at the end of the code block, VerifyAll() will be called on the repository.</param>
        /// <param name="methodCallThatHasMocks">The code that will be executed under the mock context</param>
        public static void Mocks(MockRepository mocks, Proc methodCallThatHasMocks)
        {
            Mocker.Current = mocks;
            try
            {
                methodCallThatHasMocks();
                mocks.VerifyAll();
            }
            finally
            {
                Mocker.Current = null;
            }
        }
    }
}
#endif