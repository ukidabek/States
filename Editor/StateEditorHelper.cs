using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.States
{
	internal static class StateEditorHelper
	{
		public static IEnumerable<T> GetComponentsFormRoot<T>(this Component component)
		{
			var rootGameObject = component.transform.root;
			return rootGameObject.GetComponentsInChildren<T>();
		}


		public static T ObjectSelector<T>(this IEnumerable<T> stateMachines, ref bool show, string text, Func<T, string> getName = null)
		{
			if (getName == null) 
				getName = (T item) => item.ToString();

			if(show)
			{
				foreach (var stateMachine in stateMachines)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(getName(stateMachine));
					if (GUILayout.Button("Select", GUILayout.Width(45)))
					{
						show = false;
						return stateMachine;
					}
					EditorGUILayout.EndHorizontal();
				}

				if (GUILayout.Button("Cancel"))
					show = false;
			}
			else
			{
				if (GUILayout.Button(text))
					show = true;
			}
			return default;
		}
	}
}