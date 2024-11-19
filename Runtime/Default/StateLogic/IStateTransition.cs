namespace Utilities.States
{
    public interface IStateTransition : IContextDestination
    {
        IState StateToEnter { get; }
        bool Validate();
    }
}