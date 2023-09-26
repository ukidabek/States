using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States
{ 
	public abstract class StateTransitionLogic : MonoBehaviour, IStateTransition
    {
		[SerializeField] private TransitionMode _mode = TransitionMode.FromTo;
        [SerializeField] private Object _fromStateObject = null;
        [SerializeField] private Object _toStateObject = null;

		private StateTransition m_stateTransition = null;

		protected virtual void Awake() => m_stateTransition = new StateTransition(_fromStateObject as IState, _toStateObject as IState, _mode, Cancel, Perform);

		public virtual void Cancel() {}

		public virtual void Perform(IState from, IState to) => m_stateTransition.Perform(from, to);

		protected abstract void Perform();
    }
}