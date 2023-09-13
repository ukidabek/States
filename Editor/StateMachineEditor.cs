using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.States
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

		private FieldInfo m_stateTransitionProperty = null;
		private FieldInfo m_stateProcessorsProperty = null;
		private FieldInfo m_defaultStateSetterProperty = null;

		private IEnumerable<IStateTransitionLogic> m_stateTransitions = null;
		private IEnumerable<IStatePreProcessor> m_statePreProcessors = null;
		private IEnumerable<StateSetter> m_stateSetters = null;

		private bool m_showStateTransitionsSelection = false;
		private bool m_showStatePreProcessorSelection = false;
		private bool m_showStateSettersSelection = false;

		private bool[] m_logicExecutorsSelection = Array.Empty<bool>();
		private bool[] m_stateTransitionsSelection = Array.Empty<bool>();
		private bool[] m_statePreProcessorSelection = Array.Empty<bool>();

		protected virtual void OnEnable()
		{
			m_stateMachineManager = (StateMachineManager)target;

			var targetType = typeof(StateMachineManager);
			var bindingsFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			m_stateTransitionProperty = targetType.GetField("m_stateTransition", bindingsFlags);
			m_stateProcessorsProperty = targetType.GetField("m_stateProcessors", bindingsFlags);
			m_defaultStateSetterProperty = targetType.GetField("m_defaultStateSetter", bindingsFlags);

			var components = target as Component;
			var root = components.transform.root.gameObject;
			m_stateTransitions = root.GetComponentsInChildren<IStateTransitionLogic>();
			m_statePreProcessors = root.GetComponentsInChildren<IStatePreProcessor>();
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
				int length = enumValues.Length;
				for (int i = 0; i < length; i++)
				{
					var type = m_executorsTypes[i];
					var executor = gameObject.GetComponent(type);

					bool shouldExist = (1 & executors >> i) == 1;

					if (shouldExist && executor == null)
						gameObject.AddComponent(type);

					if (!shouldExist && executor != null)
						GameObject.DestroyImmediate(executor, true);
				}
			}

			EditorGUILayout.Space();

			m_stateTransitions.ObjectsSelector(ref m_showStateTransitionsSelection,
				ref m_stateTransitionsSelection,
				$"Select {nameof(IStateTransitionLogic)}",
				(selectedObjects) =>
				{
					m_stateTransitionProperty.SetValue(target, selectedObjects
						.OfType<UnityEngine.Object>()
						.ToArray());
					serializedObject.UpdateIfRequiredOrScript();
				},
				(executor) =>
				{
					if (executor is Component component)
						return $"{component.gameObject.GetFullName()}/{executor.GetType().Name}";
					return string.Empty;
				},
				() =>
				{
					var selected = (m_stateTransitionProperty.GetValue(target) as IEnumerable<UnityEngine.Object>)
						.OfType<IStateTransitionLogic>();
					StateEditorHelper.GenerateSelectionList(m_stateTransitions, selected, ref m_stateTransitionsSelection);
				});

			m_statePreProcessors.ObjectsSelector(ref m_showStatePreProcessorSelection,
				ref m_statePreProcessorSelection,
				$"Select {nameof(IStatePreProcessor)}",
				(selectedObjects) =>
				{
					m_stateProcessorsProperty.SetValue(target, selectedObjects
						.OfType<UnityEngine.Object>()
						.ToArray());
					serializedObject.UpdateIfRequiredOrScript();
				},
				(executor) =>
				{
					if (executor is Component component)
						return $"{component.gameObject.GetFullName()}/{executor.GetType().Name}";
					return string.Empty;
				},
				() =>
				{
					var selected = (m_stateProcessorsProperty.GetValue(target) as IEnumerable<UnityEngine.Object>)
						.OfType<IStatePreProcessor>();
					StateEditorHelper.GenerateSelectionList(m_statePreProcessors, selected, ref m_statePreProcessorSelection);
				});

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