using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnLateUpdateStateLogicExecutor")]
	public class OnLateUpdateStateLogicExecutor : StateLogicExecutor<IOnLateUpdateLogic>
    {
        private void LateUpdate()
        {
			var timeInfo = GetTimeInfo();
			foreach (var onUpdateLogic in _logic)
				onUpdateLogic.OnLateUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}