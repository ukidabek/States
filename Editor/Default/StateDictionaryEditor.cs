using States.Default;
using UnityEditor;
using UnityEngine;

namespace States.Core.Default
{
    [CustomEditor(typeof(StateDictionary))]
    public class StateDictionaryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate dictionary"))
            {
                (target as StateDictionary).GenerateDictionary();
                EditorUtility.SetDirty(target);
            }
        }
    }
}