using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnFixedUpdateStateLogicExecutor")]
	public class OnFixedUpdateStateLogicExecutor : StateLogicExecutor<IOnFixUpdateLogic>
    {
        private void Update()
        {
			var timeInfo = GetTimeInfo();
			foreach (var onUpdateLogic in _logic)
				onUpdateLogic.OnFixUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}