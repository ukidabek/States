using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using BaseGameLogic.States.Utility;

namespace BaseGameLogic.States
{
    public abstract class BaseStateTransitionCondition : MonoBehaviour
    {
        public BaseState State = null;

        //public void GetConditionReferences(BaseState state, GameObject parent)
        //{
        //    this.State = state;
        //    var requiredFields = StateUtility.GetAllRequiredFields(this);
        //    StateUtility.GetAllRequiredReferences(this, requiredFields, parent);
        //}

        public abstract bool Validate();
    }
}