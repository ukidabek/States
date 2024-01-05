using System;
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

		private bool ValidateType(Context context, MemberInfo member)
		{
			var contextType = context.Type;
			Type memberType = default;

			switch (member)
			{
				case FieldInfo fieldInfo:
					memberType = fieldInfo.FieldType;
					break;
				case PropertyInfo propertyInfo:
					memberType = propertyInfo.PropertyType;
					break;
			}

			if (memberType.IsInterface)
			{
				var interfaces = contextType.GetInterfaces();
				return interfaces.Contains(memberType);
			}

			return contextType == memberType;
		}

		private void FillState(IState state, IEnumerable<Context> contexts)
		{
			var contextDestinations = state.GetContextDestination();
			foreach (var logic in contextDestinations)
			{
				var logicType = logic.GetType();
				var members = logicType.GetMembers(Binding_Flags);

				foreach (var member in members)
				{
					var attribute = member.GetCustomAttribute<ContextField>();
					if (attribute == null) continue;

					Context context = default;
					var attributeID = attribute.ID;
					if (string.IsNullOrEmpty(attributeID))
					{
						context = contexts.FirstOrDefault(context => ValidateType(context, member));
						if (context == null)
							continue;
					}
					else
					{
						context = contexts.FirstOrDefault(context => ValidateType(context, member) && context.Id == attributeID);
						if (context == null)
							continue;
					}

					switch(member)
					{
						case FieldInfo fieldInfo:
							fieldInfo.SetValue(logic, context.Object);
							break;
						case PropertyInfo propertyInfo:
							propertyInfo.SetValue(logic, context.Object);
							break;
					}
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