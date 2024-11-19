using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		private static Type[] m_stateLogicTypes;
		private static List<SearchTreeEntry> m_stateLogicEntries = new List<SearchTreeEntry>();

		private State m_state = null;
		private FieldInfo m_stateLogicList = null;

		private BindingFlags m_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
		private Type[] stateLogicTypes;

		private ReorderableList list = null;
		private SerializedProperty m_stateIDSerializedProperty = null;
		private SerializedProperty m_isStaticSerializedProperty = null;
		private SerializedProperty m_logicSerialziedProperty = null;

		static StateEditor()
		{
			var interfaceType = typeof(IStateLogic);
			var objectType = typeof(Object);
			m_stateLogicTypes = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => !type.IsAbstract && !type.IsInterface && interfaceType.IsAssignableFrom(type) && !type.IsSubclassOf(objectType))
				.ToArray();
		
			GenerateStateLogicEntities();
		}

		private static void GenerateStateLogicEntities()
		{
			m_stateLogicEntries.Add(new SearchTreeGroupEntry(new GUIContent("State logic"), 0));

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
						m_stateLogicEntries.Add(new SearchTreeGroupEntry(new GUIContent(pathItem), i++));
				}

				foreach (var type in item.Value)
				{
					m_stateLogicEntries.Add(new SearchTreeEntry(new GUIContent(type.Name))
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
			
			m_stateIDSerializedProperty = serializedObject.FindProperty("m_stateID");
			m_isStaticSerializedProperty = serializedObject.FindProperty("m_isStatic");
			m_logicSerialziedProperty = serializedObject.FindProperty("m_logic");
			list = new ReorderableList(serializedObject, m_logicSerialziedProperty, true, true, true, true)
			{
				drawElementCallback = DrawElementCallback,
				elementHeightCallback = ElementHeightCallback,
				onAddCallback = OnAddCallback,
				onRemoveCallback = OnRemoveCallback
			};
		}

		private void OnRemoveCallback(ReorderableList reorderableList)
		{
			m_logicSerialziedProperty.DeleteArrayElementAtIndex(reorderableList.index);
			serializedObject.ApplyModifiedProperties();
		}

		private void OnAddCallback(ReorderableList reorderableList)
		{
			var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
			var content = new SearchWindowContext(mousePosition);
			SearchWindow.Open(content, this);
		}

		private float ElementHeightCallback(int index)
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			return EditorGUI.GetPropertyHeight(element, element.isExpanded);
		}

		private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.x += 8;
			rect.width -= 8;
			var typeName = $"{element.managedReferenceValue.GetType().Name} {index}";
			EditorGUI.PropertyField(rect, element, new GUIContent(typeName), element.isExpanded);
			serializedObject.ApplyModifiedProperties();
		}
		
		public override void OnInspectorGUI()
		{
			var iterator = serializedObject.GetIterator();
			iterator.NextVisible(true);
			do
			{
				if (iterator.name == "m_logic")
				{
					list.DoLayoutList();
					continue;
				}
				EditorGUILayout.PropertyField(iterator);
			} 
			while (iterator.NextVisible(false));
			serializedObject.UpdateIfRequiredOrScript();
		}

		private void AddSwitchStateLogic(Type type)
		{
			var index = m_logicSerialziedProperty.arraySize;
			m_logicSerialziedProperty.InsertArrayElementAtIndex(index);
			var element = m_logicSerialziedProperty.GetArrayElementAtIndex(index);
			element.managedReferenceValue = (IStateLogic)Activator.CreateInstance(type);
			serializedObject.ApplyModifiedProperties();
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_stateLogicEntries;

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			if (SearchTreeEntry.userData is not Type type) return false;
			AddSwitchStateLogic(type);
			return true;
		}
	}
}