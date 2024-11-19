using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.Profiling;

namespace Utilities.States
{
    public class StateMachine : IStateMachine, IDisposable
    {
        private ContextHandler m_contextHandler = new ContextHandler();
        public event Action<IState> OnStateChanged;
        
        private readonly IEnumerable<Context> m_context = null;
        private readonly IEnumerable<IStatePreProcessor> m_statePreProcessors = null;
        private readonly IEnumerable<IStatePostProcessor> m_statePostProcessors = null;

        public IState CurrentState { get; private set; }
        public string Name { get; private set; }
        public IState PreviousState { get; private set; }

        private ProfilerMarker m_updateMarker, m_fixedUpdateMarker, m_lateUpdateMarker;
        
        public StateMachine(
            IEnumerable<Context> context,
            IEnumerable<IStatePreProcessor> statePreProcessor = null,
            IEnumerable<IStatePostProcessor> statePostProcessor = null)
            : this(nameof(StateMachine), context, statePreProcessor,
                statePostProcessor)
        {
        }

        public StateMachine(
            string name,
            IEnumerable<Context> context,
            IEnumerable<IStatePreProcessor> statePreProcessor = null,
            IEnumerable<IStatePostProcessor> statePostProcessor = null)
        {
            Name = name;
            m_context = context;
            m_statePreProcessors = statePreProcessor;
            m_statePostProcessors = statePostProcessor;

            m_updateMarker = new ProfilerMarker($"{name} - {nameof(OnUpdate)}");
            m_fixedUpdateMarker = new ProfilerMarker($"{name} - {nameof(OnFixedUpdate)}");
            m_lateUpdateMarker = new ProfilerMarker($"{name} - {nameof(OnLateUpdate)}");
        }

        public void EnterState(IState statToEnter)
        {
            var currentStateNotNull = CurrentState != null;

			if (currentStateNotNull && !CurrentState.CanExit) return;

			if (CurrentState == statToEnter) return;

            PreviousState = CurrentState;

            if (currentStateNotNull)
            {
                CurrentState.Exit();
                m_statePostProcessors.Process(CurrentState);
            }

            CurrentState = statToEnter;

            m_contextHandler.FillState(CurrentState, m_context);

            m_statePreProcessors.Process(CurrentState);
            CurrentState?.Enter();
            OnStateChanged?.Invoke(CurrentState);
        }

        public void OnUpdate(float deltaTime, float timeScale)
        {
            m_updateMarker.Auto();
            if (CurrentState == null) return;
            
            var transitions = CurrentState.Transitions;
            if (transitions != null && transitions.Any())
            {
                var validTransition = transitions.FirstOrDefault(transitions => transitions.Validate());
                if (validTransition != null)
                    EnterState(validTransition.StateToEnter);
            }

            CurrentState.OnUpdate(deltaTime, timeScale);
        }

        public void OnFixedUpdate(float deltaTime, float timeScale)
        {
            m_fixedUpdateMarker.Auto();
            CurrentState?.OnFixedUpdate(deltaTime, timeScale);
        }

        public void OnLateUpdate(float deltaTime, float timeScale)
        {
            m_lateUpdateMarker.Auto();
            CurrentState?.OnLateUpdate(deltaTime, timeScale);
        }

        public void Dispose()
        {
            OnStateChanged = null;
        }
    }
}