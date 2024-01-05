namespace Utilities.States
{
    public interface ISwitchStateCondition : IContextDestination
    {
        bool Condition { get; }
    }
}