using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.States
{
	[CustomEditor(typeof(SwitchStateStateLogic))]
	public class SwitchStateStateLogicEditor : Editor
	{
		private SwitchStateStateLogic m_switchStateStateLogic = null;
		private FieldInfo m_stateFieldInfo = null;
		private FieldInfo m_conditionsObjectsFieldInfo = null;

		private IEnumerable<IStateMachine> m_stateMachines = null;
		private IEnumerable<IState> m_states = null;
		private IEnumerable<ISwitchStateCondition> m_conditions = null;

		private bool m_showStateMachines = false;
		private bool m_showStateSelections = false;
		private bool m_showConditionSelection = false;

		private bool[] m_selectedConditions = Array.Empty<bool>();

		private SerializedProperty m_stateMachineSerializedProperty = null;
		private SerializedProperty m_stateSerializedProperty = null;

		private void OnEnable()
		{
			m_switchStateStateLogic = (target as SwitchStateStateLogic);
			var type = m_switchStateStateLogic.GetType();
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			m_stateFieldInfo = type.GetField("_stateToEnter", bindingFlags);
			m_conditionsObjectsFieldInfo = type.GetField("_conditionsObjects", bindingFlags);
			m_stateMachines = m_switchStateStateLogic.GetComponentsFormRoot<IStateMachine>();
			m_states = m_switchStateStateLogic.GetComponentsFormRoot<IState>();
			m_stateMachineSerializedProperty = serializedObject.FindProperty("_stateMachineInstance");
			m_stateSerializedProperty = serializedObject.FindProperty("_stateToEnter");
			m_conditions = m_switchStateStateLogic.GetComponents<ISwitchStateCondition>();
		}

		public override void OnInspectorGUI()
		{
			var state = m_stateFieldInfo.GetValue(target) as State;
			if (state != null)
			{
				var oldColor = GUI.color;
				GUI.color = Color.yellow;
				GUILayout.Label(state.name);
				GUI.color = oldColor;
			}

			base.OnInspectorGUI();

			m_stateMachines.ObjectSelector(ref m_showStateMachines,
				$"Select {nameof(IStateMachine)}",
				(selectedStateMachine) => StateEditorHelper.ApplyObject(selectedStateMachine, m_stateMachineSerializedProperty),
				(stateMachine) =>
				{
					if (stateMachine is Component component)
						return $"{component.gameObject.GetFullName()}/{stateMachine.Name}";
					return stateMachine.Name;
				});


			m_states.ObjectSelector(ref m_showStateSelections,
				$"Select {nameof(IState)}",
				(selectedState) => StateEditorHelper.ApplyObject(selectedState, m_stateSerializedProperty),
				(state) =>
				{
					if (state is Component component)
						return $"{component.gameObject.GetFullName()}";
					return state.ToString();
				});

			m_conditions.ObjectsSelector(ref m_showConditionSelection,
				ref m_selectedConditions,
				$"Select {nameof(ISwitchStateCondition)}",
				(conditions) =>
				{
					m_conditionsObjectsFieldInfo.SetValue(target, conditions
						.OfType<UnityEngine.Object>()
						.ToArray());
				},
				(condition) => condition.GetType().Name,
				() =>
				{
					var selectedConditions = (m_conditionsObjectsFieldInfo.GetValue(target) as IEnumerable<UnityEngine.Object>)
						.OfType<ISwitchStateCondition>();
					StateEditorHelper.GenerateSelectionList(m_conditions, selectedConditions, ref m_selectedConditions);
				});
		}
	}
}