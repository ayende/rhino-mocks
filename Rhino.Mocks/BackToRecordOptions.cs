using System;

namespace Rhino.Mocks
{
    /// <summary>
    /// What should BackToRecord clear
    /// </summary>
    [Flags]
    public enum BackToRecordOptions
    {
        /// <summary>
        /// Retain all expectations and behaviors and return to mock
        /// </summary>
        None = 0,
        /// <summary>
        /// All expectations
        /// </summary>
        Expectations = 1,
        /// <summary>
        /// Event subscribers for this instance
        /// </summary>
        EventSubscribers = 2,
        /// <summary>
        /// Methods that should be forwarded to the base class implementation
        /// </summary>
        OriginalMethodsToCall = 4,
        /// <summary>
        /// Properties that should behave like properties
        /// </summary>
        PropertyBehavior = 8,
        /// <summary>
        /// Remove all the behavior of the object
        /// </summary>
        All = Expectations | EventSubscribers | OriginalMethodsToCall | PropertyBehavior
    }
}