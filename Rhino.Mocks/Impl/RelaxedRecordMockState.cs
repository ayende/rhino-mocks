using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// This mock state uses relaxed semantics when it is running, allowing
    /// to move to replay state even if not all the actions on the expectations
    /// are satisified
    /// </summary>
    public class RelaxedRecordMockState : RecordDynamicMockState
    {
        public RelaxedRecordMockState(IMockedObject mockedObject, MockRepository repository)
            : base(mockedObject, repository)
        {
        }

        protected override void AssertPreviousMethodIsClose()
        {
        }

        public override IMockState Replay()
        {
            return new RelaxedReplayMockState(this);
        }
    }

    /// <summary>
    /// Replay state for relax mocks, mainly concerned with returning to the previous mode.
    /// </summary>
    public class RelaxedReplayMockState : ReplayDynamicMockState
    {
        public RelaxedReplayMockState(RelaxedRecordMockState previousState)
            : base(previousState)
        {
        }

        public override IMockState BackToRecord()
        {
            return new RelaxedRecordMockState(proxy, repository);
        }
    }
}