using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base game object.
    /// </summary>
    public class StateHandler : MonoBehaviour
    {
        #if UNITY_EDITOR

        [Header("Debug display & options.")]
        [SerializeField, Tooltip("Visualizes the contents of the stack.")]
        protected List<string> currentStateTypes = new List<string>();

        #endif

        #region States management variables

		[Header("States management."), FormerlySerializedAs("enterDefaultStateOnAwake")]
        [SerializeField] protected bool enterDefaultStateOnStart = false;

        /// <summary>
        /// Stack of states.
        /// </summary>
        protected Stack<StateInterfaceHandler> statesStack = new Stack<StateInterfaceHandler>();
        public static bool IsGamePaused = false;

        /// <summary>
        /// Reference to current state of the object.
        /// </summary>
        public StateInterfaceHandler CurrentStateInterfaceHandler { get { return statesStack.Peek(); } }

        public bool KeepStatesOnStack = true;

        #endregion

        public class StateInterfaceHandler
        {
            public IState CurrentState { get; private set; }
            public IOnSleep OnSleepInterface { get; private set; }
            public IOnAwake OnAwakeInterface { get; private set; }
            public IOnUpdate OnUpdateInterface { get; private set; }
            public IOnLateUpdate OnLateUpdateInterface { get; private set; }
            public IOnFixedUpdate OnFixedUpdateInterface { get; private set; }

            public StateInterfaceHandler(IState state)
            {
                CurrentState = state;
                OnSleepInterface = GetInterface<IOnSleep>(state);
                OnAwakeInterface = GetInterface<IOnAwake>(state);
                OnUpdateInterface = GetInterface<IOnUpdate>(state);
                OnLateUpdateInterface = GetInterface<IOnLateUpdate>(state);
                OnFixedUpdateInterface = GetInterface<IOnFixedUpdate>(state);
            }

            private T GetInterface<T>(object state) where T : class
            {
                if (state is T)
                    return state as T;

                return null;
            }
        }

        protected virtual void Awake() {}

        protected virtual void Start () {}

        #region MonoBehaviour methods


        protected virtual void Update ()
        {
            if (IsGamePaused && CurrentStateInterfaceHandler != null)
                return;

			if(CurrentStateInterfaceHandler.OnUpdateInterface != null)
               CurrentStateInterfaceHandler.OnUpdateInterface.OnUpdate();
        }

        protected virtual void LateUpdate()
        {
            if (IsGamePaused && CurrentStateInterfaceHandler != null)
                return;

            if (CurrentStateInterfaceHandler.OnLateUpdateInterface != null)
                CurrentStateInterfaceHandler.OnLateUpdateInterface.OnLateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            if (IsGamePaused && CurrentStateInterfaceHandler != null)
                return;
            if (CurrentStateInterfaceHandler.OnFixedUpdateInterface != null)
                CurrentStateInterfaceHandler.OnFixedUpdateInterface.OnFixedUpdate();
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

            var requiredFields = StateUtility.GetAllRequiredFields(newState);
            if (StateUtility.GetAllRequiredReferences(newState, requiredFields, this.gameObject))
            {
                var stateHandler = new StateInterfaceHandler(newState);
                if (KeepStatesOnStack)
                    if (statesStack.Count > 0)
                        if(CurrentStateInterfaceHandler.OnSleepInterface != null)
                            CurrentStateInterfaceHandler.OnSleepInterface.OnSleep();
                else
                    statesStack.Pop();

                statesStack.Push(stateHandler);

                newState.ControlledObject = this;
                newState.OnEnter();

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
            if (KeepStatesOnStack)
                return;

            IState oldState = statesStack.Pop().CurrentState;
            oldState.ControlledObject = null;
            oldState.OnExit();

            if(CurrentStateInterfaceHandler.OnAwakeInterface != null)
                CurrentStateInterfaceHandler.OnAwakeInterface.OnAwake();

            #if UNITY_EDITOR
            currentStateTypes.RemoveAt(0);
            #endif    
        }

        public void ReserState()
        {
            while(statesStack.Count > 1)
                ExitState();
        }
    }
}