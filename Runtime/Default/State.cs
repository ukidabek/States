using System;
using System.Collections.Generic;
using System.Linq;
using States.Core;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Utilities.General;

namespace States.Default
{
    [AddComponentMenu("States/Core/State")]
    [DisallowMultipleComponent]
	public class State : MonoBehaviour, IState, IStateMachine, IReferenceBaker
    {
        [SerializeField] private bool m_isStatic = false;
        public bool IsStatic => m_isStatic;
        
        [FormerlySerializedAs("m_clearCurrentStateOnExit")] [SerializeField] private bool m_resetStateMachine = false;
        [SerializeField, Range(0, 100)] private int m_defaultStateIndex = 0;
        
        [SerializeField, HideInInspector] private List<State> m_backedStates = new List<State>(30);
        public IList<State> BackedStates => m_backedStates;
        [FormerlySerializedAs("m_id")] [SerializeField] private StateID m_stateID;
		public IID StateID => m_stateID;

        [SerializeReference, ReferenceList] private IStateLogic[] m_logic = null;
        public IEnumerable<IContextDestination> ContextDestinations => m_logic;

        [SerializeReference, ReferenceList] private IStateTransition[] m_transition = null;
        public IEnumerable<IStateTransition> Transitions => m_transition;

        public bool CanExit => m_logic.All(_logic => _logic.CanBeDeactivated);

        public event Action<IState> OnStateChanged
        {
            add => m_stateMachine.OnStateChanged += value;
            remove => m_stateMachine.OnStateChanged -= value;
        }
        
        public IBlackboard Blackboard => m_stateMachine.Blackboard;
        public string Name => gameObject.name;
        
        public IState CurrentState => m_stateMachine.CurrentState;
        
        public IState PreviousState => m_stateMachine.PreviousState;


        private readonly List<IOnUpdateLogic> m_onUpdateLogic = new List<IOnUpdateLogic>(10);
        private readonly List<IOnFixedUpdateLogic> m_onFixedUpdateLogic = new List<IOnFixedUpdateLogic>(10);
        private readonly List<IOnLateUpdateLogic> m_onLateUpdateLogic = new List<IOnLateUpdateLogic>(10);

        [SerializeField] private List<State> m_subStates = new List<State>();
        public IEnumerable<State> StateToBake => m_subStates;
        
        private StateMachine m_stateMachine = null;

        private ProfilerMarker m_enterState, m_exitState,
            m_updateMarker, m_fixedUpdateMarker, m_lateUpdateMarker;
        
        public void Initialize(IEnumerable<Context> contexts,
            IBlackboard blackboard,
            IEnumerable<IStatePreProcessor> statePreProcessor = null,
            IEnumerable<IStatePostProcessor> statePostProcessor = null)
        {
            m_enterState = new ProfilerMarker($"{name} - {nameof(Enter)}");
            m_exitState = new ProfilerMarker($"{name} - {nameof(Exit)}");
            m_updateMarker = new ProfilerMarker($"{name} - {nameof(OnUpdate)}");
            m_fixedUpdateMarker = new ProfilerMarker($"{name} - {nameof(OnFixedUpdate)}");
            m_lateUpdateMarker = new ProfilerMarker($"{name} - {nameof(OnLateUpdate)}");
            
            if (!m_subStates.Any()) return;
            
            foreach (var subState in m_subStates) 
                subState.Initialize(contexts, blackboard, statePreProcessor, statePostProcessor);

            var contextHandler = new ContextHandler(BackedStates);
            m_stateMachine = new StateMachine(name, contexts, contextHandler, blackboard, statePreProcessor, statePostProcessor);
        }
        
        public void EnterState(IState statToEnter) => m_stateMachine.EnterState(statToEnter);
        
		public void Enter()
        {
            m_enterState.Auto();
            m_logic.FillList(m_onUpdateLogic);
            m_logic.FillList(m_onFixedUpdateLogic);
            m_logic.FillList(m_onLateUpdateLogic);

            foreach (var stateLogic in m_logic)
            {
                Profiler.BeginSample($"Activating: {stateLogic.GetType().Name}");
                stateLogic.Activate();
                Profiler.EndSample();
            }
            
            if (!m_subStates.Any()) return;
            
            m_stateMachine.EnterState(m_subStates[m_defaultStateIndex]);
        }

        public void Exit()
        {
            m_exitState.Auto();
            foreach (var stateLogic in m_logic)
            {
                Profiler.BeginSample($"Deactivate: {stateLogic.GetType().Name}");
                stateLogic.Deactivate();
                Profiler.EndSample();
            }

            if (!m_subStates.Any()) return;
            if (!m_resetStateMachine) return;
            
            m_stateMachine.Reset();
        }

        public void OnUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            m_updateMarker.Auto();
            foreach (var update in m_onUpdateLogic) 
                update.OnUpdate(deltaTime, timeScale, blackboard);
            OnUpdate(deltaTime, timeScale);
        }
        
        public void OnUpdate(float deltaTime, float timeScale) => m_stateMachine?.OnUpdate(deltaTime, timeScale);

        public void OnFixedUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            m_fixedUpdateMarker.Auto();
            foreach (var update in m_onFixedUpdateLogic) 
                update.OnFixedUpdate(deltaTime, timeScale, blackboard);
            OnFixedUpdate(deltaTime, timeScale);
        }
        
        public void OnFixedUpdate(float deltaTime, float timeScale) => m_stateMachine?.OnFixedUpdate(deltaTime, timeScale);
        
        public void OnLateUpdate(float deltaTime, float timeScale, IBlackboard blackboard)
        {
            m_lateUpdateMarker.Auto();
            foreach (var update in m_onLateUpdateLogic) 
                update.OnLateUpdate(deltaTime, timeScale, blackboard);
            OnLateUpdate(deltaTime, timeScale);
        }
        
        public void OnLateUpdate(float deltaTime, float timeScale) => m_stateMachine?.OnLateUpdate(deltaTime, timeScale);
    }
}