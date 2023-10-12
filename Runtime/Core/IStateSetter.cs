namespace Utilities.States
{
	public interface IStateSetter
	{
		IStateMachine StateMachine { get; set; }
		void SetState();
	}
}