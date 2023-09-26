using UnityEngine;

namespace Utilities.States
{
    [AddComponentMenu("States/Executors/OnUpdateStateLogicExecutor")]
    [DisallowMultipleComponent]
	public class OnUpdateStateLogicExecutor : StateLogicExecutor<IOnUpdateLogic>
    {
        private void Update()
        {
			var timeInfo = GetTimeInfo();
            for (int i = 0; i < _logic.Count; i++) 
				_logic[i].OnUpdate(timeInfo.Item1, timeInfo.Item2);
        }
    }
}