using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnFixedUpdateStateLogicExecutor")]
	[DisallowMultipleComponent]
	public class OnFixedUpdateStateLogicExecutor : StateLogicExecutor<IOnFixedUpdateLogic>
    {
		protected override (float, float) GetTimeInfo() => (Time.fixedDeltaTime, Time.timeScale);

		private void FixedUpdate()
        {
			var timeInfo = GetTimeInfo();
			for (int i = 0; i < m_logic.Count; i++)
				m_logic[i].OnFixexUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}