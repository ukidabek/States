namespace Utilities.States.Test
{
	public class StateLogicExecutor : IStateLogicExecutor
	{
		public bool Enabled { get; set; }

		public void RemoveLogicToExecute(IState state)
		{
		}

		public void SetLogicToExecute(IState state)
		{
		}
	}
}