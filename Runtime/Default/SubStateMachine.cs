using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.States.Default
{

	[StateLogicPath("States/StateLogic")]
	public class SubStateMachine : StateMachineManager, IStateLogic, IStateMachine
	{
		[SerializeField] private LogicExecutorHandlingMode m_logicExecutorHandlingMode = LogicExecutorHandlingMode.AddRemove;

		private IEnumerable<IStateLogicExecutor> m_stateLogicExecutors = null;

		public virtual IEnumerable<IContextDestination> ContextDestinations => Array.Empty<IContextDestination>();


		public void Activate()
		{
			switch (m_logicExecutorHandlingMode)
			{
				case LogicExecutorHandlingMode.AddRemove:
					if (m_stateMachine == null) break;
					m_stateMachine.CurrentState?.Enter();
					m_stateMachine.SetStateLogic();
					break;
				case LogicExecutorHandlingMode.EnableDisable:
					foreach (var executor in m_stateLogicExecutors)
						executor.Enabled = true;
					break;
			}
		}

		public void Deactivate()
		{
			switch (m_logicExecutorHandlingMode)
			{
				case LogicExecutorHandlingMode.AddRemove:
					if (m_stateMachine == null) break;
					m_stateMachine.CurrentState?.Exit();
					m_stateMachine.RemoveStateLogic();
					break;
				case LogicExecutorHandlingMode.EnableDisable:
					foreach (var executor in m_stateLogicExecutors)
						executor.Enabled = false;
					break;
			}
		}
	}
}