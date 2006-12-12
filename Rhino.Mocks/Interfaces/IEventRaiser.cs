using System;

namespace Rhino.Mocks.Interfaces
{
    /// <summary>
    /// Raise events for all subscribers for an event
    /// </summary>
    public interface IEventRaiser
    {
        /// <summary>
        /// Raise the event
        /// </summary>
        void Raise(params object[] args);
    }
}