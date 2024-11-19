namespace Utilities.States
{
    public interface IStateLogic : IContextDestination
    {
        bool CanBeDeactivated { get; }
		void Activate();
        void Deactivate();
    }
}