using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BaseGameLogic.States.Utility
{
    [CustomPropertyDrawer(typeof(StateConstructor))]
    public class StateInfoProperty : PropertyDrawer
    {
        private List<string> constructorNames = new List<string>();
        private List<StateConstructor> _stateInfoList = new List<StateConstructor>();

        private StateConstructor stateInfo = null;
        private FieldInfo field = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (stateInfo == null)
            {
                field = property.serializedObject.targetObject.GetType().GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance);
                stateInfo = field.GetValue(property.serializedObject.targetObject) as StateConstructor;
            }

            if (constructorNames.Count == 0)
            {
                foreach (var type in GetTypes())
                {
                    foreach (var constructor in type.GetConstructors())
                    {
                        _stateInfoList.Add(new StateConstructor(constructor));
                        constructorNames.Add(_stateInfoList[_stateInfoList.Count - 1].Name);
                    }
                }

                if (string.IsNullOrEmpty(stateInfo.Type.FullName) && string.IsNullOrEmpty(stateInfo.Type.AssemblFullName))
                    stateInfo = _stateInfoList[0];
            }

            return EditorGUIUtility.singleLineHeight * (stateInfo.Parameters.Length + 2);
        }

        private Type[] GetTypes()
        {
            Func<Type, bool> quiry = (Type arg) => { return (typeof(IState)).IsAssignableFrom(arg) && arg.BaseType == typeof(System.Object) && !arg.IsInterface; };
            var enumeroatr = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(quiry);
            if (enumeroatr.Count() == 0)
                return null;
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(quiry).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int index = _stateInfoList.IndexOf(stateInfo);
            if (index < 0)
            {
                stateInfo = _stateInfoList[index = 0];
                field.SetValue(property.serializedObject.targetObject, stateInfo);
            }

            if (_stateInfoList.Count < 0)
            {
                EditorGUI.LabelField(position, new GUIContent(string.Format("There is no classes that implement {0}.", (typeof(IState)).Name)));
                return;
            }

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(rect, new GUIContent(property.displayName));
            rect.y += EditorGUIUtility.singleLineHeight;
            index = EditorGUI.Popup(rect, index, constructorNames.ToArray());
            for (int i = 0; i < stateInfo.Parameters.Length; i++)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                Type parametersType = stateInfo.Parameters[i].Type;
                GUIContent parameterLabel = new GUIContent(stateInfo.Parameters[i].ParameterName);
                switch (parametersType.Name)
                {
                    case "Int32":
                        stateInfo.Parameters[i].IntValue = EditorGUI.IntField(rect, parameterLabel, stateInfo.Parameters[i].IntValue);
                        break;
                    case "Single":
                        stateInfo.Parameters[i].FloatValue = EditorGUI.FloatField(rect, parameterLabel, stateInfo.Parameters[i].FloatValue);
                        break;
                    case "Boolean":
                        stateInfo.Parameters[i].BoolValue = EditorGUI.Toggle(rect, parameterLabel, stateInfo.Parameters[i].BoolValue);
                        break;
                    case "String":
                        stateInfo.Parameters[i].StringValue = EditorGUI.TextField(rect, parameterLabel, stateInfo.Parameters[i].StringValue);
                        break;
                    default:
                        stateInfo.Parameters[i].ObjectValue = EditorGUI.ObjectField(rect, parameterLabel, stateInfo.Parameters[i].ObjectValue, parametersType, true);
                        break;
                }
            }

            if (stateInfo != _stateInfoList[index])
            {
                stateInfo = _stateInfoList[index];
                field.SetValue(property.serializedObject.targetObject, stateInfo);
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.UpdateIfRequiredOrScript();
        }
    }
}