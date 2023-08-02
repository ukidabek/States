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
        [SerializeField] private GameObjectStatusHandler[] m_gameObjectStatusHandlers = null;
        [SerializeField] private BehaviourStatusHandler[] m_behaviourStatusHandler = null;

        private IEnumerable<StatusHandler> m_handlers;

		private void InitializeHandlersList()
		{
			if (m_handlers != null) return;
			m_handlers = new List<StatusHandler>()
				.Concat(m_statusHandlers)
				.Concat(m_behaviourStatusHandler)
				.Concat(m_gameObjectStatusHandlers)
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
				.Concat(m_gameObjectStatusHandlers)
				.Concat(m_behaviourStatusHandler)
				.Select(handler => new GenericStatusHandler(handler))
				.ToArray();

			m_gameObjectStatusHandlers = null;
			m_behaviourStatusHandler = null;

			UnityEditor.EditorUtility.SetDirty(gameObject);
		}
#endif
	}
}