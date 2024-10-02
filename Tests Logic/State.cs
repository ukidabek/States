using System.Collections.Generic;

namespace Utilities.States.Test
{
	public class State : IState
	{
		public IStateID ID { get; private set; }

		public IEnumerable<IStateLogic> Logic { get; private set; }

		public bool CanExit { get; set; } = true;

		public string Name { get; private set; } = string.Empty;

		public bool IsStatic { get; set; }

		public State(string name, IStateID iD, IEnumerable<IStateLogic> logic)
			: this(iD, logic)
		{
			Name = name;
		}

		public State(IStateID iD, IEnumerable<IStateLogic> logic)
		{
			ID = iD;
			Logic = logic;
		}

		public void Enter() { }

		public void Exit() { }
	}
}