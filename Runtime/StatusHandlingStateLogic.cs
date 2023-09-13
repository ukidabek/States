using Stages;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
	[StateLogicPath("States/StateLogic")]
	public class StatusHandlingStateLogic : StateLogic
    {
		[SerializeField] private GenericStatusHandler[] m_statusHandlers = null;

        private IEnumerable<StatusHandler> m_handlers;

		private void InitializeHandlersList()
		{
			if (m_handlers != null) return;
			m_handlers = new List<StatusHandler>()
				.Concat(m_statusHandlers)
				.ToArray();
		}

		public override void Activate()
		{
			base.Activate();
			InitializeHandlersList();
			foreach (var handler in m_handlers)
                handler.Set();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			foreach (var handler in m_handlers)
				handler.Reset();
		}
#if UNITY_EDITOR
		[ContextMenu("Migrate")]
		private void Migrate()
		{
			m_statusHandlers = new List<StatusHandler>()
				.Select(handler => new GenericStatusHandler(handler))
				.ToArray();
			UnityEditor.EditorUtility.SetDirty(gameObject);
		}
#endif
	}
}