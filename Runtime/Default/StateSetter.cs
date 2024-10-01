using UnityEngine;
using UnityEngine.Assertions;

namespace Utilities.States.Default
{
	[AddComponentMenu("States/Core/StateSetter")]
	public class StateSetter : MonoBehaviour, IStateSetter
	{
#if UNITY_EDITOR
		[SerializeField] private string m_description;
		public string Description => m_description;
#endif

		private IStateSetter m_setter = null;

		public IState State
		{
			get => m_setter == null ? null : m_setter.State;
			set
			{
				if (m_state == null) return;
				m_setter.State = value;
			}
		}

		public IStateMachine StateMachine
		{
			get => m_setter.StateMachine;
			set => m_setter.StateMachine = value;
		}

		[SerializeField] protected Object _stateMachineObject = null;
		[SerializeField] protected State m_state = null;
		[SerializeField] protected bool m_returnToPreviousState = false;

		public void SetState()
		{
			if (m_setter == null)
			{
				Assert.IsTrue(_stateMachineObject is IStateMachine);
				m_setter = new States.StateSetter(_stateMachineObject as IStateMachine, m_state);
			}
			m_setter.SetState();
		}
	}
}