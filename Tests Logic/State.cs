using System.Collections.Generic;
using System.Linq;

namespace States.Core.Test
{
	public class State : IState
	{
		public IStateID ID { get; private set; }

		public IEnumerable<IStateLogic> Logic { get; private set; }= Enumerable.Empty<IStateLogic>();
		public IEnumerable<IStateTransition> Transitions { get; private set; } = Enumerable.Empty<IStateTransition>();

		public bool CanExit { get; set; } = true;

		public string Name { get; private set; } = string.Empty;

		public bool IsStatic { get; set; }

		public State(string name, IStateID iD, IEnumerable<IStateLogic> logic, IEnumerable<IStateTransition> transitions)
			: this(iD, logic, transitions)
		{
			Name = name;
		}

		public State(IStateID iD, IEnumerable<IStateLogic> logic, IEnumerable<IStateTransition> transitions)
		{
			ID = iD;
			Logic = logic;
			Transitions = transitions;
		}

		public void Enter() { }

		public void Exit() { }
		
		public void OnUpdate(float deltaTime, float timeScale)
		{
			foreach (var logic in Logic.OfType<IOnUpdateLogic>())
				logic.OnUpdate(deltaTime, timeScale);
		}

		public void OnFixedUpdate(float deltaTime, float timeScale)
		{
			foreach (var logic in Logic.OfType<IOnFixedUpdateLogic>())
				logic.OnFixexUpdate(deltaTime, timeScale);
		}

		public void OnLateUpdate(float deltaTime, float timeScale)
		{
			foreach (var logic in Logic.OfType<IOnLateUpdateLogic>())
				logic.OnLateUpdate(deltaTime, timeScale);
		}
	}
}