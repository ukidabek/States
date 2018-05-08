using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGameLogic.States
{
    public abstract class BaseTransition
    {
        [SerializeField] private List<BaseStateTransitionCondition> _conditions = new List<BaseStateTransitionCondition>();
        public List<BaseStateTransitionCondition> Conditions { get { return _conditions; } }

        public abstract bool Validate(StateHandler stateHandler);
        public abstract void Remove(); 
    }
}