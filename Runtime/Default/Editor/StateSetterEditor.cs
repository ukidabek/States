using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(StateSetter))]
	public class StateSetterEditor : Editor, ISearchWindowProvider
	{
		private static List<SearchTreeEntry> m_entries = new List<SearchTreeEntry>();
		private StateSetter m_stateSetter = null;
		private IEnumerable<IStateMachine> m_statesMachines = null;
		private SerializedProperty m_objectProperty = null;

		private void OnEnable()
		{
			m_objectProperty = serializedObject.FindProperty("_stateMachineObject");
			m_stateSetter = (target as StateSetter);
			m_statesMachines = m_stateSetter.GetComponentsFormRoot<IStateMachine>();
			m_entries.Clear();
			m_entries.Add(new SearchTreeGroupEntry(new GUIContent($"{nameof(IStateMachine)}'s"), 0));
			foreach (var stateMachine in m_statesMachines)
			{
				m_entries.Add(new SearchTreeEntry(new GUIContent(stateMachine.Name))
				{
					level = 1,
					userData = stateMachine,
				});
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button($"Select {nameof(IStateMachine)}"))
			{
				var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				var content = new SearchWindowContext(mousePosition);
				SearchWindow.Open(content, this);
			}
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_entries;

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			if (SearchTreeEntry.userData is Object unityObjectStateMachine)
			{
				m_objectProperty.objectReferenceValue = unityObjectStateMachine;
				serializedObject.ApplyModifiedProperties();
				return true;
			}
			return false;
		}
	}
}