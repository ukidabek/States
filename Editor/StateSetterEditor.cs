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
			m_statesMachines.ObjectSelector(ref m_show, 
				$"Select {nameof(IStateMachine)}",
				(stateMachine) =>
				{
					if (stateMachine is Object unityObjectStateMachine)
					{
						m_objectProperty.objectReferenceValue = unityObjectStateMachine;
						serializedObject.ApplyModifiedProperties();
					}
				},
				(stateMachine) =>
				{
					if (stateMachine is Component component)
						return $"{component.gameObject.GetFullName()}/{stateMachine.Name}";
					return stateMachine.Name;
				});
		}
	}
}