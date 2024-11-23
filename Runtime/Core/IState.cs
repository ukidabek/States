using System.Collections.Generic;

namespace States.Core
{
    public interface IState
    {
        public bool IsStatic { get; }
        string Name { get; }
        IID StateID { get; }
        IEnumerable<IContextDestination> ContextDestinations { get; }
        IEnumerable<IStateTransition> Transitions { get; }
        void Enter();
        bool CanExit { get; }
        void Exit();
        void OnUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
        void OnFixedUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
        void OnLateUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
    }
}