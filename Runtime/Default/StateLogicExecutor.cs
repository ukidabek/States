using States.Core;
using UnityEngine;

namespace States.Default
{
    public abstract class StateLogicExecutor : MonoBehaviour, IStateLogicExecutor
    {
        protected IStateMachine m_stateMachine = null;
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public void ProvideStateMachine(IStateMachine stateMachine) => m_stateMachine = stateMachine;

        protected virtual (float deltaTime, float timeScale) GetTimeInfo() => (Time.deltaTime, Time.timeScale);
    }
}