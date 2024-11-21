namespace States.Core
{
    public interface IStateLogic : IContextDestination
    {
        bool CanBeDeactivated { get; }
		void Activate();
        void Deactivate();
    }
}