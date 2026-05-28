using System;
using UnityEngine;
using Utilities.General;

namespace States.Default
{
    public class BlackboardEventHandler : MonoBehaviour
    {
        [Serializable]
        private class BlackboardValueChangeHandlerHost
        {
            [field: SerializeField] public Key Key { get; private set; }
            [SerializeReference, ReferenceList] private IBlackboardValueChangeHandler[] m_handlers = null;
			
            public void Notify(object value)
            {
                foreach (var handler in m_handlers) 
                    handler.HandleValueChange(value);
            }
        }
		
        [SerializeField] private StateMachineHost m_stateMachine = null;
        [SerializeField] private BlackboardValueChangeHandlerHost[] m_blackboardValueChangeHandlerHost = null;

        private void Start()
        {
            var blackboard = m_stateMachine.Blackboard;
            if (blackboard == null) return;

            foreach (var host in m_blackboardValueChangeHandlerHost)
                blackboard.AddOnValueChangeListener(host.Key, host.Notify);
        }

        private void OnDestroy()
        {
            var blackboard = m_stateMachine.Blackboard;
            if (blackboard == null) return;

            foreach (var host in m_blackboardValueChangeHandlerHost)
                blackboard.RemoveOnValueChangeListener(host.Key, host.Notify);
        }
    }
}