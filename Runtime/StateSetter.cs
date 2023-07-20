using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Utilities.States
{
	public class StateSetter : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private string m_description;
#endif
		[SerializeField] protected Object _stateMachineObject = null;
		[FormerlySerializedAs("_defaultState")]
		[SerializeField] protected State m_state = null;
		[SerializeField] protected bool m_returnToPreviousState = false;

		private IStateMachine _stateManager = null;

		public void SetState()
		{
			Assert.IsNotNull(m_state, "No state selected!");

			if (_stateManager == null)
				_stateManager = _stateMachineObject as IStateMachine;

			var stateToEnter = m_returnToPreviousState ? _stateManager.PreviousState : m_state;
			_stateManager.EnterState(stateToEnter);
		}
	}
}