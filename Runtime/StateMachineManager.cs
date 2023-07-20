using System.Linq;
using UnityEngine;

namespace Utilities.States
{
    public class StateMachineManager : MonoBehaviour, IStateMachine
    {
        [SerializeField] private State m_currentState = null;
		[SerializeField] private Object[] m_logicExecutor;
        [SerializeField] private Object[] m_stateTransitions = null;
		[SerializeField] private StateSetter m_defaultStateSetter = null;
		[SerializeField] private Object[] m_stateProcessors = null;

		public string Name => name;

        public IState CurrentState => m_currentState;

		public IState PreviousState => m_stateMachine.PreviousState;

        private StateMachine m_stateMachine = null;

        private void Awake()
		{
			m_stateMachine = new StateMachine(
							name,
							m_logicExecutor.OfType<IStateLogicExecutor>(),
							m_stateTransitions.OfType<IStateTransitionLogic>(),
							m_stateProcessors.OfType<IStatePreProcessor>(),
							m_stateProcessors.OfType<IStatePostProcessor>()); 
			m_stateMachine.OnStateChange += OnStateChange;
		}

		private void OnEnable() => m_defaultStateSetter?.SetState();

		private void OnStateChange() => m_currentState = m_stateMachine.CurrentState as State;

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
	}
}
