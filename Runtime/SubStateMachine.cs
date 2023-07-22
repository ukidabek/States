using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities.States
{
	[AddComponentMenu("States/StateLogic/SubStateMachine")]
	public class SubStateMachine : StateLogic, IStateMachine
	{
		[SerializeField] private State _currentState = null;
		[FormerlySerializedAs("m_stateLogicExecutorsObjects")]
		[SerializeField] private Object[] m_logicExecutor = null;
		[FormerlySerializedAs("m_stateLogicExecutorsObjects")]
		[SerializeField] private Object[] m_stateTransition = null;
		[SerializeField] private Object[] m_stateProcessors = null;
		[Space]
		[SerializeField] private StateSetter m_defaultStateSetter = null;

		public string Name => name;
		public IState CurrentState => _currentState;
		public IState PreviousState => m_stateMachine.PreviousState;

		private IEnumerable<IStateLogicExecutor> m_stateLogicExecutors = null;

		private StateMachine m_stateMachine;

		private void CreateStateMachine()
		{
			if (m_stateMachine == null)
			{
				m_stateLogicExecutors = m_logicExecutor.OfType<IStateLogicExecutor>();

				m_stateMachine = new StateMachine(
					$"{name}{nameof(SubStateMachine)}",
					m_stateLogicExecutors,
					m_stateTransition.OfType<IStateTransitionLogic>(),
					m_stateProcessors.OfType<IStatePreProcessor>(),
					m_stateProcessors.OfType<IStatePostProcessor>());

				m_stateMachine.OnStateChange += StateMachineOnOnStateChange;
			}

			m_defaultStateSetter?.SetState();
		}

		public override void Activate()
		{
			CreateStateMachine();
			foreach (var executor in m_stateLogicExecutors)
				executor.Enabled = true;
		}

		public override void Deactivate()
		{
			foreach (var executor in m_stateLogicExecutors)
				executor.Enabled = false;
		}

		private void StateMachineOnOnStateChange()
		{
			if (m_stateMachine.CurrentState is State state)
				_currentState = state;
		}

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);

		public bool Enabled
		{
			get => enabled;
			set => enabled = value;
		}
	}
}