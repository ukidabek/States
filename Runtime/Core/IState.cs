using System.Collections.Generic;

namespace States.Core
{
    public interface IState
    {
        public bool IsStatic { get; }
        string Name { get; }
        IStateID ID { get; }
        IEnumerable<IStateLogic> Logic { get; }
        IEnumerable<IStateTransition> Transitions { get; }
        void Enter();
        bool CanExit { get; }
        void Exit();
        void OnUpdate(float deltaTime, float timeScale);
        void OnFixedUpdate(float deltaTime, float timeScale);
        void OnLateUpdate(float deltaTime, float timeScale);
    }
}