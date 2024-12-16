using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

namespace States.Core
{
#if UNITY_2023_1_OR_NEWER
    [CustomPropertyDrawer(typeof(ReferenceListAttribute))]
#endif
    public class ReferenceListPropertyDrover : PropertyDrawer
    {
        private class TypeProvider : ScriptableObject, ISearchWindowProvider
        {
            public SerializedProperty ListToFill { get; set; }
            private readonly List<SearchTreeEntry> m_searchTreeEntry = new List<SearchTreeEntry>();
            
            public void GenerateTreeEntries(Type baseType)
            {
                m_searchTreeEntry.Clear();
                m_searchTreeEntry.Add(new SearchTreeGroupEntry(new GUIContent(baseType.Name)));

                bool TypeValidateLogic(Type type)
                {
                    if (type.IsAbstract) return false;
                    if (type.IsInterface) return false;
                    if (type.IsSubclassOf(typeof(UnityEngine.Object))) return false;
                    if (baseType.IsInterface)
                    {
                        if (!baseType.IsAssignableFrom(type)) return false;
                    }
                    else
                    {
                        if (!type.IsSubclassOf(baseType)) return false;
                    }
                 
                    return true;
                }
                
                var selectedTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(TypeValidateLogic);
                
                foreach (var type in selectedTypes)
                    m_searchTreeEntry.Add(new SearchTreeEntry(new GUIContent(type.Name))
                    {
                        level = 1,
                        userData = type,
                    });
            }

            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => m_searchTreeEntry;

            public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
            {
                try
                {
                    var Type = SearchTreeEntry.userData as Type;
                    var instance = Activator.CreateInstance(Type);
                    var index = ListToFill.arraySize++;
                    var newElement = ListToFill.GetArrayElementAtIndex(index);
                    newElement.managedReferenceValue = instance;
                    ListToFill.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
                
                return true;
            }
        }

        private const float Margin =
#if UNITY_6000
            10f;
#else
            0f;
#endif
        
        private ReorderableList m_reorderableList = null;
        private TypeProvider m_typeProvider = null;
        
        private int m_activeIndex = -1;
        private bool? m_isTypeInvalid;
        private Type m_baseType = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_isTypeInvalid.Value)
            {
                EditorGUI.LabelField(position, $"{m_baseType} is invalid!");
                return;
            }

            m_reorderableList.DoList(position);
            m_reorderableList.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            m_baseType = null;
            if (!m_isTypeInvalid.HasValue)
            {
                m_isTypeInvalid = !property.isArray;
                if (m_isTypeInvalid.Value)
                    return base.GetPropertyHeight(property, label);

                var genericArguments = fieldInfo.FieldType.GenericTypeArguments;
                if (genericArguments != null && genericArguments.Any())
                {
                    m_isTypeInvalid = genericArguments.Length > 1;
                    if (m_isTypeInvalid.Value)
                        return base.GetPropertyHeight(property, label);
                    m_baseType = genericArguments.First();
                }
                else
                    m_baseType = fieldInfo.FieldType.GetElementType();

                m_isTypeInvalid = m_baseType.IsSubclassOf(typeof(UnityEngine.Object));
                if (m_isTypeInvalid.Value)
                    return base.GetPropertyHeight(property, label);
            }
            
            if(m_isTypeInvalid.Value)
                return base.GetPropertyHeight(property, label);

            if (m_typeProvider == null)
            {
                m_typeProvider = ScriptableObject.CreateInstance<TypeProvider>();
                m_typeProvider.GenerateTreeEntries(m_baseType);
            }
            
            m_reorderableList ??= new ReorderableList(
                property.serializedObject, 
                property, 
                true, 
                true, 
                true, 
                true)
            {
                onAddCallback = OnAddCallback,
                onRemoveCallback = OnRemoveCallback,
                drawHeaderCallback = DrawHeaderCallback, 
                drawElementCallback = DrawElementCallback,
                elementHeightCallback = ElementHeightCallback,
            };
            
            return m_reorderableList.GetHeight();
        }

        private void DrawHeaderCallback(Rect rect)
        {
            var serializedPropertyDisplayName = m_reorderableList.serializedProperty.displayName;
            EditorGUI.LabelField(rect, serializedPropertyDisplayName);
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            if (m_activeIndex < 0) return;
            var listSerializedProperty = list.serializedProperty;
            listSerializedProperty.DeleteArrayElementAtIndex(m_activeIndex);
            listSerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private float ElementHeightCallback(int index)
        {
            var element = m_reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element, element.isExpanded);
            return elementHeight;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isActive) m_activeIndex = index;
            var element = m_reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var elementManagedReferenceValue = element.managedReferenceValue;
            
            if (elementManagedReferenceValue != null)
            {
                rect.x += Margin;
                rect.width -= Margin;
                var elementType = elementManagedReferenceValue.GetType();
                EditorGUI.PropertyField(rect, element, new GUIContent(elementType.Name), element.isExpanded);
                var elementSerializedObject = element.serializedObject;
                elementSerializedObject.ApplyModifiedProperties();
                elementSerializedObject.UpdateIfRequiredOrScript();
                return;
            }
           
            var warningStyle = new GUIStyle(EditorStyles.label)
            {
                normal =
                {
                    textColor = Color.red
                },
                fontStyle = FontStyle.Bold
            };
            EditorGUI.LabelField(rect, "This class is null! Maybe the class name was changed. Use the [MovedFrom] attribute!", warningStyle);
        }

        private void OnAddCallback(ReorderableList list)
        {
            var mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            var context = new SearchWindowContext(mousePosition);
            m_typeProvider.ListToFill = list.serializedProperty;
            SearchWindow.Open(context, m_typeProvider);
        }
    }
}