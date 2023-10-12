using NUnit.Framework;
using System;
using AssertionException = UnityEngine.Assertions.AssertionException;

namespace Utilities.States.Test
{
	public class StateSetterTests
	{
		[Test]
		public void Validate_If_Exception_Is_Throwed_By_State_Setter()
		{
			var stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), Array.Empty<Context>());
			var state = new State(new StateID(), Array.Empty<IStateLogic>());
			Assert.Throws<AssertionException>(() => new StateSetter(null, state).SetState());
			Assert.Throws<AssertionException>(() => new StateSetter(stateMachine, null).SetState());
		}

		[Test]
		public void Validate_If_StateSetter_Set_Correct_State()
		{
			var stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), Array.Empty<Context>());
			var state = new State(new StateID(), Array.Empty<IStateLogic>());
			new StateSetter(stateMachine, state).SetState();

			Assert.AreEqual(state, stateMachine.CurrentState);
		}
	}
}