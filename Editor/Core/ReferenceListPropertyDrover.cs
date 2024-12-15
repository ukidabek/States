using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

namespace States.Core
{
    [CustomPropertyDrawer(typeof(ReferenceListAttribute))]
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
                
                var selectedTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => 
                        type.IsSubclassOf(baseType) &&
                        !type.IsSubclassOf(typeof(UnityEngine.Object)) &&
                        !type.IsAbstract && 
                        !type.IsInterface);
                
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
        
        private ReorderableList m_reorderableList = null;
        private TypeProvider m_typeProvider = null;
        
        private int m_activeIndex = -1;
        private bool? m_isTypeInvalid;
        private Type m_baseType = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_isTypeInvalid.Value)
            {
                var name = m_baseType == null ? fieldInfo.FieldType.Name : m_baseType.Name; 
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
            else if(m_isTypeInvalid.Value)
                return base.GetPropertyHeight(property, label);

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

            if (m_typeProvider == null)
            {
                m_typeProvider = ScriptableObject.CreateInstance<TypeProvider>();
                m_typeProvider.GenerateTreeEntries(m_baseType);
            }
            
            var propertiesEnumerator = property.GetEnumerator();
            var propertiesToDisplayCount = 0;
            while (propertiesEnumerator.MoveNext())
            {
                var element = propertiesEnumerator.Current as SerializedProperty;
                propertiesToDisplayCount += element.CountInProperty();
            }

            propertiesToDisplayCount = propertiesToDisplayCount == 0 ? 1 : propertiesToDisplayCount;

            return m_reorderableList.headerHeight +
                   m_reorderableList.elementHeight * propertiesToDisplayCount +
                   m_reorderableList.footerHeight +
                   base.GetPropertyHeight(property, label);
        }

        private void DrawHeaderCallback(Rect rect) => 
            EditorGUI.LabelField(rect, m_reorderableList.serializedProperty.displayName);

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
            return EditorGUI.GetPropertyHeight(element, element.isExpanded);
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isActive) m_activeIndex = index;
            rect.x += 10f;
            var element = m_reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var elementType = element.managedReferenceValue.GetType();
            EditorGUI.PropertyField(rect, element, new GUIContent(elementType.Name), element.isExpanded);
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