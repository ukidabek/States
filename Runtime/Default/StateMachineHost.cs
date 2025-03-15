using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using States.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace States.Default
{
	[AddComponentMenu("States/Core/StateMachineHost")]
	public class StateMachineHost : MonoBehaviour, IStateMachine, IReferenceBaker
	{	
		[SerializeField] private BlackboardFactory m_blackboardFactory;
		[SerializeField] private StateMachineContext[] m_externalContext = null;
		[SerializeField] private List<State> m_states = new List<State>(30);
		[SerializeField, HideInInspector] private List<State> m_backedStates = new List<State>(30);
		[SerializeField] private Executor m_executors = 0;
		public Executor Executor => m_executors;

		public IBlackboard Blackboard => m_stateMachine.Blackboard;
		
		public string Name => name;
		
		public IState CurrentState => m_stateMachine.CurrentState;
		
		public IState PreviousState => m_stateMachine.PreviousState;

		protected StateMachine m_stateMachine = null;

		public event Action<IState> OnStateChanged
		{
			add => m_stateMachine.OnStateChanged += value;
			remove => m_stateMachine.OnStateChanged -= value;
		}

		private void Awake()
		{
			var executors = GetComponents<IStateLogicExecutor>();
			
			foreach (var executor in executors)
				executor.ProvideStateMachine(this);

			var preProcessors = GetComponents<IStatePreProcessor>();
			var postProcessors = GetComponents<IStatePostProcessor>();
			var context = m_externalContext.SelectMany(_context => _context.Context).ToArray();
			var contextHandler = new ContextHandler(m_backedStates);
			var blackboard = m_blackboardFactory.CreateBlackboard();
			m_stateMachine = new StateMachine(name, context, contextHandler, blackboard, preProcessors, postProcessors);

			foreach (var state in m_states)
				state.Initialize(context, blackboard, preProcessors, postProcessors);
		}

		private void OnEnable() => EnterState(m_states.First());

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
		
		public void OnUpdate(float deltaTime, float timeScale) => m_stateMachine.OnUpdate(deltaTime, timeScale);

		public void OnFixedUpdate(float deltaTime, float timeScale) => m_stateMachine.OnFixedUpdate(deltaTime, timeScale);

		public void OnLateUpdate(float deltaTime, float timeScale) => m_stateMachine.OnLateUpdate(deltaTime, timeScale);

		[Conditional("UNITY_EDITOR")]
		public void BakeReferences()
		{
			var context = m_externalContext.SelectMany(_context => _context.Context);
			(this as IReferenceBaker).Bake(context);
			foreach (var baker in StateToBake.OfType<IReferenceBaker>())
				baker.Bake(context);
		}

		public IList<State> BackedStates => m_backedStates;

		public IEnumerable<State> StateToBake => m_states;
	}
}
