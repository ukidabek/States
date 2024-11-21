using UnityEngine;

namespace States.Default
{
    [AddComponentMenu("States/Executors/OnFixedUpdateStateLogicExecutor")]
    [DisallowMultipleComponent]
    public class OnFixedUpdateStateLogicExecutor : StateLogicExecutor
    {
        protected override (float deltaTime, float timeScale) GetTimeInfo() => (Time.fixedDeltaTime, Time.timeScale);

        private void FixedUpdate()
        {
            var timeInfo = GetTimeInfo();
            m_stateMachine.OnFixedUpdate(timeInfo.deltaTime, timeInfo.timeScale);
        }
    }
}