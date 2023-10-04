using System.Collections.Generic;
using UnityEngine;

namespace Utilities.States.Default
{
	[StateLogicPath("States/StateLogic")]
	public class StatusHandlingStateLogic : StateLogic
    {
		[SerializeField] private GenericStatusHandler[] m_statusHandlers = null;

        private IEnumerable<StatusHandler> m_handlers;

		private void Awake() => InitializeHandlersList();

		private void InitializeHandlersList()
		{
			if (m_handlers != null) return;
			m_handlers = m_statusHandlers;
		}

		public override void Activate()
		{
			base.Activate();
			foreach (var handler in m_handlers)
                handler.Set();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			foreach (var handler in m_handlers)
				handler.Reset();
		}
	}
}