using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
	[AddComponentMenu("States/Core/StateDictionary")]
	public class StateDictionary : MonoBehaviour
	{
		[SerializeField] private Object m_stateMachineObject = null;
		[SerializeField] private Object[] m_statesObjects = null;

		private IStateMachine m_stateMachine = null;
		private Dictionary<IStateID, IState> m_stateDictionary = new Dictionary<IStateID, IState>();

		private void Awake()
		{
			m_stateMachine = m_stateMachineObject as IStateMachine;
			var states = m_statesObjects.OfType<IState>();
			foreach (var state in states)
				m_stateDictionary.Add(state.ID, state);
		}

		private void Reset()
		{
			m_stateMachineObject = GetComponent<IStateMachine>() as Object;
			GenerateDictionary();
		}

		public void GenerateDictionary()
			=> m_statesObjects = GetComponentsInChildren<IState>()
							.Where(state => state.ID != null)
							.OfType<Object>()
							.ToArray();

		public void SetState(IStateID id)
		{
			if (m_stateDictionary.TryGetValue(id, out var state))
				m_stateMachine.EnterState(state);
		}
	}
}