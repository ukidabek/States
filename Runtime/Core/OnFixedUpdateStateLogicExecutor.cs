using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Executors/OnFixedUpdateStateLogicExecutor")]
	[DisallowMultipleComponent]
	public class OnFixedUpdateStateLogicExecutor : StateLogicExecutor<IOnFixUpdateLogic>
    {
        private void Update()
        {
			var timeInfo = GetTimeInfo();
			for (int i = 0; i < _logic.Count; i++)
				_logic[i].OnFixUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}