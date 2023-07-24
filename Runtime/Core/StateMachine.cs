using System;
using System.Collections.Generic;

namespace Utilities.States
{
	public class StateMachine : IStateMachine
    {
        public event Action OnStateChange; 
        
        private readonly IEnumerable<IStateLogicExecutor> _stateLogicExecutor = null;
        private readonly IEnumerable<IStateTransitionLogic> _transitions = null;
        private readonly IEnumerable<IStatePreProcessor> _statePreProcessors = null;
		private readonly IEnumerable<IStatePostProcessor> _statePostProcessors = null;

		public IState CurrentState { get; private set; }
		public string Name { get; private set; }
		public IState PreviousState { get; private set; }

		public StateMachine(IEnumerable<IStateLogicExecutor> stateLogicExecutor, IEnumerable<IStateTransitionLogic> transitions, 
            IEnumerable<IStatePreProcessor> statePreProcessor = null, IEnumerable<IStatePostProcessor> statePostProcessor = null)
            : this(nameof(StateMachine), stateLogicExecutor, transitions, statePreProcessor, statePostProcessor)
		{
		}

		public StateMachine(string name, IEnumerable<IStateLogicExecutor> stateLogicExecutor, IEnumerable<IStateTransitionLogic> transitions, 
            IEnumerable<IStatePreProcessor> statePreProcessor, IEnumerable<IStatePostProcessor> statePostProcessor)
        {
            Name = name;
            _stateLogicExecutor = stateLogicExecutor;
            _transitions = transitions;
			_statePreProcessors = statePreProcessor;
            _statePostProcessors = statePostProcessor;
		}

        public void EnterState(IState statToEnter)
		{
			if (CurrentState == statToEnter) return;

			PreviousState = CurrentState;

			foreach (var transition in _transitions)
			{
				transition.Cancel();
				transition.Perform(CurrentState, statToEnter);
			}

			if (CurrentState != null)
			{
				RemoveStateLogic();
				CurrentState.Exit();
				_statePostProcessors.Process(CurrentState);
			}

			CurrentState = statToEnter;

			_statePreProcessors.Process(CurrentState);
			CurrentState?.Enter();
			OnStateChange?.Invoke();
			
			SetStateLogic();
		}

		public void SetStateLogic()
		{
			foreach (var stateLogicExecutor in _stateLogicExecutor)
				stateLogicExecutor.SetLogicToExecute(CurrentState);
		}
		
		public void RemoveStateLogic()
		{
			foreach (var stateLogicExecutor in _stateLogicExecutor)
				stateLogicExecutor.RemoveLogicToExecute(CurrentState);
		}
	}
}