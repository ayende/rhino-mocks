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
        /// Nothing
        /// </summary>
        None = 0,
        /// <summary>
        /// Event subscribers for this instance
        /// </summary>
        EventSubscribers = 1,
        /// <summary>
        /// Methods that should be forwarded to the base class implementation
        /// </summary>
        OriginalMethodsToCall = 2,
        /// <summary>
        /// Properties that should behave like properties
        /// </summary>
        PropertyBehavior = 4,
        /// <summary>
        /// Remove al the behavior of the object
        /// </summary>
        All = EventSubscribers|OriginalMethodsToCall|PropertyBehavior
    }
}