using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace BaseGameLogic.States
{
    public class StateGraphEditorWindow : EditorWindow
    {
        private StateGraph _stateGraph = null;
        private List<Node> _nodes = new List<Node>();
        private List<TransitionInfo> _transitionRectList = new List<TransitionInfo>();
        private TransitionInfo _selectedTransition = null;

        private Dictionary<BaseState, List<Node>> _statesDictionary = new Dictionary<BaseState, List<Node>>();
        private Node _selectedNode = null;

        private Vector2 offset;
        private Vector2 drag;

        private Rect _graphAreaRect = new Rect();
        private Rect _graphAreaWorkRect = new Rect(Vector2.zero, new Vector2(5000, 5000));

        private Rect _menuAreaRect = new Rect();
        private Rect _inspectorAreaRecr = new Rect();
        private float _inspectorSize = .3f;

        private GenericMenu _addStateContextMenu = new GenericMenu();
        private GenericMenu _addContitionContextMenu = new GenericMenu();

        private GenericMenu _selectedNodeContextMenu = new GenericMenu();
        private GenericMenu _selectedTransitionContextMenu = new GenericMenu();

        private Vector2 _currentMousePossition = Vector2.zero;

        private Connector _connector = null;
        private Connector _formAnyStateConnector = null;

        private StateInspecotr _stateInspecotr = new StateInspecotr();
        private TransitionConditionsInspector _transitionConditionsInspector = new TransitionConditionsInspector();
        private Vector2 _scrollPositon;

        public StateGraphEditorWindow()
        {
            titleContent = new GUIContent("State Graph");
            minSize = new Vector2(800, 600);
            _scrollPositon = new Vector2(_graphAreaWorkRect.width / 2, _graphAreaWorkRect.height / 2);
        }

        public void Initialize(StateGraph stateGraph)
        {
            _stateGraph = stateGraph;

            _addStateContextMenu = GenerateAddMenu(AssemblyExtension.GetDerivedTypes<BaseState>(), "Add state/{0}", AddState, GetNameFromType);
            //_addStateContextMenu = GenerateAddMenu(_stateGraph.NodeInfo.ToArray(), "Add state reference/{0}", AddStateReference, GetNameFromNode, _addStateContextMenu);
            _addContitionContextMenu = GenerateAddMenu(AssemblyExtension.GetDerivedTypes<BaseStateTransitionCondition>(), "{0}", AddCondition, GetNameFromType);

            _transitionConditionsInspector.SetData(null, null, _addContitionContextMenu);

            _selectedNodeContextMenu.AddItem(new GUIContent("Remove state"), false, RemoveState);
            _selectedTransitionContextMenu.AddItem(new GUIContent("Remove transition"), false, RemoveTransition);

            _stateInspecotr.SetData(_selectedNode, _stateGraph, _addContitionContextMenu);

            _connector = new Connector(SetDirty);
            _formAnyStateConnector = new Connector(SetDirty, _stateGraph);

            _nodes.AddRange(_stateGraph.NodeInfo);
            _nodes.Add(_stateGraph.FromAnyStateNode);

            _stateGraph.FromAnyStateNode.OnConnectionPointClicked += _formAnyStateConnector.Connect;

            foreach (var item in _nodes)
            {
                if (item.State != null)
                {
                    List<Node> nodeList = null;
                    if (_statesDictionary.ContainsKey(item.State))
                    {
                        nodeList = _statesDictionary[item.State];
                    }
                    else
                    {
                        nodeList = new List<Node>();
                        _statesDictionary.Add(item.State, nodeList);
                    }

                    nodeList.Add(item);
                }

                item.IsSelected = false;
                item.OnConnectionPointClicked += _formAnyStateConnector.Connect;
                item.OnConnectionPointClicked += _connector.Connect;
            }
        }

        private void Awake()
        {
            _menuAreaRect.position = Vector2.zero;
            _menuAreaRect.size = new Vector2(position.width, EditorGUIUtility.singleLineHeight);

            CalculateInspectorAndGraphRect();
        }

        private string GetNameFromType(object obj)
        {
            return (obj as Type).Name;
        }

        private string GetNameFromNode(object obj)
        {
            return (obj as Node).State.GetType().Name;
        }

        private GenericMenu GenerateAddMenu(object[] typesToAdd, string formatString, GenericMenu.MenuFunction2 addState, Func<object, string> getName = null, GenericMenu existingMenu = null)
        {
            GenericMenu menu = existingMenu != null ? existingMenu : new GenericMenu();
            Func<object, string> _getName = getName;
            if(_getName == null)
            {
                _getName = ( object obj) => { return obj.ToString(); };
            }

            foreach (var item in typesToAdd)
            {
                GUIContent content = new GUIContent(string.Format(formatString, _getName(item)));
                menu.AddItem(content, false, addState, item);
            }

            return menu;
        }

        private void CalculateInspectorAndGraphRect()
        {
            _inspectorAreaRecr.position = new Vector2(position.width * (1 - _inspectorSize), _menuAreaRect.height);
            _inspectorAreaRecr.size = new Vector2(position.width * _inspectorSize, position.height - _menuAreaRect.height);


            _graphAreaRect.position = new Vector2(0, _menuAreaRect.height);
            _graphAreaRect.size = new Vector2(position.width * (1 - _inspectorSize), position.height - _menuAreaRect.height);
        }

        private void OnDestroy()
        {
            if (_stateGraph == null) return;

            _stateGraph.FromAnyStateNode.OnConnectionPointClicked -= _formAnyStateConnector.Connect;

            foreach (var item in _nodes)
            {
                item.OnConnectionPointClicked -= _connector.Connect;
                item.OnConnectionPointClicked -= _formAnyStateConnector.Connect;
            }
        }

        private void OnGUI()
        {
            CalculateInspectorAndGraphRect();
            DrawMenuArea();
            DrawGraphArea();
            DrawInspectorArea();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void ProcessNodeEvents(Event current)
        {
            _selectedNode = null;
            foreach (var item in _nodes)
            {
                if (item.ProcessEvents(current, new Vector2(0, _menuAreaRect.height) - _scrollPositon))
                {
                    GUI.changed = true;
                }

                if (item.IsSelected)
                {
                    _selectedNode = item;
                    _selectedTransition = null;
                }
            }
        }

        private void ProcessEvents(Event current)
        {
            switch (current.type)
            {
                case EventType.MouseDown:
                    bool transitionSelected = false;
                    foreach (var item in _transitionRectList)
                    {
                        if (item.Rect.Contains(current.mousePosition - new Vector2(0, _menuAreaRect.height) + _scrollPositon))
                        {
                            transitionSelected = true;
                            _selectedTransition = item;
                            if(_selectedNode != null)
                            {
                                _selectedNode.IsSelected = false;
                                _selectedNode = null;
                            }
                            GUI.changed = true; 
                            break;
                        }
                    }
                    switch(current.button)
                    {
                        case 1:
                            if (_selectedNode == null)
                            {
                                if(transitionSelected)
                                    _selectedTransitionContextMenu.ShowAsContext();
                                else
                                    _addStateContextMenu.ShowAsContext();
                            }
                            else
                            {
                                _selectedNodeContextMenu.ShowAsContext();
                            }
                            break;
                    }
                       
                    _currentMousePossition = current.mousePosition;
                    break;

                case EventType.MouseDrag:
                    if(current.button == 2)
                    {
                        _scrollPositon -= current.delta;
                        Repaint();
                    }
                    break;
            }
        }

        private void DrawMenuArea()
        {
            GUILayout.BeginArea(_menuAreaRect);
            {
                _stateGraph.Type = (GraphType)EditorGUILayout.EnumPopup(_stateGraph.Type);
            }
            GUILayout.EndArea();
        }

        private void DrawGraphArea()
        {
            GUILayout.BeginArea(_graphAreaRect);
            {
                Rect scrollArea = new Rect(_graphAreaRect);
                scrollArea.y -= 16;
                _scrollPositon = GUI.BeginScrollView(scrollArea, _scrollPositon, _graphAreaWorkRect, false, false, new GUIStyle(), new GUIStyle());
                {
                    DrawGrid(_graphAreaWorkRect, 10, 0.2f, Color.gray);
                    DrawGrid(_graphAreaWorkRect, 100, 0.4f, Color.gray);

                    _transitionRectList.Clear();

                    for (int i = 0; i < _stateGraph.FormAnyStateTransition.Count; i++)
                    {
                        if(_stateGraph.FormAnyStateTransition[i].TargetState == null)
                        {
                            _stateGraph.FormAnyStateTransition.RemoveAt(i);
                            --i;
                            continue;
                        }

                        DrawTransitionBezier(_stateGraph.FromAnyStateNode, _stateGraph.FormAnyStateTransition[i], -1, i);
                    }

                    for (int i = 0; i < _nodes.Count; i++)
                    {
                        var node = _nodes[i];
                        if (node.State == null) continue;
                        for (int j = 0; j < node.State.Transitions.Count; j++)
                        {
                            var transition = node.State.Transitions[j];

                            if (transition.TargetState == null)
                            {
                                node.State.Transitions.RemoveAt(j--);
                            }
                            else
                            {
                                DrawTransitionBezier(node, transition, i, j);
                            }
                        }
                        node.Draw();
                    }
                    _stateGraph.FromAnyStateNode.Draw();
                }
                GUI.EndScrollView();
            }
            GUILayout.EndArea();
        }

        private void DrawTransitionBezier(Node node, StateTransition transition, int i, int j)
        {
            List<Node> targetNode = _statesDictionary[transition.TargetState];

            if (node.IsReference) return;

            for (int k = 0; k < targetNode.Count; k++)
            {
                bool isSelectedTransition = _selectedTransition != null && _selectedTransition.NodeIndex == i && _selectedTransition.TransitionIndex == j;
                Handles.DrawBezier(
                    node.Out.Rect.center,
                    targetNode[k].In.Rect.center,
                    node.Out.Rect.center - Vector2.left * 50f,
                    targetNode[k].In.Rect.center + Vector2.left * 50f,
                    isSelectedTransition ? Color.red : Color.white,
                    null,
                    2f);

                Rect transitionRect = new Rect(Vector2.zero, new Vector2(10, 10));
                transitionRect.center = ((node.Out.Rect.center + targetNode[k].In.Rect.center) / 2);
                _transitionRectList.Add(new TransitionInfo(transitionRect, i, j));
                GUI.Box(transitionRect, "");
            }
        }

        private void DrawInspectorArea()
        {
            GUILayout.BeginArea(_inspectorAreaRecr);
            {
                if (_selectedTransition != null)
                {
                    StateTransition transition = _stateGraph[_selectedTransition.NodeIndex, _selectedTransition.TransitionIndex];
                    _transitionConditionsInspector.DrawInspector(transition.Conditions);
                }

                _stateInspecotr.DrawInspector(_selectedNode, _stateGraph, _addContitionContextMenu);
            }
            GUILayout.EndArea();
        }

        private void AddState(object data)
        {
            Type type = data as Type;
            BaseState state = _stateGraph.gameObject.AddComponent(type) as BaseState;

            Undo.RecordObject(_stateGraph, "State added");

            Node newNode = new Node(_currentMousePossition + _scrollPositon, state);
            List<Node> nodeList = new List<Node>();
            nodeList.Add(newNode);
            _statesDictionary.Add(state, nodeList);

            newNode.OnConnectionPointClicked += _formAnyStateConnector.Connect;
            newNode.OnConnectionPointClicked += _connector.Connect;

            _stateGraph.NodeInfo.Add(newNode);
            _nodes.Add(newNode);

            SetDirty(_stateGraph);

            if (_stateGraph.RootState == null)
                _stateGraph.RootState = state;
        }

        private void AddStateReference(object data)
        {
            Node node = data as Node;
            Node newNode = new Node(_currentMousePossition, node);
            _stateGraph.NodeInfo.Add(newNode);
        }

        private void AddCondition(object data)
        {
            if(_selectedTransition != null)
            {
                Undo.RecordObject(_stateGraph, "Connection added");
                var transition = _stateGraph[_selectedTransition.NodeIndex, _selectedTransition.TransitionIndex];
                transition.Conditions.Add(CreateCondition(data));
            }
            if (_selectedNode != null && _selectedNode.State != null)
            {
                Undo.RecordObject(_stateGraph, "Connection added");
                _selectedNode.State.ExitStateTransitions[_selectedNode.State.SelectedExitStateTransition].Conditions.Add(CreateCondition(data));
            }
        }

        private BaseStateTransitionCondition CreateCondition(object data)
        {
            Type type = data as Type;
            return _stateGraph.gameObject.AddComponent(type) as BaseStateTransitionCondition;
        }

        private void RemoveState()
        {
            if (_selectedNode != null && _selectedNode.State != null)
            {
                Undo.RecordObject(_stateGraph.gameObject, "State removed");
                int index = _stateGraph.NodeInfo.IndexOf(_selectedNode);
                _stateGraph.NodeInfo.RemoveAt(index);
                index = _nodes.IndexOf(_selectedNode);
                _nodes.RemoveAt(index);

                for (int i = 0; i < _selectedNode.State.Transitions.Count; i++)
                {
                    for (int j = 0; j < _selectedNode.State.Transitions[i].Conditions.Count; j++)
                    {
                        GameObject.DestroyImmediate(_selectedNode.State.Transitions[i].Conditions[j]);
                    }
                    _selectedNode.State.Transitions.RemoveAt(i);
                    i--;
                }

                for (int i = 0; i < _selectedNode.State.ExitStateTransitions.Count; i++)
                {
                    for (int j = 0; j < _selectedNode.State.ExitStateTransitions[i].Conditions.Count; j++)
                    {
                        GameObject.DestroyImmediate(_selectedNode.State.ExitStateTransitions[i].Conditions[j]);
                    }

                    _selectedNode.State.ExitStateTransitions.RemoveAt(i);
                    i--;
                }

                if (!_selectedNode.IsReference)
                {
                    _selectedNode.Remove();
                    _selectedNode = null;
                }

                SetDirty(_stateGraph);
            }
        }

        private void RemoveTransition()
        {
            if (_selectedTransition != null)
            {
                Undo.RecordObject(_stateGraph, "Transition removed");
                if(_selectedTransition.NodeIndex < 0)
                {
                    RemoveAllContitionObjects(_stateGraph.FormAnyStateTransition[_selectedTransition.TransitionIndex]);
                    _stateGraph.FormAnyStateTransition.RemoveAt(_selectedTransition.TransitionIndex);
                }
                else
                {
                    RemoveAllContitionObjects(_stateGraph[_selectedTransition.NodeIndex, _selectedTransition.TransitionIndex]);
                    _stateGraph.NodeInfo[_selectedTransition.NodeIndex].State.Transitions.RemoveAt(_selectedTransition.TransitionIndex);
                }

                _selectedTransition = null;
                SetDirty(_stateGraph);
            }
        }

        private void RemoveAllContitionObjects(StateTransition stateTransition)
        {
            for (int i = 0; i < stateTransition.Conditions.Count; i++)
            {
                DestroyImmediate(stateTransition.Conditions[i]);
            }
        }

        private void DrawGrid(Rect rect, float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(rect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(rect.height / gridSpacing);

            Handles.BeginGUI();
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

                for (int i = 0; i < widthDivs; i++)
                {
                    Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0), new Vector3(gridSpacing * i, rect.height, 0f));
                }

                for (int j = 0; j < heightDivs; j++)
                {
                    Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0), new Vector3(rect.width, gridSpacing * j, 0f));
                }

                Handles.color = Color.white;
            }
            Handles.EndGUI();
        }

        private void SetDirty(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null) return;

            EditorUtility.SetDirty(monoBehaviour);
            EditorSceneManager.MarkSceneDirty(monoBehaviour.gameObject.scene);
        }
    }
}