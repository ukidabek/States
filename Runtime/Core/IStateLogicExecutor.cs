namespace States.Core
{
    public interface IStateLogicExecutor
    {
        bool Enabled { get; set; }
        
        public void ProvideStateMachine(IStateMachine stateMachine);
	}
}