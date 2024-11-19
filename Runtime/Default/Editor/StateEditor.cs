using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(State))]
	public class StateEditor : Editor, ISearchWindowProvider
	{
		private enum Types
		{
			Transitions,
			StateLogic,
		}
		private Types m_typesToReturn = Types.StateLogic;
		
		private static Type[] m_stateLogicTypes = null;
		private static Type[] m_stateTransitionsTypes = null;
		
		private static List<SearchTreeEntry> m_stateLogicEntries = new List<SearchTreeEntry>();
		private static List<SearchTreeEntry> m_stateTransitionsEntries = new List<SearchTreeEntry>();
		
		private SerializedProperty m_logicSerializedProperty = null;
		private ReorderableList m_statesLogicList = null;
		private ReorderableList m_stateTransitionsList = null;
		private SerializedProperty m_stateTransitionsProperty;

		static StateEditor()
		{
			var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
			m_stateLogicTypes = allTypes.Where(ValidateType<IStateLogic>).ToArray();
			m_stateTransitionsTypes = allTypes.Where(ValidateType<IStateTransition>).ToArray();
			GenerateStateLogicEntities(m_stateLogicEntries, m_stateLogicTypes, "State logic");
			GenerateStateLogicEntities(m_stateTransitionsEntries, m_stateTransitionsTypes, "State tramsitions");
		}

		private static bool ValidateType<T>(Type type)
		{
			if (type.IsAbstract) return false;
			if (type.IsInterface) return false;
			if (!typeof(T).IsAssignableFrom(type)) return false;
			if(type.IsSubclassOf(typeof(Object))) return false;
			return true;
		}
		
		private static void GenerateStateLogicEntities(IList<SearchTreeEntry> entries, IEnumerable<Type> types, string title)
		{
			entries.Add(new SearchTreeGroupEntry(new GUIContent(title), 0));
			var groupDictionary = new Dictionary<string, List<Type>>();

			foreach (var type in types)
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
						entries.Add(new SearchTreeGroupEntry(new GUIContent(pathItem), i++));
				}

				foreach (var type in item.Value)
				{
					entries.Add(new SearchTreeEntry(new GUIContent(type.Name))
					{
						level = i,
						userData = type
					});
				}
			}
		}

		private void OnEnable()
		{
			m_logicSerializedProperty = serializedObject.FindProperty("m_logic");
			m_stateTransitionsProperty = serializedObject.FindProperty("m_transition");
			
			m_statesLogicList = new ReorderableList(serializedObject, m_logicSerializedProperty, true, true, true, true)
			{
				drawHeaderCallback = rect => EditorGUI.LabelField(rect, "States"),
				drawElementCallback = (rect, index, _, _) =>
				{
					var element = m_statesLogicList.serializedProperty.GetArrayElementAtIndex(index);
					rect.x += 8;
					rect.width -= 8;
					var typeName = $"{element.managedReferenceValue.GetType().Name} {index}";
					EditorGUI.PropertyField(rect, element, new GUIContent(typeName), element.isExpanded);
					serializedObject.ApplyModifiedProperties();
				},
				elementHeightCallback = index =>
				{
					var element = m_statesLogicList.serializedProperty.GetArrayElementAtIndex(index);
					return EditorGUI.GetPropertyHeight(element, element.isExpanded);
				},
				onAddCallback = reorderableList =>
				{
					m_typesToReturn = Types.StateLogic;
					OpenSearchWindow();
				},
				onRemoveCallback = RemoveItem,
			};
			
			m_stateTransitionsList = new ReorderableList(serializedObject, m_stateTransitionsProperty, true, true, true, true)
			{
				drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Transitions"),
				drawElementCallback = (rect, index, _, _) =>
				{
					var element = m_stateTransitionsList.serializedProperty.GetArrayElementAtIndex(index);
					rect.x += 8;
					rect.width -= 8;
					var typeName = $"{element.managedReferenceValue.GetType().Name} {index}";
					EditorGUI.PropertyField(rect, element, new GUIContent(typeName), element.isExpanded);
					serializedObject.ApplyModifiedProperties();
				},
				elementHeightCallback = index =>
				{
					var element = m_stateTransitionsList.serializedProperty.GetArrayElementAtIndex(index);
					return EditorGUI.GetPropertyHeight(element, element.isExpanded);
				},
				onAddCallback = _ =>
				{
					m_typesToReturn = Types.Transitions;
					OpenSearchWindow();
				},
				onRemoveCallback = RemoveItem,
			};
		}

		private void OpenSearchWindow()
		{
			var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			var content = new SearchWindowContext(mousePosition);
			SearchWindow.Open(content, this);
		}

		private void RemoveItem(ReorderableList list)
		{
			list.serializedProperty.DeleteArrayElementAtIndex(list.index);
			serializedObject.ApplyModifiedProperties();
		}


		public override void OnInspectorGUI()
		{
			var iterator = serializedObject.GetIterator();
			iterator.NextVisible(true);
			do
			{
				switch (iterator.name)
				{
					case "m_logic":
						m_statesLogicList.DoLayoutList();
						continue;
					case "m_transition":
						m_stateTransitionsList.DoLayoutList();
						continue;
					default:
						EditorGUILayout.PropertyField(iterator);
						serializedObject.ApplyModifiedProperties();
						continue;
				}
			} while (iterator.NextVisible(false));

			serializedObject.UpdateIfRequiredOrScript();
		}

		private void AddSwitchStateLogic<T>(Type type, SerializedProperty property)
		{
			var index = property.arraySize;
			property.InsertArrayElementAtIndex(index);
			var element = property.GetArrayElementAtIndex(index);
			element.managedReferenceValue = (T)Activator.CreateInstance(type);
			serializedObject.ApplyModifiedProperties();
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) =>
			m_typesToReturn switch
			{
				Types.Transitions => m_stateTransitionsEntries,
				Types.StateLogic => m_stateLogicEntries,
			};

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			if (SearchTreeEntry.userData is not Type type) return false;

			switch (m_typesToReturn)
			{
				case Types.Transitions:
					AddSwitchStateLogic<IStateTransition>(type, m_stateTransitionsProperty);
					break;
				case Types.StateLogic:
					AddSwitchStateLogic<IStateLogic>(type, m_logicSerializedProperty);
					break;
				default:
					return false;
			}
			
			return true;
		}
	}
}