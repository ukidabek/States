using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(State))]
	public class StateEditor : Editor, ISearchWindowProvider
	{
		private static Type[] m_stateLogicTypes;
		private static List<SearchTreeEntry> m_entries = new List<SearchTreeEntry>();

		private State m_state = null;
		private FieldInfo m_stateLogicList = null;

		private BindingFlags m_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

		static StateEditor()
		{
			m_stateLogicTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => !type.IsAbstract && !type.IsInterface && type.GetInterface(nameof(IStateLogic)) != null)
				.ToArray();
		
			m_entries.Add(new SearchTreeGroupEntry(new GUIContent("State logic"), 0));

			var groupDictionary = new Dictionary<string, List<Type>>();

			foreach (var type in m_stateLogicTypes)
			{
				var path = string.Empty;

				var addComponentAttribute = type.GetCustomAttributes(true)
					.OfType<StateLogicPath>()
					.FirstOrDefault();

				if (addComponentAttribute != null)
				{
					var pathSegments = addComponentAttribute.Path.Split("/");
					path = string.Join("/", pathSegments);
				}

				if(groupDictionary.TryGetValue(path, out var list))
					list.Add(type);
				else
				{
					list = new List<Type>
					{
						type
					};
					groupDictionary.Add(path, list);
				}
			}

			foreach (var item in groupDictionary.OrderBy(pair => pair.Key.Length))
			{
				var i = 1;
				if (!string.IsNullOrEmpty(item.Key))
				{
					var path = item.Key.Split("/");
					foreach (var pathItem in path)
						m_entries.Add(new SearchTreeGroupEntry(new GUIContent(pathItem), i++));
				}

				foreach (var type in item.Value)
				{
					m_entries.Add(new SearchTreeEntry(new GUIContent(type.Name))
					{
						level = i,
						userData = type
					});
				}
			}
		}

		private void OnEnable()
		{
			m_state = target as State;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button($"Get all {nameof(IStateLogic)}"))
			{
				GetAllStateLogic();
			}

			if (GUILayout.Button("Add state logic"))
			{
				var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				var content = new SearchWindowContext(mousePosition);
				SearchWindow.Open(content, this);
			}
		}

		private void GetAllStateLogic()
		{
			var stateLogic = m_state
				.GetComponents<IStateLogic>()
				.OfType<Object>();

			if (m_stateLogicList == null)
			{
				m_stateLogicList = target
					.GetType()
					.GetField("m_logic", m_bindingFlags);
			}

			var stateLogicListObject = m_stateLogicList.GetValue(m_state) as Object[] ?? Array.Empty<Object>();
			var currentStateLogicSet = stateLogicListObject.Where(stateLogic => stateLogic != null);
			var exception = stateLogic.Except(currentStateLogicSet);
			var newList = currentStateLogicSet.Concat(exception).OfType<Object>().ToArray();

			m_stateLogicList.SetValue(m_state, newList);
			EditorUtility.SetDirty(m_state);
		}

		private void AddSwitchStateLogic(Type type) => m_state.gameObject.AddComponent(type);

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_entries;

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			if (SearchTreeEntry.userData != null && SearchTreeEntry.userData is Type type)
			{
				AddSwitchStateLogic(type);
				GetAllStateLogic();
				return true;
			}

			return false;
		}
	}
}