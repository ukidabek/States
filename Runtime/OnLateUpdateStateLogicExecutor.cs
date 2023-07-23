using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnLateUpdateStateLogicExecutor")]
	public class OnLateUpdateStateLogicExecutor : StateLogicExecutor<IOnLateUpdateLogic>
    {
        private void LateUpdate()
        {
			var timeInfo = GetTimeInfo();
			for (int i = 0; i < _logic.Count; i++)
				_logic[i].OnLateUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}