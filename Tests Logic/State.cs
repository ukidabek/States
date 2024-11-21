using System.Collections.Generic;
using System.Linq;

namespace States.Core.Test
{
	public class State : IState
	{
		public IID StateID { get; }
		
		public IEnumerable<IContextDestination> ContextDestinations { get; } = Enumerable.Empty<IContextDestination>();
		
		public IEnumerable<IStateTransition> Transitions { get; } = Enumerable.Empty<IStateTransition>();

		public bool CanExit { get; set; } = true;

		public string Name { get; } = string.Empty;

		public bool IsStatic { get; set; }

		public Blackboard Blackboard { get; private set; }
		
		public State(
			string name, 
			IID iD, 
			IEnumerable<IContextDestination> contextDestinations, 
			IEnumerable<IStateTransition> transitions) 
		{
			StateID = iD;
			Name = name;
			ContextDestinations = contextDestinations;
			Transitions = transitions;
		}

		public State(IID iD, IEnumerable<IStateTransition> transitions) 
			: this(
				string.Empty, 
				iD, 
				Enumerable.Empty<IContextDestination>(),
				transitions)
		{
		}

		public float UpdateCount {get; private set;}
		
		public float FixUpdateCount {get; private set;}
		
		public float LateUpdateCount {get; private set;}
		
		public void Enter() { }

		public void Exit() { }

		public void OnUpdate(float deltaTime, float timeScale, Blackboard blackboard)
		{
			Blackboard = blackboard;
			UpdateCount++;
		}

		public void OnFixedUpdate(float deltaTime, float timeScale, Blackboard blackboard)
		{
			Blackboard = blackboard;
			FixUpdateCount++;
		}

		public void OnLateUpdate(float deltaTime, float timeScale, Blackboard blackboard)
		{
			Blackboard = blackboard;
			LateUpdateCount++;
		}
	}
}