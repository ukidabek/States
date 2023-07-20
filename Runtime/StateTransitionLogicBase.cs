using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States
{
    public abstract class StateTransitionLogicBase : MonoBehaviour, IStateTransitionLogic
    {
        protected interface ITransitionLogicTriggerCondition
        {
            bool Validate( IState fromA, IState fromB, IState toA, IState toB);
        }
        
        private class FormTo : ITransitionLogicTriggerCondition
        {
            public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => fromA == fromB && toA == toB;
        }
        
        private class Form : ITransitionLogicTriggerCondition
        {
            public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => fromA == fromB;
        }
        
        private class To : ITransitionLogicTriggerCondition
        {
            public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => toA == toB;
        }
        
        protected enum TransitionMode { FromTo, From, To }

        [SerializeField] private TransitionMode _mode = TransitionMode.FromTo;
        [SerializeField] private Object _fromStateObject = null;
        [SerializeField] private Object _toStateObject = null;

        private IState _fromState = null;
        private IState _toState = null;
        
        protected ITransitionLogicTriggerCondition _condition = null;
        
        protected virtual void Awake()
        {
            _fromState = _fromStateObject as IState;
            _toState = _toStateObject as IState;
            InitializeCondition();
        }

        private void InitializeCondition()
        {
            _condition = _mode switch
			{
				TransitionMode.FromTo => new FormTo(),
				TransitionMode.From => new Form(),
				TransitionMode.To => new To(),
				_ => null,
			};
        }

        public virtual void Cancel() {}

        public virtual void Perform(IState from, IState to)
        {
            if (_condition.Validate(_fromState, from, _toState, to))
            {
                Perform();
            }
        }

        protected abstract void Perform();
        
        private void OnValidate() => InitializeCondition();
    }
}