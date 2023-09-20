using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States
{
	[AddComponentMenu("States/Core/StateMachineManager")]
    public class StateMachineManager : MonoBehaviour, IStateMachine
    {
		[SerializeField] private Executor m_executors = 0;
		public Executor Executor => m_executors;

		[SerializeField] private State m_currentState = null;
        [SerializeField] private Object[] m_stateTransition = null;
		[SerializeField] private Object[] m_stateProcessors = null;
		[Space]
		[SerializeField] private StateSetter m_defaultStateSetter = null;

		public string Name => name;

        public IState CurrentState => m_currentState;

		public IState PreviousState => m_stateMachine.PreviousState;

        protected StateMachine m_stateMachine = null;

		private void Awake()
		{
			var executors = GetComponents<IStateLogicExecutor>();
			var transitions = m_stateTransition.OfType<IStateTransitionLogic>();
			var preProcessors = m_stateProcessors.OfType<IStatePreProcessor>();
			var postProcessors = m_stateProcessors.OfType<IStatePostProcessor>();

			m_stateMachine = new StateMachine(name, executors, transitions, preProcessors, postProcessors);
			m_stateMachine.OnStateChange += OnStateChange;
		}

		private void OnEnable() => m_defaultStateSetter?.SetState();

		private void OnDestroy() => m_stateMachine.OnStateChange -= OnStateChange;

		private void OnStateChange() => m_currentState = m_stateMachine.CurrentState as State;

		public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
	}
}
