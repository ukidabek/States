﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Transition to next state.
    /// Define conditions and state to enter.
    /// </summary>
    [Serializable]
    public class StateTransition : BaseTransition
    {
        [SerializeField] protected BaseState _ownerState = null;
        [SerializeField] protected BaseState _targetState = null;
        /// <summary>
        /// State that object will be when all conditions will be met.
        /// </summary>
        public BaseState TargetState { get { return _targetState; } }

        public StateTransition() {}

        public StateTransition(BaseState ownerState, BaseState targetState)
        {
            _ownerState = ownerState;
            _targetState = targetState;
        }

        public override bool Validate(StateHandler stateHandler)
        {
            for (int i = 0; i < Conditions.Capacity; i++)
            {
                if (!Conditions[i].Validate() || (_targetState as IState) == stateHandler.CurrentStateInterfaceHandler.CurrentState)
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