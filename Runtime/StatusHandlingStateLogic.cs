using Stages;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
	public class StatusHandlingStateLogic : StateLogic
    {
        [SerializeField] private GameObjectStatusHandler[] m_gameObjectStatusHandlers = null;
        [SerializeField] private BehaviourStatusHandler[] m_behaviourStatusHandler = null;
        private IEnumerable<StatusHandler> m_handlers;


		private void InitializeHandlersList()
		{
			if (m_handlers != null) return;
			m_handlers = new List<StatusHandler>()
				.Concat(m_behaviourStatusHandler)
				.Concat(m_gameObjectStatusHandlers);
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
	}
}