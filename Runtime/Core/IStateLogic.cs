using States.Core;

namespace States.Default
{
    public interface IStateLogic : IContextDestination
    {
        bool CanBeDeactivated { get; }
		void Activate();
        void Deactivate();
    }
}