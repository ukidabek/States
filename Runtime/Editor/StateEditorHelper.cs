using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities.States.Default
{
	internal static class StateEditorHelper
	{
		public static IEnumerable<T> GetComponentsFormRoot<T>(this Component component)
		{
			var rootGameObject = component.transform.root;
			return rootGameObject.GetComponentsInChildren<T>();
		}

		public static void ApplyObject<T>(T objectToSet, SerializedProperty serializedProperty)
		{
			if (objectToSet != null && objectToSet is UnityEngine.Object stateMachineObject)
			{
				serializedProperty.objectReferenceValue = stateMachineObject;
				serializedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		public static string GetFullName(this GameObject gameObject)
		{
			string GetFullName(GameObject gameObject, string name)
			{
				
				if(gameObject.transform.parent != null) 
				{
					name += $"/{gameObject.name}";
					var nextGameObject = gameObject.transform.parent.gameObject;
					name = GetFullName(nextGameObject, name);
				}
				else
					name += gameObject.name;

				return name;
			}

			string fullName = string.Empty;
			return GetFullName(gameObject, fullName);
		}

		public static void ObjectSelector<T>(this IEnumerable<T> @object, ref bool show, string text, Action<T> select, Func<T, string> getName = null, Action onOpen = null)
		{
			if (getName == null) 
				getName = (T item) => item.ToString();

			if (show)
			{
				foreach (var stateMachine in @object)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(getName(stateMachine));
					if (GUILayout.Button("Select", GUILayout.Width(45)))
					{
						show = false;
						select?.Invoke(stateMachine);
					}
					EditorGUILayout.EndHorizontal();
				}

				if (GUILayout.Button("Cancel"))
					show = false;
			}
			else
			{
				if (GUILayout.Button(text))
				{
					onOpen?.Invoke();
					show = true;
				}
			}
		}

		public static void GenerateSelectionList<T>(IEnumerable<T> all, IEnumerable<T> selected, ref bool[] selection)
		{
			var count = all.Count();
			if(count != selection.Length)
				selection = new bool[count];

			var set = selected.ToHashSet();
			var index = 0;
			foreach (var item in all)
				selection[index++] = set.Contains(item);
		}

		public static void ObjectsSelector<T>(this IEnumerable<T> objects, ref bool show, ref bool[] selection, string text, Action<IEnumerable<T>> onSelected, Func<T, string> getName = null, Action onOpen = null)
		{
			if (getName == null)
				getName = (T item) => item.ToString();

			if (show)
			{
				var count = objects.Count();
				if(selection.Length != count)
					selection = new bool[count];

				int index = 0;
				foreach (var @object in objects)
				{
					EditorGUILayout.BeginHorizontal();
					selection[index] = GUILayout.Toggle(selection[index++], getName(@object));
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Select"))
				{
					var selectedObjects = new List<T>();
					index = 0;
                    foreach (var item in objects)
                    {
						var status = selection[index];
						if (status)
							selectedObjects.Add(item);
						selection[index++] = false;
					}
					onSelected?.Invoke(selectedObjects);
					show = false;
				}

				if (GUILayout.Button("Cancel"))
					show = false;

				EditorGUILayout.EndHorizontal();
			}
			else
			{
				if (GUILayout.Button(text))
				{
					onOpen?.Invoke();
					show = true;
				}
			}
		}
	}
}