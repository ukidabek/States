using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States.Default
{
	[AddComponentMenu("States/Core/StateMachineHost")]
	public class StateMachineHost : MonoBehaviour, IStateMachine
	{
		[SerializeField] private ExternalContext[] m_externalContext = null;
		[SerializeField] private List<Context> m_context = new List<Context>();
		[SerializeField] private Executor m_executors = 0;
		public Executor Executor => m_executors;

		[SerializeField] private State m_currentState = null;

		[Space]
		[SerializeField] private StateSetter m_defaultStateSetter = null;

		public string Name => name;

		public IState CurrentState => m_currentState;

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
			var transitions = GetComponents<IStateTransition>();
			var preProcessors = GetComponents<IStatePreProcessor>();
			var postProcessors = GetComponents<IStatePostProcessor>();
			var externalContext = m_externalContext.SelectMany(_context => _context.Context);
			var context = m_context.Concat(externalContext).ToArray();

			m_stateMachine = new StateMachine(
				name, 
				executors, 
				transitions,
				context, 
				preProcessors, 
				postProcessors);
			m_stateMachine.OnStateChanged += OnStateChange;
		}

		private void OnEnable() => m_defaultStateSetter?.SetState();

		private void OnDestroy() => m_stateMachine.OnStateChanged -= OnStateChange;

		private void OnStateChange(IState state)
		{
			if (state is State monoState)
				m_currentState = monoState;
			else
			{
#if UNITY_EDITOR
				Debug.LogWarning($"Implementation of {nameof(IState)} is not {typeof(State).FullName}");
#endif
				m_currentState = null;
			}
		}

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
	}
}
