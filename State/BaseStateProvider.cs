using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGameLogic.States.Providers
{ 
    [RequireComponent(typeof(StateHandler))]
    public abstract class BaseStateProvider : MonoBehaviour
    {
        [SerializeField] private StateHandler _stateHandler = null;
        [SerializeField] private bool _enterDefaultState = true;
        public abstract IState DefaultState { get; }

        protected virtual void Start()
        {
            EnterDefaultState();
        }

        public void EnterDefaultState()
        {
            if (_enterDefaultState && _stateHandler != null)
                _stateHandler.EnterState(DefaultState);
        }

        private void Reset()
        {
            _stateHandler = GetComponent<StateHandler>();
        }
    }
}