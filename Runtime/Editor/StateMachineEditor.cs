using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.States.Default
{
	[CustomEditor(typeof(StateMachineManager), true)]
	public class StateMachineEditor : Editor
	{
		public static Type[] m_executorsTypes = new Type[]
		{
			typeof(OnUpdateStateLogicExecutor),
			typeof(OnLateUpdateStateLogicExecutor),
			typeof(OnFixedUpdateStateLogicExecutor),
		};

		private StateMachineManager m_stateMachineManager = null;

		private FieldInfo m_defaultStateSetterProperty = null;

		private IEnumerable<StateSetter> m_stateSetters = null;

		private bool m_showStateSettersSelection = false;

		private bool[] m_statePreProcessorSelection = Array.Empty<bool>();

		protected virtual void OnEnable()
		{
			m_stateMachineManager = (StateMachineManager)target;

			var targetType = typeof(StateMachineManager);
			var bindingsFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			m_defaultStateSetterProperty = targetType.GetField("m_defaultStateSetter", bindingsFlags);

			var components = target as Component;
			var root = components.transform.root.gameObject;
			m_stateSetters = root.GetComponentsInChildren<StateSetter>();
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

			EditorGUILayout.Space();

			m_stateSetters.ObjectSelector(ref m_showStateSettersSelection,
				$"Select Default {nameof(StateSetter)}",
				(StateSetter setter) =>
				{
					m_defaultStateSetterProperty.SetValue(target, setter);
					serializedObject.UpdateIfRequiredOrScript();
				},
				(setter) => string.IsNullOrEmpty(setter.Description) ? setter.GetType().Name : setter.Description);

		}
	}
}