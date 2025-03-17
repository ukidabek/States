using System.Diagnostics;
using States.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace States.Default
{
    [RequireComponent(typeof(IStateMachine))]
    public class StateMachineDebugger : MonoBehaviour
    {
        private IStateMachine m_stateMachine;
        [SerializeReference] private IState m_currentState = null;
        [SerializeField] private Object m_currentStateObject = null;
        
        [Conditional("UNITY_EDITOR")]
        private void Awake()
        {
            m_stateMachine = gameObject.GetComponent<IStateMachine>();
        }

        private void Start()
        {
            if (m_stateMachine is null) return;
            m_stateMachine.OnStateChanged += OnStateChanged;
            OnStateChanged(m_stateMachine.CurrentState);
        }

        private void OnDestroy()
        {
            if (m_stateMachine is null) return;
            m_stateMachine.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(IState currentState)
        {
            switch (currentState)
            {
                case Object unityEngineObject:
                    m_currentStateObject = unityEngineObject;
                    break;
                case object:
                    m_currentState = currentState;
                    break;
            }
        }
    }
}