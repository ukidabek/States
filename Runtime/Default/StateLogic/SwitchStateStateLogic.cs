using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States.Default
{
	[StateLogicPath("States/StateLogic")]
	public class SwitchStateStateLogic : StateLogic, IOnUpdateLogic
	{
		public enum ConditionMode { All, Any }

		[SerializeField] private ConditionMode _mode = ConditionMode.All;
		[SerializeField] private Object _stateMachineInstance = null;
		[SerializeField] private State _stateToEnter = null;
		[SerializeField] private Object[] _conditionsObjects = null;
		[SerializeField] private bool m_returnOnEmpty = false;
		[SerializeField] private bool m_switchEnabled = true;

		private List<ISwitchStateCondition> _stateConditions = new List<ISwitchStateCondition>();
		public IEnumerable<IContextDestination> ContextDestinations { get; private set; }

		private IStateMachine m_stateMachine = null;

		private bool Condition
		{
			get
			{
				var isEmpty = _stateConditions.Count() == 0;
				if (isEmpty) return m_returnOnEmpty;
				return _mode switch
				{
					ConditionMode.All => _stateConditions.All(condition => condition.Condition),
					ConditionMode.Any => _stateConditions.Any(condition => condition.Condition),
					_ => false
				};
			}
		}

		private void Awake()
		{
			m_stateMachine = _stateMachineInstance as IStateMachine;
			ContextDestinations = _stateConditions.OfType<IContextDestination>().Concat(_conditionsObjects.OfType<IContextDestination>());
		}

		public override void Activate()
		{
			_stateConditions = _stateConditions
				.Concat(_conditionsObjects.OfType<ISwitchStateCondition>())
				.ToList();
		}

		public void AddCondition(ISwitchStateCondition condition)
		{
			if (_stateConditions.Contains(condition)) return;
			_stateConditions.Add(condition);
		}

		public void RemoveCondition(ISwitchStateCondition condition)
		{
			if (!_stateConditions.Contains(condition)) return;
			_stateConditions.Remove(condition);
		}

		public virtual void OnUpdate(float deltaTime, float timeScale)
		{
			if (m_switchEnabled == false)
				return;

			if (Condition)
				Switch();
		}

		public void Switch()
		{
			var stateToEnter = _stateToEnter as IState;
			if (_stateToEnter == null || m_stateMachine.CurrentState == stateToEnter)
				return;
			m_stateMachine.EnterState(_stateToEnter);
		}

#if UNITY_EDITOR
		public void GetStateMachineObject()
		{
			var stateMachine = GetStateMachine(transform.parent);
			_stateMachineInstance = stateMachine as Object;
		}

		private IStateMachine GetStateMachine(Transform transform)
		{
			var gameObject = transform.gameObject;
			var stateMachine = gameObject.GetComponent<IStateMachine>();

			if (stateMachine != null)
				return stateMachine;

			return GetStateMachine(transform.parent);
		}

		public void GetConditions()
		{
			var components = GetComponentsInChildren<ISwitchStateCondition>();
			_conditionsObjects = components.OfType<Object>().ToArray();
		}
#endif
	}
}