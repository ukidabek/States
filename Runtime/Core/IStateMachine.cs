using System;

namespace Utilities.States
{
    public interface IStateMachine
    {
        event Action<IState> OnStateChanged;
        string Name { get; }
        IState CurrentState { get; }
        IState PreviousState { get; }
        void EnterState(IState statToEnter);
        public void OnUpdate(float deltaTime, float timeScale);
        public void OnFixedUpdate(float deltaTime, float timeScale);
        public void OnLateUpdate(float deltaTime, float timeScale);
    }
}