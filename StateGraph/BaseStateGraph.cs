using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.States.NodeDefinition;

namespace BaseGameLogic.States
{
    /// <summary>
    /// State graph.
    /// This class contains information about states, states transitions, conditions of transitions
    /// </summary>
    public class StateGraph : MonoBehaviour
    {
        [SerializeField] private GraphType _type = GraphType.Stack;
        /// <summary>
        /// Type of a graph.
        /// </summary>
        public GraphType Type { get { return _type; } set { _type = value; } }

        [SerializeField] private Node _fromAnyStateNode = new Node();
        /// <summary>
        /// Node used to define transitions form any state.
        /// </summary>
        public Node FromAnyStateNode { get { return _fromAnyStateNode; } }

        [SerializeField] private List<StateTransition> _formAnyStateTransition = new List<StateTransition>();
        /// <summary>
        /// Transition form any state.
        /// </summary>
        public List<StateTransition> FormAnyStateTransition { get { return _formAnyStateTransition; } }

        [SerializeField] private List<Node> _nodeInfo = new List<Node>();
        /// <summary>
        /// List of all nodes in graph.
        /// </summary>
        public List<Node> NodeInfo { get { return _nodeInfo; } }

        [SerializeField] private BaseState _rootState = null;
        /// <summary>
        /// Root state of graph. 
        /// This state will be treated abut default state. Object enters it on game start.
        /// </summary>
        public BaseState RootState { get { return _rootState; } set { _rootState = value; } }

        private bool _transitionDone = false;

        public StateHandler Handler;

        /// <summary>
        /// Returns stare transition.
        /// </summary>
        /// <param name="i">Node containing state index</param>
        /// <param name="y">State transition index</param>
        /// <returns>State transition</returns>
        public StateTransition this[int i, int y]
        {
            get
            {
                if(i < 0)
                    return _formAnyStateTransition[y];

                return _nodeInfo[i].State.Transitions[y];
            }
        }

        private void Awake()
        {
            Transform root = StateUtility.GetRootTransform(this.transform);

            foreach (var transition in _formAnyStateTransition)
            {
                foreach (var condition in transition.Conditions)
                {
                    condition.GetConditionReferences(null, root.gameObject);
                }
            }
        }

        /// <summary>
        /// Check all conditions in all transitions and if conditions are meat enters a new state.
        /// </summary>
        /// <param name="handler">State handler object</param>
        public void HandleTransitions(StateHandler handler) 
		{
            var state = (handler.CurrentStateInterfaceHandler.CurrentState as BaseState);
            _transitionDone = false;

            HandleTransitionLoop(handler, _formAnyStateTransition);
            HandleTransitionLoop(handler, state.Transitions);

            if (Type == GraphType.Free || _transitionDone)
                return;

            for (int i = 0; i < state.ExitStateTransitions.Count; i++)
            {
                BaseTransition transition = state.ExitStateTransitions[i];
                if(transition.Validate(handler))
                    break;
            }
		}

        /// <summary>
        /// Iterate by all transitions and validate it's condition.
        /// </summary>
        /// <param name="handler">State handler object</param>
        /// <param name="transitions">Transition list.</param>
        private void HandleTransitionLoop(StateHandler handler, List<StateTransition> transitions)
        {
            foreach (var item in transitions)
            {
                if (item.Validate(handler))
                {
                    _transitionDone = true;
                    break;
                }
            }
        }

        protected virtual void Update()
        {
            HandleTransitions(Handler);
        }
    }
}