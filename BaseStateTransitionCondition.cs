using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.LogicModule;

namespace BaseGameLogic.States
{
    public abstract class BaseStateTransitionCondition : MonoBehaviour
    {
        public virtual void GetConditionReferences(BaseState state, GameObject gameObject) {}

        public abstract bool Validate();
    }
}