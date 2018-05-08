using UnityEngine;
using UnityEditor;

namespace BaseGameLogic.States
{
    [CustomEditor(typeof(BaseState), true)]
    public class BaseStateEditor : Editor 
    {
        private const string GET_ALL_REQUIRED_LABEL ="Get all required references"; 

        private BaseState state = null;

        private void OnEnable() 
        {
            state = target as BaseState;
        }

        public override void OnInspectorGUI() 
        {
            base.OnInspectorGUI();
            if(GUILayout.Button(GET_ALL_REQUIRED_LABEL))
            {
                Undo.RecordObject(state, GET_ALL_REQUIRED_LABEL);
                EditorGUI.BeginChangeCheck();
                {
                    state.GetAllRequiredReferences();
                }
                if(EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(state);
                }
            }
        }
    }
}