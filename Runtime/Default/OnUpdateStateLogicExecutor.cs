using UnityEngine;

namespace Utilities.States
{
    [AddComponentMenu("States/Executors/OnUpdateStateLogicExecutor")]
    [DisallowMultipleComponent]
	public class OnUpdateStateLogicExecutor : StateLogicExecutor
    {
        private void Update()
        {
            var timeInfo = GetTimeInfo();
            m_stateMachine.OnUpdate(timeInfo.deltaTime, timeInfo.timeScale);
        }
    }
}