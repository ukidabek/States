using UnityEngine;

namespace Utilities.States
{
    [AddComponentMenu("States/Executors/OnUpdateStateLogicExecutor")]
	public class OnUpdateStateLogicExecutor : StateLogicExecutor<IOnUpdateLogic>
    {
        private void Update()
        {
			var timeInfo = GetTimeInfo();
			foreach (var onUpdateLogic in _logic) 
                onUpdateLogic.OnUpdate(timeInfo.Item1, timeInfo.Item2);
        }
    }
}