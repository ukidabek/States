using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base class for transitions.
    /// </summary>
    public abstract class BaseTransition
    {
        [SerializeField] private List<BaseStateTransitionCondition> _conditions = new List<BaseStateTransitionCondition>();
        public List<BaseStateTransitionCondition> Conditions { get { return _conditions; } }

        /// <summary>
        /// Method defining how conditions in transitions are validated.
        /// </summary>
        /// <param name="stateHandler"></param>
        /// <returns></returns>
        public abstract bool Validate(StateHandler stateHandler);

        /// <summary>
        /// Defines what happens when transition is removed.  
        /// </summary>
        public abstract void Remove(); 
    }
}