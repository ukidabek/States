using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    [Serializable]
    public class StateTransition : BaseTransition
    { 
        [SerializeField] private BaseState _targetState = null;
        public BaseState TargetState { get { return _targetState; } }

        public StateTransition() {}

        public StateTransition(BaseState targetState)
        {
            _targetState = targetState;
        }

        public override bool Validate(StateHandler stateHandler)
        {
            for (int i = 0; i < Conditions.Capacity; i++)
            {
                if (!Conditions[i].Validate() || _targetState == stateHandler.CurrentState)
                    return false;
            }

            stateHandler.EnterState(_targetState);
            return true;
        }

        public override void Remove()
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(Conditions[i]);
                else
                    GameObject.DestroyImmediate(Conditions[i]);
            }

            Conditions.Clear();
        }
    }
}