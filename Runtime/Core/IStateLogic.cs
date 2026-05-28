using States.Core;

namespace States.Default
{
    public interface IStateLogic : IContextDestination
    {
        bool CanBeDeactivated { get; }
		void Activate(IBlackboard blackboard);
        void Deactivate(IBlackboard blackboard);
    }
}