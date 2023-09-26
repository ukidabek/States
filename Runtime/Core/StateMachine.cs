﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utilities.States
{
	public class StateMachine : IStateMachine, IDisposable
    {
		private const BindingFlags Binding_Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public event Action<IState> OnStateChanged;

		private readonly IEnumerable<IStateLogicExecutor> m_stateLogicExecutor = null;
        private readonly IEnumerable<IStateTransition> m_transitions = null;
		private readonly IEnumerable<Context> m_context = null;
		private readonly IEnumerable<IStatePreProcessor> m_statePreProcessors = null;
		private readonly IEnumerable<IStatePostProcessor> m_statePostProcessors = null;

		public IState CurrentState { get; private set; }
		public string Name { get; private set; }
		public IState PreviousState { get; private set; }

		public StateMachine(
			IEnumerable<IStateLogicExecutor> stateLogicExecutor, 
			IEnumerable<IStateTransition> transitions,
			IEnumerable<Context> context,
            IEnumerable<IStatePreProcessor> statePreProcessor = null, 
			IEnumerable<IStatePostProcessor> statePostProcessor = null)
            : this(nameof(StateMachine), stateLogicExecutor, transitions, context, statePreProcessor, statePostProcessor)
		{
		}

		public StateMachine(
			string name,
			IEnumerable<IStateLogicExecutor> stateLogicExecutor,
			IEnumerable<IStateTransition> transitions,
			IEnumerable<Context> context,
			IEnumerable<IStatePreProcessor> statePreProcessor,
			IEnumerable<IStatePostProcessor> statePostProcessor)
		{
			Name = name;
			m_stateLogicExecutor = stateLogicExecutor;
			m_transitions = transitions;
			m_context = context;
			m_statePreProcessors = statePreProcessor;
			m_statePostProcessors = statePostProcessor;
		}

        public void EnterState(IState statToEnter)
		{
			if (CurrentState == statToEnter) return;

			PreviousState = CurrentState;

			foreach (var transition in m_transitions)
			{
				transition.Cancel();
				transition.Perform(CurrentState, statToEnter);
			}

			if (CurrentState != null)
			{
				RemoveStateLogic();
				CurrentState.Exit();
				m_statePostProcessors.Process(CurrentState);
			}

			CurrentState = statToEnter;

			FillState(CurrentState, m_context);

			m_statePreProcessors.Process(CurrentState);
			CurrentState?.Enter();
			OnStateChanged?.Invoke(CurrentState);
			
			SetStateLogic();
		}

		private void FillState(IState state, IEnumerable<Context> contexts)
		{
			var stateLogic = state.Logic;

			foreach (var logic in stateLogic)
			{
				var logicType = logic.GetType();
				var fields = logicType.GetFields(Binding_Flags);

				foreach (var field in fields)
				{
					var attribute = field.GetCustomAttribute<ContextField>();
					if (attribute == null) continue;

					Context context = default;
					var attributeID = attribute.ID;
					if (string.IsNullOrEmpty(attributeID))
					{
						context = contexts.FirstOrDefault(context => context.Type == field.FieldType);
						if (context == null)
							continue;
					}
					else
					{
						context = contexts.FirstOrDefault(context => context.Type == field.FieldType && context.Id == attributeID);
						if (context == null)
							continue;
					}

					field.SetValue(logic, context.Object);
				}
			}
		}

		public void SetStateLogic()
		{
			foreach (var stateLogicExecutor in m_stateLogicExecutor)
				stateLogicExecutor.SetLogicToExecute(CurrentState);
		}
		
		public void RemoveStateLogic()
		{
			foreach (var stateLogicExecutor in m_stateLogicExecutor)
				stateLogicExecutor.RemoveLogicToExecute(CurrentState);
		}

		public void Dispose()
		{
			OnStateChanged = null;
		}
	}
}