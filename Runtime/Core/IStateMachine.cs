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
    }
}