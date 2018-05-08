using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    [CustomEditor(typeof(StateGraph), true)]
    public class BaseStateGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Open editor"))
            {
                StateGraphEditorWindow window = Editor.CreateInstance<StateGraphEditorWindow>();
                window.Initialize(target as StateGraph);
                window.Show();
            }
        }
    }

}