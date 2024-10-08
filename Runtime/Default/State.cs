﻿using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States.Default
{
    [AddComponentMenu("States/Core/State")]
	public class State : MonoBehaviour, IState
    {
        [SerializeField] private StateID m_stateID;
		public IStateID ID => m_stateID;

        [SerializeField] private Object[] m_logic = null;
        public IEnumerable<IStateLogic> Logic => m_logic.OfType<IStateLogic>();

        public bool CanExit => Logic.All(_logic => _logic.CanBeDeactivated);

		public string Name => gameObject.name;

        [SerializeField] private bool m_isStatic = false;
        public bool IsStatic => m_isStatic;

		public void Enter()
        {
            foreach (var stateLogic in Logic) 
                stateLogic.Activate();
        }

        public void Exit()
        {
            foreach (var stateLogic in Logic) 
                stateLogic.Deactivate();
        }
    }
}