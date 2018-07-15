using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BaseGameLogic.States.Utility
{
    [CustomPropertyDrawer(typeof(StateInfo))]
    public class StateInfoProperty : PropertyDrawer
    {
        private List<string> constructorNames = new List<string>();
        private List<StateInfo> _stateInfoList = new List<StateInfo>();

        private StateInfo stateInfo = null;
        private FieldInfo field = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (stateInfo == null)
            {
                field = property.serializedObject.targetObject.GetType().GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance);
                stateInfo = field.GetValue(property.serializedObject.targetObject) as StateInfo;
            }

            if (constructorNames.Count == 0)
            {
                var types = new List<Type>(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(Conditions));
                for (int i = 0; i < types.Count; i++)
                {
                    var constructors = types[i].GetConstructors();
                    for (int j = 0; j < constructors.Length; j++)
                    {
                        var info = new StateInfo(constructors[j]);
                        string constructor = string.Empty;
                        constructor += constructors[j].DeclaringType.Name;

                        for (int k = 0; k < info.Parameters.Length; k++)
                        {
                            Type parameterType = info.Parameters[k].Type;
                            constructor += string.Format(" {0} {1}", parameterType.Name, info.Parameters[k].ParameterName);
                        }
                        _stateInfoList.Add(info);
                        constructorNames.Add(constructor);
                    }
                }

                if (string.IsNullOrEmpty(stateInfo.Type.TypeName) && string.IsNullOrEmpty(stateInfo.Type.AssemblFullName))
                    stateInfo = _stateInfoList[0];
            }

            return EditorGUIUtility.singleLineHeight * (stateInfo.Parameters.Length + 1);
        }

        private bool Conditions(Type arg)
        {
            return (typeof(IState)).IsAssignableFrom(arg) && arg.BaseType == typeof(System.Object) && !arg.IsInterface;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int index = _stateInfoList.IndexOf(stateInfo);
            if (index < 0)
            {
                stateInfo = _stateInfoList[index = 0];
                field.SetValue(property.serializedObject.targetObject, stateInfo);
            }

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
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
        }
    }
}