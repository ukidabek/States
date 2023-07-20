namespace Utilities.States
{
    public interface IStateTransitionLogic
    {
        void Cancel();
        void Perform(IState from, IState to);
    }
}