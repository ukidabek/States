using System;
using States.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace States.Default
{
    [CustomPropertyDrawer(typeof(Context))]
    public class ContextPropertyDrawer : PropertyDrawer
    {
        private readonly Type m_objectType = typeof(Object);
        
        private SerializedProperty m_idSerializedProperty = null;
        private SerializedProperty m_objectSerializedProperty = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            m_idSerializedProperty = property.FindPropertyRelative("m_id");
            m_objectSerializedProperty = property.FindPropertyRelative("m_object");
            return EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var offset = 17f;
            var lineHeight = EditorGUIUtility.singleLineHeight +  EditorGUIUtility.standardVerticalSpacing;
            var labelWidth = EditorGUIUtility.labelWidth;
            
            
            var labelRect = new Rect(offset, 0f, labelWidth - offset, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, m_idSerializedProperty.displayName);
            labelRect.y += lineHeight;
            GUI.Label(labelRect, $"{m_objectSerializedProperty.displayName} type");
            labelRect.y += lineHeight;
            GUI.Label(labelRect, m_objectSerializedProperty.displayName);
            
            position.x += labelWidth;
            position.width -= labelWidth;
            position.height = EditorGUIUtility.singleLineHeight;
            m_idSerializedProperty.stringValue = EditorGUI.TextField(position, m_idSerializedProperty.stringValue);
            position.y += lineHeight;
            GUI.Label(position, m_objectSerializedProperty.objectReferenceValue is not null ? m_objectSerializedProperty.objectReferenceValue.GetType().Name : "None");
            position.y += lineHeight;
            m_objectSerializedProperty.objectReferenceValue = EditorGUI.ObjectField(position, 
                m_objectSerializedProperty.objectReferenceValue, 
                m_objectType, 
                true);
            
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}