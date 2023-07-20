﻿using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{

	public class State : MonoBehaviour, IState
    {
        [SerializeField] private StateID m_stateID;
		public IStateID ID => m_stateID;


        [SerializeField] private Object[] m_logic = null;
        public IEnumerable<IStateLogic> Logic => m_logic.OfType<IStateLogic>();

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