using System;
using UnityEngine;

namespace Stages
{
	[Serializable]
	public class GameObjectStatusHandler : StatusHandler<GameObject>
	{
		public GameObjectStatusHandler(GameObject managedObject, bool status) : base(managedObject, status) { }

		public override void Reset()
		{
			var statusToSet = !m_status;
			if (m_managedObject.activeSelf != statusToSet)
				m_managedObject.SetActive(statusToSet);
		}

		public override void Set()
		{
			if (m_managedObject.activeSelf != m_status)
				m_managedObject.SetActive(m_status);
		}
	}
}