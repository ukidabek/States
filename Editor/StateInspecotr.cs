using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    public class StateInspecotr : BaseStateGraphInspector
    {
        private Node _selectedNode = null;
        private StateGraph _stateGraph;
        private GenericMenu addConditionGenericMenu = null;

        private TransitionConditionsInspector transitionConditionsInspector = new TransitionConditionsInspector();

        public override void DrawInspector()
        {
            if (_selectedNode != null && _selectedNode.State != null)
            {
                _selectedNode.BacgroundColor = EditorGUILayout.ColorField(_selectedNode.BacgroundColor);
                DrawInspectorArea(_selectedNode.State);

                EditorGUILayout.Space();

                if (_stateGraph.Type == GraphType.Stack)
                {
                    for (int i = 0; i < _selectedNode.State.ExitStateTransitions.Count; i++)
                    {
                        var item = _selectedNode.State.ExitStateTransitions[i];
                        GUIStyle style = new GUIStyle();
                        style.fontStyle = FontStyle.Bold;
                        EditorGUILayout.LabelField(string.Format("Exit state transition {0}", i), style);

                        if (GUILayout.Button("Remove transition"))
                        {
                            foreach (var condition in _selectedNode.State.ExitStateTransitions[i].Conditions)
                            {
                                GameObject.DestroyImmediate(condition);
                            }
                            _selectedNode.State.ExitStateTransitions.RemoveAt(i--);
                        }

                        transitionConditionsInspector.DrawInspector(item.Conditions, _selectedNode.State, addConditionGenericMenu, i);
                    }
                    if(GUILayout.Button("Add exit state transition"))
                    {
                        _selectedNode.State.ExitStateTransitions.Add(new ExitStateTransition());
                    }
                }
            }
        }

        public override void SetData(params object[] data)
        {
            if (data.Length > 0 && data[0] != null && data[0] is Node)
                _selectedNode = data[0] as Node;

            if (data.Length > 1 && data[1] != null && data[1] is StateGraph)
                _stateGraph = data[1] as StateGraph;

            if (data.Length > 2 && data[2] != null && data[2] is GenericMenu)
                addConditionGenericMenu = data[2] as GenericMenu;
        }
    }
}