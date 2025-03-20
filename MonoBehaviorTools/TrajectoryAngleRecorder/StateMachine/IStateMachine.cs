namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public interface IStateMachine
        {
            TrajectoryMode Mode { get; }
            void Init(ITrajectoryCheckmarkDetector mainDetector, StateMachineCarryOverInfo previousInfo);
            void Execute(TrajectoryAngleRecorder.AddNodeResult recordResult, out StateMachineCarryOverInfo nextStateInfo);
        }
    }
}