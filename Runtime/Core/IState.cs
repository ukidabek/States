using System.Collections.Generic;

namespace States.Core
{
    public interface IState : IContextDestinationHolder
    {
        public bool IsStatic { get; }
        string Name { get; }
        IID StateID { get; }
        IEnumerable<IStateTransition> Transitions { get; }
        bool CanExit { get; }
        void Enter(IBlackboard blackboard);
        void Exit(IBlackboard blackboard);
        void OnUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
        void OnFixedUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
        void OnLateUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
    }
}