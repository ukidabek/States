using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(SwitchStateStateLogic))]
	public class SwitchStateStateLogicEditor : Editor, ISearchWindowProvider
	{
		private enum EntriesType
		{
			State,
			StateMachine,
		}

		private EntriesType m_entriesType = EntriesType.State;

		private SwitchStateStateLogic m_switchStateStateLogic = null;
		private FieldInfo m_conditionsObjectsFieldInfo = null;

		private IEnumerable<IStateMachine> m_stateMachines = null;
		private IEnumerable<IState> m_states = null;
		private IEnumerable<ISwitchStateCondition> m_conditions = null;

		private SerializedProperty m_stateMachineSerializedProperty = null;
		private SerializedProperty m_stateSerializedProperty = null;

		private static List<SearchTreeEntry> m_statesEntries = new List<SearchTreeEntry>();
		private static List<SearchTreeEntry> m_stateMachinesEntries = new List<SearchTreeEntry>();
		private static List<SearchTreeEntry> m_conditionsEntries = new List<SearchTreeEntry>();

		private void OnEnable()
		{
			m_switchStateStateLogic = (target as SwitchStateStateLogic);
			var type = m_switchStateStateLogic.GetType();
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

			m_conditionsObjectsFieldInfo = type.GetField("m_conditionsObjects", bindingFlags);
			m_stateMachineSerializedProperty = serializedObject.FindProperty("m_stateMachineInstance");
			m_stateSerializedProperty = serializedObject.FindProperty("m_stateToEnter");

			m_states = m_switchStateStateLogic.GetComponentsFormRoot<IState>();
			m_stateMachines = m_switchStateStateLogic.GetComponentsFormRoot<IStateMachine>();
			m_conditions = m_switchStateStateLogic.GetComponents<ISwitchStateCondition>();

			StateEditorHelper.GenerateEntries(
				m_statesEntries,
				m_states,
				$"Select {nameof(IState)}",
				(state) =>
				{
					var entry = new SearchTreeEntry(new GUIContent(state.Name));
					entry.userData = state;
					return entry;
				});

			StateEditorHelper.GenerateEntries(
				m_stateMachinesEntries,
				m_stateMachines,
				$"Select {nameof(IStateMachine)}",
				(stateMachine) =>
				{
					var entry = new SearchTreeEntry(new GUIContent(string.Empty));
					if (stateMachine is Component component)
						entry.content = new GUIContent($"{component.gameObject.GetFullName()}");
					else
						entry.content = new GUIContent(stateMachine.Name);
					entry.userData = stateMachine;
					return entry;
				});
		}

		public override void OnInspectorGUI()
		{
			var state = m_stateSerializedProperty.objectReferenceValue as State;
			if (state != null)
			{
				var oldColor = GUI.color;
				GUI.color = Color.yellow;
				GUILayout.Label(state.name);
				GUI.color = oldColor;
			}

			base.OnInspectorGUI();

			var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			var content = new SearchWindowContext(mousePosition);

			if (GUILayout.Button($"Select {nameof(IStateMachine)}"))
			{
				m_entriesType = EntriesType.StateMachine;
				SearchWindow.Open(content, this);
			}

			if (GUILayout.Button($"Select {nameof(IState)}"))
			{
				m_entriesType = EntriesType.State;
				SearchWindow.Open(content, this);
			}

			if (GUILayout.Button($"Find {nameof(ISwitchStateCondition)}"))
			{
				FillConditionReferences();
			}
		}

		private void FillConditionReferences()
		{
			var components = m_switchStateStateLogic.GetComponents<Component>();
			var lenght = components.Length;
			if (lenght == 0) return;

			var componentsToAdd = new List<Component>();
			var beginCollecting = false;
			
			for (int i = 0; i < lenght; i++)
			{
				if (!beginCollecting)
					beginCollecting = components[i] == target;
				else
				{
					if (components[i] is ISwitchStateCondition)
						componentsToAdd.Add(components[i]);
					else
						break;
				}
			}

			var componentsArray = componentsToAdd.OfType<UnityEngine.Object>().ToArray();
			m_conditionsObjectsFieldInfo.SetValue(target, componentsArray);
			serializedObject.ApplyModifiedProperties();
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			switch (m_entriesType)
			{
				case EntriesType.State:
					return m_statesEntries;
				case EntriesType.StateMachine:
					return m_stateMachinesEntries;
				default:
					return null;
			}
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			switch (m_entriesType)
			{
				case EntriesType.State:
					m_stateSerializedProperty.ApplyObject(SearchTreeEntry.userData);
					return true;
				case EntriesType.StateMachine:
					m_stateMachineSerializedProperty.ApplyObject(SearchTreeEntry.userData);
					return true;
			}

			return false;
		}
	}
}