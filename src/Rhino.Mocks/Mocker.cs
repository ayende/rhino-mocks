#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks
{
    /// <summary>
    /// Accessor for the current mocker
    /// </summary>
    public static class Mocker
    {
        static MockRepository current;

        /// <summary>
        /// The current mocker
        /// </summary>
        public static MockRepository Current
        {
            get
            {
                if (current == null)
                    throw new InvalidOperationException("You cannot use Mocker.Current outside of a With.Mocks block");
                return current;
            }
            internal set { current = value; }
        }
    }
}
#endif