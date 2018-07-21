using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using BaseGameLogic.States.Utility;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base game object.
    /// </summary>
    public partial class StateHandler : MonoBehaviour
    {
        #if UNITY_EDITOR

        [Header("Debug display & options.")]
        [SerializeField, Tooltip("Visualizes the contents of the stack.")]
        protected List<string> currentStateTypes = new List<string>();

        #endif

        #region States management variables

        /// <summary>
        /// Stack of states.
        /// </summary>
        protected Stack<StateInterfaceHandler> statesStack = new Stack<StateInterfaceHandler>();
        public static bool IsGamePaused = false;

        /// <summary>
        /// Reference to current state of the object.
        /// </summary>
        public StateInterfaceHandler CurrentStateInterfaceHandler { get { return statesStack.Count == 0 ? null : statesStack.Peek(); } }
        public bool KeepStatesOnStack = true;

        #endregion

        #region MonoBehaviour methods

        protected virtual void Update ()
        {
            if (IsGamePaused || CurrentStateInterfaceHandler == null)
                return;

            CurrentStateInterfaceHandler.Update();
        }

        protected virtual void LateUpdate()
        {
            if (IsGamePaused || CurrentStateInterfaceHandler == null)
                return;

            CurrentStateInterfaceHandler.LateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (IsGamePaused || CurrentStateInterfaceHandler == null)
                return;

            CurrentStateInterfaceHandler.FixedUpdate();
        }

        #endregion

        /// <summary>
        /// Enter a new state. 
        /// </summary>
        /// <param name="newState"> New state instance.</param>
        public void EnterState(IState newState)
        {
            if(newState == null)
                return;
            var requirement = StateUtility.GetAllRequirements(newState);
            if (StateUtility.GetAllRequiredReferences(requirement, gameObject, true))
            {
                var stateHandler = new StateInterfaceHandler(newState);
                if (KeepStatesOnStack)
                {
                    if (statesStack.Count > 0)
                        CurrentStateInterfaceHandler.Sleep();
                }
                else
                    statesStack.Pop().Exit();

                statesStack.Push(stateHandler);

                CurrentStateInterfaceHandler.Enter();

                #if UNITY_EDITOR
				currentStateTypes.Insert(0, newState.GetType().Name);
                #endif    
            }
            else
            {
                string typeName = newState.GetType().Name;
                Debug.LogErrorFormat("Conditions to enter the state type of {0} were not met.", typeName);
            }
        }

        /// <summary>
        /// Exits current state. 
        /// </summary>
        public void ExitState()
        {
            CurrentStateInterfaceHandler.Exit();
            statesStack.Pop();
            if(CurrentStateInterfaceHandler != null)
                CurrentStateInterfaceHandler.Awake();

            #if UNITY_EDITOR
            currentStateTypes.RemoveAt(0);
            #endif    
        }

        public void ReserState()
        {
            while(statesStack.Count > 1)
                ExitState();
        }

        public void ClearStack()
        {
            while (statesStack.Count > 0)
                ExitState();
        }
    }
}