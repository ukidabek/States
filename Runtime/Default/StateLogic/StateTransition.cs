using System;
using UnityEngine;

namespace States.Core.Default
{
    [Serializable]
    public abstract class StateTransition : IStateTransition
    {
        [SerializeField] private State m_stateToEnter = null;
        public IState StateToEnter => m_stateToEnter;
        public abstract bool Validate();
    }
}