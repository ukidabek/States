using UnityEngine;

using System.Reflection;
using System.Collections.Generic;

using BaseGameLogic.States.Utility;

namespace BaseGameLogic.States
{
    /// <summary>
    /// Base state.
    /// </summary>
    public abstract class BaseState : MonoBehaviour, IState
    {
		private FieldInfo[] requiredFieldList = null;

        public Transform RootParent = null;

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