using UnityEngine.Assertions;

namespace Utilities.States
{
	public class StateSetter : IStateSetter
	{
		public IStateMachine StateMachine { get; set; }
		public IState State { get; set; }

		public StateSetter() { }

		public StateSetter(IStateMachine stateMachine, IState state)
		{
			StateMachine = stateMachine;
			State = state;
		}

		public void SetState()
		{
			Assert.IsNotNull(StateMachine, $"No {nameof(IStateMachine)} selected!");
			Assert.IsNotNull(State, $"No {nameof(IState)} selected!");

			StateMachine.EnterState(State);
		}
	}
}