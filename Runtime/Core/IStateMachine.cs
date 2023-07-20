namespace Utilities.States
{
    public interface IStateMachine
    {
        string Name { get; }
        IState CurrentState { get; }
        IState PreviousState { get; }
        void EnterState(IState statToEnter);
    }
}