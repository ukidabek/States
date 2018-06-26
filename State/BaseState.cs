using UnityEngine;

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.States.Assembly;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base state.
    /// </summary>
    public abstract class BaseState : MonoBehaviour, IState
    {
        [SerializeField] private StateHandler controlledObject = null;
        /// <summary>
        /// Object controlled by the states in graph.
        /// </summary>
        public StateHandler ControlledObject
        {
    		get { return this.controlledObject; }
			set { controlledObject = value; }
    	}

		private FieldInfo[] requiredFieldList = null;

		public Transform RootParent { get; private set; }

        [SerializeField] private List<StateTransition> _stateTransition = new List<StateTransition>();
        /// <summary>
        /// All transitions for this state
        /// </summary>
        public List<StateTransition> Transitions { get { return _stateTransition; } }

        public int SelectedExitStateTransition = 0; 

        [SerializeField] private List<ExitStateTransition> _exitStateTransitions = new List<ExitStateTransition>();
        /// <summary>
        /// All exit state transitions.
        /// </summary>
        public List<ExitStateTransition> ExitStateTransitions { get { return _exitStateTransitions; } }

        protected virtual void Awake() 
		{
			RootParent = StateUtility.GetRootTransform(this.transform);
			requiredFieldList = StateUtility.GetAllRequiredFields(this);

            foreach (var tranistion in _stateTransition)
                FillConditionReference(this, RootParent.gameObject, tranistion.Conditions);

            foreach (var item in _exitStateTransitions)
                FillConditionReference(this, RootParent.gameObject, item.Conditions);
        }

        private void FillConditionReference(BaseState baseState, GameObject parent, List<BaseStateTransitionCondition> conditions)
        {
            foreach (var condition in conditions)
                condition.GetConditionReferences(baseState, parent);
        }

  //      /// <summary>
  //      /// Returns root transform of object under control of states.
  //      /// </summary>
  //      /// <param name="parent">Start transform position</param>
  //      /// <returns>Root transform</returns>
  //      public static Transform GetRootTransform(Transform parent)
  //      {
  //          if (parent.parent == null)
  //              return parent;
  //          else
  //              return GetRootTransform(parent.parent);
  //      }

  //      /// <summary>
  //      /// Collect information for all required references for state to work.
  //      /// </summary>
  //      /// <returns></returns>
  //      public FieldInfo[] GetAllRequiredFields()
  //      {
		//	return StatesAssemblyExtension.GetAllFieldsWithAttribute(this.GetType(), typeof(RequiredReferenceAttribute), true).ToArray();
  //      }

		///// <summary>
		///// Get all references to fields marked with RequiredReference attribute 
		///// </summary>
		///// <param name="parent"></param>
		//public void GetAllRequiredReferences(GameObject parent = null, bool overrideReference = false)
		//{
		//	parent = parent == null ? GetRootTransform(this.transform).gameObject : parent;

  //          requiredFieldList = requiredFieldList == null ? GetAllRequiredFields() : requiredFieldList;

  //          foreach (FieldInfo field in requiredFieldList)
  //              if (overrideReference || field.GetValue(this) == null)
  //                  field.SetValue(this, GetComponentDeep(parent, field.FieldType));
  //      }

  //      protected Component GetComponentDeep(GameObject gameObject, Type type, bool includeInactive = false)
  //      {
  //          Component component = gameObject.GetComponent(type);
  //          if (component != null)
  //              return component;

  //          component = gameObject.GetComponentInChildren(type, includeInactive);
  //          if (component != null)
  //              return component;

  //          component = gameObject.GetComponentInParent(type);
  //          return component;
  //      }

        public virtual bool EnterConditions() { return true; }
        /// <summary>
        /// This method is called when system enter to this state.
        /// </summary>
        public abstract void OnEnter();
        /// <summary>
        /// Coaled when system exit this state.
        /// </summary>
        public abstract void OnExit();

        public virtual void OnAnimatorIK(int layerIndex) {}
    }
}