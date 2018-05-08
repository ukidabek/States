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
			RootParent = GetRootTransform(this.transform);
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

        public static Transform GetRootTransform(Transform parent)
        {
            if (parent.parent == null)
                return parent;
            else
                return GetRootTransform(parent.parent);
        }

        public FieldInfo[] GetAllRequiredFields()
        {
			return StatesAssemblyExtension.GetAllFieldsWithAttribute(this.GetType(), typeof(RequiredReferenceAttribute), true).ToArray();
        }

		/// <summary>
		/// Get all references to fields marked with RequiredReference attribute 
		/// </summary>
		/// <param name="parent"></param>
		public void GetAllRequiredReferences(GameObject parent = null, bool overrideReference = false)
		{
			parent = parent == null ? GetRootTransform(this.transform).gameObject : parent;
			//LogicModulesHandler handler = GetComponentDeep<LogicModulesHandler>(parent);

            requiredFieldList = requiredFieldList == null ? GetAllRequiredFields() : requiredFieldList;

            foreach (FieldInfo field in requiredFieldList)
            {
                if (overrideReference || field.GetValue(this) == null)
                {
                    field.SetValue(this, GetComponentDeep(parent, field.FieldType));
                }
            }
        }

        //     public void GetAllRequiredReferences(MonoBehaviour handler, bool overrideReference = false)
        //     {
        //requiredFieldList = requiredFieldList == null ? GetAllRequiredFields() : requiredFieldList;

        //         foreach (FieldInfo field in requiredFieldList)
        //         {
        //             if (overrideReference || field.GetValue(this) == null)
        //             {
        //                 field.SetValue(this, handler.GetModule(field.FieldType));
        //             }
        //         }
        //     }

        protected Component GetComponentDeep(GameObject gameObject, Type type, bool includeInactive = false)
        {
            Component component = gameObject.GetComponent(type);
            if (component != null)
                return component;

            component = gameObject.GetComponentInChildren(type, includeInactive);
            if (component != null)
                return component;

            component = gameObject.GetComponentInParent(type);
            return component;
        }

        /// <summary>
        /// Tray get component form parent, object and it's children's.
        /// </summary>
        /// <typeparam name="T">Type of component to find.</typeparam>
        /// <param name="gameObject">Reference to object.</param>
        /// <returns>Reference to component.</returns>
        protected T GetComponentDeep<T>(GameObject gameObject, bool includeInactive = false) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
                return component;

            component = gameObject.GetComponentInChildren<T>(includeInactive);
            if (component != null)
                return component;

            component = gameObject.GetComponentInParent<T>();
            return component;
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