using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Utilities.States.Default
{
	[AddComponentMenu("States/Core/StateSetter")]
	public class StateSetter : MonoBehaviour, IStateSetter
	{
#if UNITY_EDITOR
		[SerializeField] private string m_description;
		public string Description => m_description;
#endif

		private IStateMachine _stateManager = null;
		public IStateMachine StateMachine
		{
			get => _stateManager; 
			set
			{
				if (value is Object @object)
					_stateMachineObject = @object;
				_stateManager = value;
			}
		}

		[SerializeField] protected Object _stateMachineObject = null;
		[FormerlySerializedAs("_defaultState")]
		[SerializeField] protected State m_state = null;
		[SerializeField] protected bool m_returnToPreviousState = false;

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