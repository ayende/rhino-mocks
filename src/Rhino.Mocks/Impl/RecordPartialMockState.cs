using System;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// Records all the expectations for a mock and
    /// return a ReplayPartialMockState when Replay()
    /// is called.
    /// </summary>
    public class RecordPartialMockState : RecordMockState
    {
        /// <summary>
        /// Creates a new <see cref="RecordDynamicMockState"/> instance.
        /// </summary>
        /// <param name="repository">Repository.</param>
        /// <param name="mockedObject">The proxy that generates the method calls</param>
        public RecordPartialMockState(IMockedObject mockedObject, MockRepository repository)
            : base(mockedObject, repository)
        {
        }

        /// <summary>
        /// Verify that we can move to replay state and move 
        /// to the reply state.
        /// </summary>
        protected override IMockState DoReplay()
        {
            return new ReplayPartialMockState(this);
        }


        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public override IMockState BackToRecord()
        {
            return new RecordPartialMockState(this.Proxy, Repository);
        }
    }
}
