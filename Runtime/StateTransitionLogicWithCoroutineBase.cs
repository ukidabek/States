using UnityEngine;
using Utilities.General;

namespace Utilities.States
{
	[AddComponentMenu("States/StateLogic/Transitions/StateTransitionLogicWithCoroutineBase")]
	public abstract class StateTransitionLogicWithCoroutineBase : StateTransitionLogicBase
    {
        protected CoroutineManager _manager;

        protected override void Awake()
        {
            base.Awake();
            _manager = new CoroutineManager(this);
        }
        
        public override void Cancel() => _manager.Stop();
    }
}