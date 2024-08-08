using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnLateUpdateStateLogicExecutor")]
	[DisallowMultipleComponent]
	public class OnLateUpdateStateLogicExecutor : StateLogicExecutor<IOnLateUpdateLogic>
    {
        private void LateUpdate()
        {
			var timeInfo = GetTimeInfo();
			for (int i = 0; i < m_logic.Count; i++)
				m_logic[i].OnLateUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}