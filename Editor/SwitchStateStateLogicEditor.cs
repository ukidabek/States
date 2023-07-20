using System.Collections.Generic;
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
		private IEnumerable<IStateMachine> m_stateMachines = null;
		private IEnumerable<IState> m_states = null;
		private bool m_showStateMachines = false;
		private bool m_showStateSelections = false;

		private SerializedProperty m_stateMachineSerializedProperty = null;
		private SerializedProperty m_stateSerializedProperty = null;

		private void OnEnable()
		{
			m_switchStateStateLogic = (target as SwitchStateStateLogic);
			m_stateFieldInfo = m_switchStateStateLogic
				.GetType()
				.GetField("_stateToEnter", BindingFlags.Instance | BindingFlags.NonPublic);
			m_stateMachines = m_switchStateStateLogic.GetComponentsFormRoot<IStateMachine>();
			m_states = m_switchStateStateLogic.GetComponentsFormRoot<IState>();
			m_stateMachineSerializedProperty = serializedObject.FindProperty("_stateMachineInstance");
			m_stateSerializedProperty = serializedObject.FindProperty("_stateToEnter");
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

			var selectedStateMachine = m_stateMachines.ObjectSelector(ref m_showStateMachines, "Select state machine",
				stateMachine => stateMachine.Name);
			ApplyObject(selectedStateMachine, m_stateMachineSerializedProperty);

			var selectedState = m_states.ObjectSelector(ref m_showStateSelections, "Select state",
				state => state is Object unityObject ? unityObject.name : state.ToString());
			ApplyObject(selectedState, m_stateSerializedProperty);

			if (GUILayout.Button("Get Conditions"))
			{
				m_switchStateStateLogic.GetConditions();
				EditorUtility.SetDirty(target);
			}
		}

		private void ApplyObject<T>(T onjectToSet, SerializedProperty serializedProperty)
		{
			if (onjectToSet != null && onjectToSet is Object stateMachineObject)
			{
				serializedProperty.objectReferenceValue = stateMachineObject;
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}