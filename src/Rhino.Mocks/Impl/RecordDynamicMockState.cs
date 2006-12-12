using System;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Records all the expectations for a mock and
	/// return a ReplayDynamicMockState when Replay()
	/// is called.
	/// </summary>
	public class RecordDynamicMockState : RecordMockState
	{
		/// <summary>
		/// Creates a new <see cref="RecordDynamicMockState"/> instance.
		/// </summary>
		/// <param name="repository">Repository.</param>
		/// <param name="mockedObject">The proxy that generates the method calls</param>
		public RecordDynamicMockState(IMockedObject mockedObject, MockRepository repository) : base(mockedObject, repository)
		{
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected override IMockState DoReplay()
		{
			return new ReplayDynamicMockState(this);
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public override IMockState BackToRecord()
        {
            return new RecordDynamicMockState(this.Proxy, Repository);
        }
	}
}
