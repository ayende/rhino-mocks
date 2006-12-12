using System;
using System.Text;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// Raise events for all subscribers for an event
    /// </summary>
    public class EventRaiser : IEventRaiser
    {
        string eventName;
        IMockedObject proxy;

        /// <summary>
        /// Creates a new instance of <c>EventRaiser</c>
        /// </summary>
        public EventRaiser(IMockedObject proxy, string eventName)
        {
            this.eventName = eventName;
            this.proxy = proxy;
        }

        #region IEventRaiser Members

        /// <summary>
        /// Raise the event
        /// </summary>
        public void Raise(params object[] args)
        {
            Delegate subscribed = proxy.GetEventSubscribers(eventName);
            if(subscribed!=null)
                subscribed.DynamicInvoke(args);
        }

        #endregion
    }
}
