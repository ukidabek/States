using System;
using System.Collections.Generic;
using System.Linq;
using Default;
using UnityEngine;

namespace States.Core.Default
{
	[AddComponentMenu("States/Core/StateMachineHost")]
	public class StateMachineHost : MonoBehaviour, IStateMachine
	{	
		[SerializeField] private StateMachineContext[] m_externalContext = null;
		[SerializeField] private List<State> m_states = new List<State>();
		[SerializeField] private Executor m_executors = 0;
		public Executor Executor => m_executors;

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

			m_stateMachine = new StateMachine(
				name, 
				context, 
				preProcessors, 
				postProcessors);

			foreach (var state in m_states) 
				state.SetContext(context);
		}

		private void OnEnable() => EnterState(m_states.First());

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
		
		public void OnUpdate(float deltaTime, float timeScale) => m_stateMachine.OnUpdate(deltaTime, timeScale);

		public void OnFixedUpdate(float deltaTime, float timeScale) => m_stateMachine.OnFixedUpdate(deltaTime, timeScale);

		public void OnLateUpdate(float deltaTime, float timeScale) => m_stateMachine.OnLateUpdate(deltaTime, timeScale);
	}
}
