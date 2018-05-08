using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.States
{
    public abstract class BaseStateGraphInspector
    {
        public abstract void DrawInspector();

        public abstract void SetData(params object[] data);

        public void DrawInspector(params object[] data)
        {
            SetData(data);
            DrawInspector();
        }

        protected void DrawInspectorArea(MonoBehaviour item)
        {
            if (item == null) return;

            Editor editor = Editor.CreateEditor(item);
            EditorGUILayout.InspectorTitlebar(false, item);
            editor.OnInspectorGUI();
        }
    }
}