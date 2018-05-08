using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    public abstract class BaseStateTransitionCondition : MonoBehaviour
    {
        public virtual void GetConditionReferences(BaseState state, GameObject gameObject) {}

        public abstract bool Validate();
    }
}