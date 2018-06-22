using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGameLogic.States
{
    /// <summary>
    /// This type of transition is used when state graph in stack mode. 
    /// Define conditions necessary to exit current active state.
    /// </summary>
    [Serializable]
    public class ExitStateTransition : BaseTransition
    {
        public override void Remove() {}

        public override bool Validate(StateHandler stateHandler)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (!Conditions[i].Validate())
                    return false;
            }

            stateHandler.ExitState();
            return true;
        }
    }
}