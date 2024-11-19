namespace Utilities.States
{
    public interface IStateLogicExecutor
    {
        bool Enabled { get; set; }
        
        public void ProvideStateMachine(IStateMachine stateMachine);
	}
}