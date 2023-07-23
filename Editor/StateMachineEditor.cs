using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.States
{
	public class StateMachineEditor : Editor
	{
		private FieldInfo m_logicExecutorProperty = null;
		private FieldInfo m_stateTransitionProperty = null;
		private FieldInfo m_stateProcessorsProperty = null;
		private FieldInfo m_defaultStateSetterProperty = null;

		private IEnumerable<IStateLogicExecutor> m_logicExecutors = null;
		private IEnumerable<IStateTransitionLogic> m_stateTransitions = null;
		private IEnumerable<IStatePreProcessor> m_statePreProcessors = null;
		private IEnumerable<StateSetter> m_stateSetters = null;

		private bool m_showLogicExecutorsSelection = false;
		private bool m_showStateTransitionsSelection = false;
		private bool m_showStatePreProcessorSelection = false;
		private bool m_showStateSettersSelection = false;

		private bool[] m_logicExecutorsSelection = Array.Empty<bool>();
		private bool[] m_stateTransitionsSelection = Array.Empty<bool>();
		private bool[] m_statePreProcessorSelection = Array.Empty<bool>();

		protected virtual void OnEnable()
		{
			var targetType = target.GetType();
			var bindingsFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			m_logicExecutorProperty = targetType.GetField("m_logicExecutor", bindingsFlags);
			m_stateTransitionProperty = targetType.GetField("m_stateTransition", bindingsFlags);
			m_stateProcessorsProperty = targetType.GetField("m_stateProcessors", bindingsFlags);
			m_defaultStateSetterProperty = targetType.GetField("m_defaultStateSetter", bindingsFlags);

			var components = target as Component;
			var root = components.transform.root.gameObject;
			m_logicExecutors = root.GetComponentsInChildren<IStateLogicExecutor>();
			m_stateTransitions = root.GetComponentsInChildren<IStateTransitionLogic>();
			m_statePreProcessors = root.GetComponentsInChildren<IStatePreProcessor>();
			m_stateSetters = root.GetComponentsInChildren<StateSetter>();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			m_logicExecutors.ObjectsSelector(ref m_showLogicExecutorsSelection,
				ref m_logicExecutorsSelection,
				$"Select {nameof(IStateLogicExecutor)}",
				(selectedObjects) =>
				{
					m_logicExecutorProperty.SetValue(target, selectedObjects
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
				()=>
				{
					var selected = (m_logicExecutorProperty.GetValue(target) as IEnumerable<UnityEngine.Object>)
						.OfType<IStateLogicExecutor>();
					StateEditorHelper.GenerateSelectionList(m_logicExecutors, selected, ref m_logicExecutorsSelection);
				});

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
					var selected = (m_logicExecutorProperty.GetValue(target) as IEnumerable<UnityEngine.Object>)
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