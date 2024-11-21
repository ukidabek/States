using System;
using System.Collections.Generic;
using Default;
using UnityEngine;

namespace States.Core.Default
{

	[StateLogicPath("States/StateLogic")]
	public class SubStateMachine : StateMachineHost, IStateLogic, IStateMachine
	{
		[SerializeField] private LogicExecutorHandlingMode m_logicExecutorHandlingMode = LogicExecutorHandlingMode.AddRemove;

		private IEnumerable<IStateLogicExecutor> m_stateLogicExecutors = null;

		public virtual IEnumerable<IContextDestination> ContextDestinations => Array.Empty<IContextDestination>();

		public bool CanBeDeactivated => true;

		public void Activate()
		{
			switch (m_logicExecutorHandlingMode)
			{
				case LogicExecutorHandlingMode.AddRemove:
					if (m_stateMachine == null) break;
					m_stateMachine.CurrentState?.Enter();
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
					break;
				case LogicExecutorHandlingMode.EnableDisable:
					foreach (var executor in m_stateLogicExecutors)
						executor.Enabled = false;
					break;
			}
		}
	}
}