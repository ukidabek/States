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
        public Transform RootParent = null;

        [SerializeField, RequiredReference] private List<StateTransition> _stateTransition = new List<StateTransition>();
        /// <summary>
        /// All transitions for this state
        /// </summary>
        public List<StateTransition> Transitions { get { return _stateTransition; } }

        public int SelectedExitStateTransition = 0; 

        [SerializeField, RequiredReference] private List<ExitStateTransition> _exitStateTransitions = new List<ExitStateTransition>();
        /// <summary>
        /// All exit state transitions.
        /// </summary>
        public List<ExitStateTransition> ExitStateTransitions { get { return _exitStateTransitions; } }

        protected virtual void Awake() 
		{
            RootParent = StateUtility.GetRootTransform(transform);

            foreach (var tranistion in _stateTransition)
                FillConditionReference(this, tranistion.Conditions);

            foreach (var item in _exitStateTransitions)
                FillConditionReference(this, item.Conditions);
        }

        private void FillConditionReference(BaseState baseState, List<BaseStateTransitionCondition> conditions)
        {
            foreach (var condition in conditions)
                condition.State = baseState;
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