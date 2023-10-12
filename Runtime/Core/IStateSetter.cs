namespace Utilities.States
{
	public interface IStateSetter
	{
		IState State { get; set; }
		IStateMachine StateMachine { get; set; }
		void SetState();
	}
}