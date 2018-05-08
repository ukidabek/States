using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

namespace BaseGameLogic.States
{

    public class TransitionConditionsInspector : BaseStateGraphInspector
    {
        private List<BaseStateTransitionCondition> _conditions = null;
        private GenericMenu _addContitionContextMenu = null;
        private BaseState _baseState = null;
        private int _selectetExitStateTransitionIndex = -1;

        public override void DrawInspector()
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Transition conditions", style);

            for (int i = 0; i < _conditions.Count; i++)
            {
                var item = _conditions[i];
                if (item == null)
                {
                    _conditions.RemoveAt(i);
                    --i;
                    continue;
                }

                DrawInspectorArea(item);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add condition"))
            {
                if(_baseState != null)
                    _baseState.SelectedExitStateTransition = _selectetExitStateTransitionIndex;
                _addContitionContextMenu.ShowAsContext();
            }
        }

        public override void SetData(params object[] data)
        {
            if (data.Length > 0 && data[0] != null && data[0] is List<BaseStateTransitionCondition>)
                _conditions = data[0] as List<BaseStateTransitionCondition>;

            if (data.Length > 1 && data[1] != null && data[1] is BaseState)
                _baseState = data[1] as BaseState;

            if (data.Length > 2 && data[2] != null && data[2] is GenericMenu)
                _addContitionContextMenu = data[2] as GenericMenu;

            if (data.Length > 3 && data[3] != null && data[3] is int)
                _selectetExitStateTransitionIndex = (int)data[3];
            else
                _selectetExitStateTransitionIndex = -1;
        }
    }
}