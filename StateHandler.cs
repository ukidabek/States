using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.Management;
using BaseGameLogic.Inputs;
using BaseGameLogic.Audio;
using BaseGameLogic.LogicModule;

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

		[Header("States management.")]
        [SerializeField] protected bool enterDefaultStateOnAwake = false;

        [SerializeField] protected StateGraph _graph = null;
        [SerializeField] protected GameObject stateGraphPrefab = null;


        protected BaseState _currentState = null;
        /// <summary>
        /// Stack of states.
        /// </summary>
        protected Stack<BaseState> statesStack = new Stack<BaseState>();

        /// <summary>
        /// Reference to current state of the object.
        /// </summary>
        public BaseState CurrentState
        {
            get 
            {
                switch (_graph.Type)
                {
                    case GraphType.Stack:
                        if (statesStack.Count == 0) return null;
                        return statesStack.Peek();

                    case GraphType.Free:
                        return _currentState;
                }

                return null;
            }
        }

        #endregion

        #region Managers references

        protected bool GameManagerExist { get; private set; }
        protected BaseGameManager GameManagerInstance { get { return BaseGameManager.Instance; } }

        /// <summary>
        /// Enable or disable execution of StateObject updates methods.
        /// </summary>
		protected bool IsGamePaused { get { return GameManagerExist && GameManagerInstance.GameStatus == GameStatusEnum.Pause; } }

        #endregion

        /// <summary>
        /// Enters the default state of the object.
        /// </summary>
        protected virtual void EnterDefaultState()
        {
            if(_graph != null)
            {
                this.EnterState(_graph.RootState);
                return;
            }
        }

        protected virtual void Awake()
        {
            if(stateGraphPrefab != null)
            {
                _graph = Instantiate(stateGraphPrefab, this.transform, false).GetComponent<StateGraph>();
            }
        }

        protected virtual void Start () 
        {
            if(enterDefaultStateOnAwake)
                EnterDefaultState();

            GameManagerExist = GameManagerInstance != null;
            if(GameManagerExist)
			    GameManagerInstance.ObjectInitializationCallBack.AddListener(InitializeObject);
        }

		/// <summary>
		/// This method is called by GameManager in first update of this object.
		/// </summary>
		protected virtual void InitializeObject(BaseGameManager gameManager)
        {
            if(!enterDefaultStateOnAwake)
                EnterDefaultState();
        }

        #region MonoBehaviour methods

        protected virtual void OnDestroy()
        {
            if(GameManagerExist)
                GameManagerInstance.ObjectInitializationCallBack.RemoveListener(InitializeObject);
        }

        protected virtual void Update ()
        {
			if (IsGamePaused && CurrentState != null)
				return;

            if(_graph != null)
                _graph.HandleTransitions(this);

			CurrentState.OnUpdate();
        }

        protected virtual void LateUpdate()
        {
			if (IsGamePaused && CurrentState != null) 
                return;
			
			CurrentState.OnLateUpdate();
        }

        protected virtual void FixedUpdate()
        {
			if (IsGamePaused && CurrentState != null) 
                return;
			
			CurrentState.OnFixedUpdate();
        }

        public virtual void OnAnimatorIK(int layerIndex)
        {
            if (IsGamePaused && CurrentState != null) 
                return;

            CurrentState.OnAnimatorIK(layerIndex);
        }

        #endregion

        /// <summary>
        /// Enter a new state. 
        /// </summary>
        /// <param name="newState"> New state instance.</param>
        public void EnterState(BaseState newState)
        {
            if(newState == null)
                return;

            if (newState.EnterConditions())
            {
                if(_graph.Type == GraphType.Stack)
                {
                    if (statesStack.Count > 0)
                        CurrentState.OnSleep();
                    
                    statesStack.Push(newState);
                }
                else
                {
                    _currentState = newState;
                }

                newState.ControlledObject = this;
                newState.GetAllRequiredReferences(this.gameObject, true);
                CurrentState.ControlledObject = this;
                CurrentState.OnEnter();

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
            if (_graph.Type == GraphType.Free) return;

            BaseState oldState = statesStack.Pop();
            oldState.ControlledObject = null;
            oldState.OnExit();
            CurrentState.OnAwake();

            #if UNITY_EDITOR
            currentStateTypes.RemoveAt(0);
            #endif    
        }
    }
}