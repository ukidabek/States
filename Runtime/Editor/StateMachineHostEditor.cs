using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(StateMachineHost), true)]
	public class StateMachineHostEditor : Editor, ISearchWindowProvider
	{
		private static List<SearchTreeEntry> m_entries = new List<SearchTreeEntry>();
		public static Type[] m_executorsTypes = new Type[]
		{
			typeof(OnUpdateStateLogicExecutor),
			typeof(OnLateUpdateStateLogicExecutor),
			typeof(OnFixedUpdateStateLogicExecutor),
		};

		private StateMachineHost m_stateMachineManager = null;
		private FieldInfo m_defaultStateSetterProperty = null;
		private IEnumerable<StateSetter> m_stateSetters = null;
		private bool[] m_statePreProcessorSelection = Array.Empty<bool>();

		protected virtual void OnEnable()
		{
			m_stateMachineManager = (StateMachineHost)target;

			var targetType = typeof(StateMachineHost);
			var bindingsFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			m_defaultStateSetterProperty = targetType.GetField("m_defaultStateSetter", bindingsFlags);

			var components = target as Component;
			var root = components.transform.root.gameObject;
			m_stateSetters = root.GetComponentsInChildren<StateSetter>();

			m_entries.Clear();
			m_entries.Add(new SearchTreeGroupEntry(new GUIContent($"{nameof(StateSetter)}'s"), 0));
			foreach (var setter in m_stateSetters)
			{
				var name = setter.gameObject.GetFullName();
				if (setter.State != null)
					name = $"{name}/{setter.State.Name}";

				m_entries.Add(new SearchTreeEntry(new GUIContent(name))
				{
					level = 1,
					userData = setter,
				});
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck())
			{
				var gameObject = m_stateMachineManager.gameObject;
				var enumValues = Enum.GetValues(typeof(Executor));
				var executors = (uint)m_stateMachineManager.Executor;
				var length = enumValues.Length;
				for (int i = 0; i < length; i++)
				{
					var type = m_executorsTypes[i];
					var executor = gameObject.GetComponent(type);
					var shouldExist = (1 & executors >> i) == 1;

					if (shouldExist && executor == null)
						gameObject.AddComponent(type);

					if (!shouldExist && executor != null)
						GameObject.DestroyImmediate(executor, true);
				}
			}

			if (GUILayout.Button($"Select {nameof(StateSetter)}"))
			{
				var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				var content = new SearchWindowContext(mousePosition);
				SearchWindow.Open(content, this);
			}
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_entries;

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			if (!(SearchTreeEntry.userData is StateSetter setter)) return false;

			m_defaultStateSetterProperty.SetValue(target, setter);
			serializedObject.UpdateIfRequiredOrScript();

			return true;
		}
	}
}