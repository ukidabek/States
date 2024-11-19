using System;
using UnityEngine;

namespace Utilities.States.Default
{
    [Serializable]
    public abstract class StateTransition : IStateTransition
    {
        [SerializeField] private State m_stateToEnter = null;
        public IState StateToEnter => m_stateToEnter;
        public abstract bool Validate();
    }
}