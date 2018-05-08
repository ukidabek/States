using UnityEngine;
using UnityEditor;

using System;

namespace BaseGameLogic.States
{
    internal class Connector 
    {
        private BaseState _inNode = null;
        private BaseState _outNode = null;

        private Action<MonoBehaviour> SetDirty = null;

        private StateGraph _graph = null;
        private bool _formAnyStateTransition = false;

        private bool _inNodeSelected = false;
        private bool _outNodeSelected = false;

        public Connector(Action<MonoBehaviour> setDirty)
        {
            SetDirty = setDirty;
        }

        public Connector(Action<MonoBehaviour> setDirty, StateGraph graph) : this(setDirty)
        {
            SetDirty = setDirty;
            _graph = graph;
        }

        public void Connect(ConnectionPointType type, BaseState state)
        {
            switch (type)
            {
                case ConnectionPointType.In:
                    if (_inNode == null) _inNode = state;
                    _inNodeSelected = true;
                    break;
                case ConnectionPointType.Out:
                    if (_outNode == null) _outNode = state;
                    if (_graph != null) _formAnyStateTransition = true;
                    _outNodeSelected = true;
                    break;
            }

            if(_graph == null && !_formAnyStateTransition && _inNode != null && _outNode != null)
            {
                Undo.RecordObject(_outNode.gameObject, "Transition added");
                _outNode.Transitions.Add(new StateTransition(_inNode));

                if(SetDirty != null)
                {
                    SetDirty(_outNode);
                    SetDirty(_inNode);
                }
            }

            if(_graph != null && _formAnyStateTransition && _inNode != null && _outNode == null)
            {
                Undo.RecordObject(_inNode.gameObject, "From Any state transition added");

                _graph.FormAnyStateTransition.Add(new StateTransition(_inNode));

                if (SetDirty != null)
                {
                    SetDirty(_graph);
                    SetDirty(_outNode);
                }
            }

            if(_inNodeSelected && _outNodeSelected)
            {
                _formAnyStateTransition = false;
                _inNodeSelected = _outNodeSelected = false;
                _inNode = _outNode = null;
            }
        }
    }
}