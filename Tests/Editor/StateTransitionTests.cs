using NUnit.Framework;
using System;

namespace Utilities.States.Test
{
	public class StateTransitionTests
	{
		[TestCase(TransitionMode.FromTo)]
		[TestCase(TransitionMode.To)]
		[TestCase(TransitionMode.From)]
		public void Validate_If_From_To_Transition_Will_Work(TransitionMode mode)
		{
			var stateA = new State(new StateID(), Array.Empty<IStateLogic>());
			var stateB = new State(new StateID(), Array.Empty<IStateLogic>());
			var transitionPreformed = false;
			var stateTransition = new StateTransition(stateA, stateB, mode, () => transitionPreformed = true, null);

			stateTransition.Perform(stateB, stateA);
			Assert.IsFalse(transitionPreformed);
			stateTransition.Perform(stateA, stateB);
			Assert.IsTrue(transitionPreformed);
		}
	}
}