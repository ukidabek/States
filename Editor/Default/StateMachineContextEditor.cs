using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using States.Core;
using UnityEditor;
using UnityEngine;

namespace States.Default
{
    [CustomEditor(typeof(StateMachineContext), true)]
    public class StateMachineContextEditor : Editor
    {
        private StateMachineContext m_stateMachineContext;
        
        private readonly List<ContextHandler> m_contextHandlers = new List<ContextHandler>(30);
        private readonly List<ContextHandler> m_validContexts = new List<ContextHandler>(30);
        
        private string m_search = string.Empty;
        private List<Context> m_contextList = null;
        
        private GUIContent m_plusIcon = null;
        private GUIContent m_minusIcon = null;
        private GUIStyle m_searchCancelButtonStyle = null;

        private void OnEnable()
        {
            m_plusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
            m_minusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
            m_stateMachineContext = (StateMachineContext)target;
            var type = m_stateMachineContext.GetType();
            var contextsFieldInfo = type.GetField("m_context", BindingFlags.NonPublic | BindingFlags.Instance);
            m_contextList = contextsFieldInfo.GetValue(target) as List<Context>;
            m_contextHandlers.AddRange(m_contextList.Select(contextHandler => new ContextHandler(contextHandler)));
        }

        public override void OnInspectorGUI()
        {
            m_searchCancelButtonStyle ??= GUI.skin.FindStyle("SearchCancelButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("SearchCancelButton");
        
            EditorGUILayout.BeginHorizontal();
            m_search = GUILayout.TextField(m_search, EditorStyles.toolbarSearchField);
            if (!string.IsNullOrEmpty(m_search))
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                
                lastRect.width = EditorGUIUtility.singleLineHeight;
                lastRect.height = EditorGUIUtility.singleLineHeight;
                lastRect.x = EditorGUIUtility.currentViewWidth - lastRect.width - 17;

                var current = Event.current;
                GUI.Label(lastRect, GUIContent.none, m_searchCancelButtonStyle);
                if (current.type == EventType.Used && current.button == 0 && lastRect.Contains(current.mousePosition))
                {
                    m_search = string.Empty;
                    current.Use();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            m_validContexts.Clear();
            m_validContexts.AddRange(m_contextHandlers.Where(context => context.IsValid(m_search)));
            var isDirty = false;

            foreach (var contextHandler in m_validContexts)
            {
                EditorGUILayout.BeginHorizontal();
                if (contextHandler.Draw()) 
                    isDirty = true;
                
                if (GUILayout.Button(m_minusIcon))
                {
                    isDirty = true;
                    m_contextHandlers.Remove(contextHandler);
                    m_search = string.Empty;
                }
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button(m_plusIcon))
            {
                m_search = string.Empty;
                isDirty = true;
                m_contextHandlers.Add(new ContextHandler());
            }
            
            if (!isDirty) return;
            
            m_contextList.Clear();
            m_contextList.AddRange(m_contextHandlers.Select(handler => handler.Context));
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }

    public class ContextHandler
    {
        private static FieldInfo m_idFieldInfo = null;
        private static FieldInfo m_objectFieldInfo = null;
        
        public Context Context { get; private set; }
        
        private string m_id = null;
        private Object m_object = null;

        static ContextHandler()
        {
            var type = typeof(Context);
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            m_idFieldInfo = type.GetField("m_id", bindingFlags);
            m_objectFieldInfo = type.GetField("m_object", bindingFlags);
        }

        public ContextHandler() : this(new Context())
        {
        }
        
        public ContextHandler(Context context)
        {
            Context = context;
            m_id = m_idFieldInfo.GetValue(Context) as string;
            m_object = m_objectFieldInfo.GetValue(Context) as Object;
        }

        public bool Draw()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("ID", GUILayout.Width(20));
            m_id = EditorGUILayout.TextField(m_id,GUILayout.Width(100));
            EditorGUILayout.LabelField("Object", GUILayout.Width(40));
            m_object = EditorGUILayout.ObjectField(m_object, typeof(Object), true);
            EditorGUILayout.EndHorizontal();
            if (!EditorGUI.EndChangeCheck()) return false;
            
            m_idFieldInfo.SetValue(Context, m_id);
            m_objectFieldInfo.SetValue(Context, m_object);
            
            return true;
        }

        public bool IsValid(string search)
        {
            if(string.IsNullOrEmpty(search)) return true;

            var searchLower = search.ToLower();
            return m_id.ToLower().Contains(searchLower) || (m_object != null && m_object.GetType().Name.ToLower().Contains(searchLower));
        }
    }
    
}