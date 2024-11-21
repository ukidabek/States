using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States.Core.Default
{
    [AddComponentMenu("States/Core/State")]
	public class State : MonoBehaviour, IState
    {
        [SerializeField] private StateID m_stateID;
		public IStateID ID => m_stateID;

        [SerializeReference] private IStateLogic[] m_logic = null;
        public IEnumerable<IStateLogic> Logic => m_logic;

        [SerializeReference] private IStateTransition[] m_transition = null;
        public IEnumerable<IStateTransition> Transitions => m_transition;

        public bool CanExit => Logic.All(_logic => _logic.CanBeDeactivated);

		public string Name => gameObject.name;

        [SerializeField] private bool m_isStatic = false;
        public bool IsStatic => m_isStatic;

        private List<IOnUpdateLogic> m_onUpdateLogic = new List<IOnUpdateLogic>(10);
        private List<IOnFixedUpdateLogic> m_onFixedUpdateLogic = new List<IOnFixedUpdateLogic>(10);
        private List<IOnLateUpdateLogic> m_onLateUpdateLogic = new List<IOnLateUpdateLogic>(10);

        [SerializeField] private List<State> m_subStates = new List<State>();
        
        private StateMachine m_stateMachine = null;
        public void SetContext(IEnumerable<Context> contexts)
        {
            if (!m_subStates.Any()) return;
            
            foreach (var subState in m_subStates) 
                subState.SetContext(contexts);
            
            m_stateMachine = new StateMachine(name, contexts);
        }
        
		public void Enter()
        {
            m_logic.FillList(m_onUpdateLogic);
            m_logic.FillList(m_onFixedUpdateLogic);
            m_logic.FillList(m_onLateUpdateLogic);
            
            foreach (var stateLogic in Logic) 
                stateLogic.Activate();
            
            if (!m_subStates.Any()) return;
            m_stateMachine.EnterState(m_subStates.First());
        }

        public void Exit()
        {
            foreach (var stateLogic in Logic) 
                stateLogic.Deactivate();
        }

        public void OnUpdate(float deltaTime, float timeScale)
        {
            foreach (var update in m_onUpdateLogic) 
                update.OnUpdate(deltaTime, timeScale);
            
            m_stateMachine?.OnUpdate(deltaTime, timeScale);
        }

        public void OnFixedUpdate(float deltaTime, float timeScale)
        {
            foreach (var update in m_onFixedUpdateLogic) 
                update.OnFixexUpdate(deltaTime, timeScale);
            
            m_stateMachine?.OnFixedUpdate(deltaTime, timeScale);
        }
        
        public void OnLateUpdate(float deltaTime, float timeScale)
        {
            foreach (var update in m_onLateUpdateLogic) 
                update.OnLateUpdate(deltaTime, timeScale);
            
            m_stateMachine?.OnLateUpdate(deltaTime, timeScale);
        }
    }
}