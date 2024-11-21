using System;
using UnityEditor;
using UnityEngine;

namespace States.Default
{
	[CustomEditor(typeof(StateMachineHost), true)]
	public class StateMachineHostEditor : Editor
	{
		public static Type[] m_executorsTypes = new Type[]
		{
			typeof(OnUpdateStateLogicExecutor),
			typeof(OnLateUpdateStateLogicExecutor),
			typeof(OnFixedUpdateStateLogicExecutor),
		};

		private StateMachineHost m_stateMachineManager = null;
		protected virtual void OnEnable() => m_stateMachineManager = (StateMachineHost)target;

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

		}
		
	}
}