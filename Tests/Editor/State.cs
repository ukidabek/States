using System.Collections.Generic;
using System.Linq;

namespace Utilities.States.Test
{
	public class State : IState
	{
		public IStateID ID { get; private set; }

		public IEnumerable<IStateLogic> Logic { get; private set; }

		public bool CanExit { get; set; } = true;

		public State(IStateID iD, IEnumerable<IStateLogic> logic)
		{
			ID = iD;
			Logic = logic;
		}

		public void Enter() { }

		public void Exit() { }
	}
}