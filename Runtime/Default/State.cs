using System.Collections.Generic;
using System.Linq;
using States.Core;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.General;

namespace States.Default
{
    [AddComponentMenu("States/Core/State")]
	public class State : MonoBehaviour, IState
    {
        [FormerlySerializedAs("m_id")] [SerializeField] private StateID m_stateID;
		public IID StateID => m_stateID;

        [SerializeReference, ReferenceList] private IStateLogic[] m_logic = null;
        public IEnumerable<IContextDestination> ContextDestinations => m_logic;

        [SerializeReference, ReferenceList] private IStateTransition[] m_transition = null;
        public IEnumerable<IStateTransition> Transitions => m_transition;

        public bool CanExit => m_logic.All(_logic => _logic.CanBeDeactivated);

		public string Name => gameObject.name;

        [SerializeField] private bool m_isStatic = false;
        public bool IsStatic => m_isStatic;

        private List<IOnUpdateLogic> m_onUpdateLogic = new List<IOnUpdateLogic>(10);
        private List<IOnFixedUpdateLogic> m_onFixedUpdateLogic = new List<IOnFixedUpdateLogic>(10);
        private List<IOnLateUpdateLogic> m_onLateUpdateLogic = new List<IOnLateUpdateLogic>(10);

        [SerializeField] private List<State> m_subStates = new List<State>();
        
        private StateMachine m_stateMachine = null;
        
        public void Initialize(IEnumerable<Context> contexts,
            IBlackboard blackboard,
            IEnumerable<IStatePreProcessor> statePreProcessor = null,
            IEnumerable<IStatePostProcessor> statePostProcessor = null)
        {
            if (!m_subStates.Any()) return;
            
            foreach (var subState in m_subStates) 
                subState.Initialize(contexts, blackboard, statePreProcessor, statePostProcessor);
            
            m_stateMachine = new StateMachine(name, contexts, blackboard, statePreProcessor, statePostProcessor);
        }
        
		public void Enter()
        {
            m_logic.FillList(m_onUpdateLogic);
            m_logic.FillList(m_onFixedUpdateLogic);
            m_logic.FillList(m_onLateUpdateLogic);
            
            foreach (var stateLogic in m_logic) 
                stateLogic.Activate();
            
            if (!m_subStates.Any()) return;
            
            m_stateMachine.EnterState(m_subStates.First());
        }

        public void Exit()
        {
            foreach (var stateLogic in m_logic) 
                stateLogic.Deactivate();
        }

        public void OnUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            foreach (var update in m_onUpdateLogic) 
                update.OnUpdate(deltaTime, timeScale, blackboard);
            
            m_stateMachine?.OnUpdate(deltaTime, timeScale);
        }

        public void OnFixedUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            foreach (var update in m_onFixedUpdateLogic) 
                update.OnFixedUpdate(deltaTime, timeScale, blackboard);
            
            m_stateMachine?.OnFixedUpdate(deltaTime, timeScale);
        }
        
        public void OnLateUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            foreach (var update in m_onLateUpdateLogic) 
                update.OnLateUpdate(deltaTime, timeScale, blackboard);
            
            m_stateMachine?.OnLateUpdate(deltaTime, timeScale);
        }
    }
}