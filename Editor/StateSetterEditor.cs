using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.States
{
	[CustomEditor(typeof(StateSetter))]
	public class StateSetterEditor : Editor
	{
		private StateSetter m_stateSetter = null;
		private IEnumerable<IStateMachine> m_statesMachines = null;
		private SerializedProperty m_objectProperty = null;
		private bool m_show = false;

		private void OnEnable()
		{
			m_objectProperty = serializedObject.FindProperty("_stateMachineObject");
			m_stateSetter = (target as StateSetter);
			m_statesMachines = m_stateSetter.GetComponentsFormRoot<IStateMachine>();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var selectedStateMachine = m_statesMachines.ObjectSelector(ref m_show, "Select state machine",
				stateMachine => stateMachine.Name);

			if(selectedStateMachine != null && selectedStateMachine is Object unityObjectStateMachine)
			{
				m_objectProperty.objectReferenceValue = unityObjectStateMachine;
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}