using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using BaseGameLogic.States.Utility;

namespace BaseGameLogic.States
{
    public abstract class BaseStateTransitionCondition : MonoBehaviour
    {
        protected BaseState state = null;

        public void GetConditionReferences(BaseState state, GameObject parent)
        {
            this.state = state;
            var requiredFields = StateUtility.GetAllRequiredFields(this);
            StateUtility.GetAllRequiredReferences(this, requiredFields, parent);
        }

        public abstract bool Validate();
    }
}