using UnityEngine;

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.Inputs;

using BaseGameLogic.LogicModule;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base state.
    /// </summary>
    public abstract class BaseState : MonoBehaviour
    {
        private StateHandler controlledObject = null;
		public StateHandler ControlledObject
        {
    		get { return this.controlledObject; }
			set { controlledObject = value; }
    	}

		private FieldInfo[] requiredFieldList = null;

		public Transform RootParent { get; private set; }

        [SerializeField] private List<StateTransition> _stateTransition = new List<StateTransition>();
        public List<StateTransition> Transitions { get { return _stateTransition; } }

        public int SelectedExitStateTransition = 0; 

        [SerializeField] private List<ExitStateTransition> _exitStateTransitions = new List<ExitStateTransition>();
        public List<ExitStateTransition> ExitStateTransitions { get { return _exitStateTransitions; } }

        //[SerializeField] private List<BaseStateTransitionCondition> _exitStateConditons = new List<BaseStateTransitionCondition>();
        //public List<BaseStateTransitionCondition> ExitStateConditons { get { return _exitStateConditons; } }

        protected virtual void Awake() 
		{
			RootParent = this.transform.GetRootTransform();
			requiredFieldList = GetAllRequiredFields();

            foreach (var tranistion in _stateTransition)
            {
                FillConditionReference(this, RootParent.gameObject, tranistion.Conditions);
            }

            foreach (var item in _exitStateTransitions)
            {
                FillConditionReference(this, RootParent.gameObject, item.Conditions);
            }
        }

        private void FillConditionReference(BaseState baseState, GameObject gameObject, List<BaseStateTransitionCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                condition.GetConditionReferences(baseState, gameObject);
            }
        }


        public FieldInfo[] GetAllRequiredFields()
        {
			return AssemblyExtension.GetAllFieldsWithAttribute(this.GetType(), typeof(RequiredReferenceAttribute), true).ToArray();
        }

		/// <summary>
		/// Get all references to fields marked with RequiredReference attribute 
		/// </summary>
		/// <param name="parent"></param>
		public void GetAllRequiredReferences(GameObject parent = null, bool overrideReference = false)
		{
			parent = parent == null ? this.transform.GetRootTransform().gameObject : parent;
			LogicModulesHandler handler = parent.GetComponentDeep<LogicModulesHandler>();

            GetAllRequiredReferences(handler, overrideReference);
        }

        public void GetAllRequiredReferences(LogicModulesHandler handler, bool overrideReference = false)
        {
			requiredFieldList = requiredFieldList == null ? GetAllRequiredFields() : requiredFieldList;

            foreach (FieldInfo field in requiredFieldList)
            {
                if (overrideReference || field.GetValue(this) == null)
                {
                    field.SetValue(this, handler.GetModule(field.FieldType));
                }
            }
        }

        public virtual bool EnterConditions() { return true; }
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnSleep();
        public abstract void OnAwake();
        public virtual void UpdateAnimator() {}
        public abstract void OnUpdate();
        public abstract void OnLateUpdate();
        public abstract void OnFixedUpdate();
        public virtual void HandleAnimator() {}
        public virtual void OnAnimatorIK(int layerIndex) {}
    }
}