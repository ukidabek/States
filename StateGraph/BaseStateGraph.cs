using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

namespace BaseGameLogic.States
{
    public enum GraphType
    {
        Stack,
        Free
    }

    public class StateGraph : MonoBehaviour
    {
        [SerializeField] private GraphType _type = GraphType.Stack;
        public GraphType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [SerializeField] private Node _fromAnyStateNode = new Node();
        public Node FromAnyStateNode { get { return _fromAnyStateNode; } }

        [SerializeField] private List<Node> _nodeInfo = new List<Node>();
        public List<Node> NodeInfo { get { return _nodeInfo; } }

        [SerializeField] private List<StateTransition> _formAnyStateTransition = new List<StateTransition>();
        public List<StateTransition> FormAnyStateTransition { get { return _formAnyStateTransition; } }

        [SerializeField] private BaseState _rootState = null;
        public BaseState RootState
        {
            get { return _rootState; }
            set { _rootState = value; }
        }

        private bool _transitionDone = false;
        public bool TransitionDone { get { return _transitionDone; } }

        public StateTransition this[int i, int y]
        {
            get
            {
                if(i < 0)
                    return _formAnyStateTransition[y];

                return _nodeInfo[i].State.Transitions[y];
            }
        }

        public void HandleTransitions(StateHandler handler) 
		{
            _transitionDone = false;

            HandleTransitionLoop(handler, _formAnyStateTransition);
            HandleTransitionLoop(handler, handler.CurrentState.Transitions);

            if (Type == GraphType.Free || _transitionDone) return;

            for (int i = 0; i < handler.CurrentState.ExitStateTransitions.Count; i++)
            {
                BaseTransition transition = handler.CurrentState.ExitStateTransitions[i];
                if(transition.Validate(handler))
                {
                    break;
                }
            }
		}

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
    }
}