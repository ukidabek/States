using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.States
{
	[CustomEditor(typeof(State))]
	public class StateEditor : Editor
	{
		private State m_state = null;
		private FieldInfo m_stateLogicList = null;

		private BindingFlags m_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

		private void OnEnable()
		{
			m_state = target as State;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Get all state logic components"))
			{
				var stateLogic = m_state
					.GetComponents<IStateLogic>()
					.OfType<Object>();

				if(m_stateLogicList == null)
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
		}
	}
}