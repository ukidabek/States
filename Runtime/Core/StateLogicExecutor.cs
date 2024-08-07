﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
    public abstract class StateLogicExecutor : MonoBehaviour, IStateLogicExecutor
    {
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
        public abstract void SetLogicToExecute(IState state);

        public abstract void RemoveLogicToExecute(IState state);

        protected virtual (float, float) GetTimeInfo() => (Time.deltaTime, Time.timeScale);
    }

	public abstract class StateLogicExecutor<T> : StateLogicExecutor where T : IUpdateLogic
	{
        protected readonly List<T> m_logic = new List<T>(30);

		public override void SetLogicToExecute(IState state)
		{
			var logicToAdd = state.Logic.OfType<T>();
            m_logic.AddRange(logicToAdd.Except(m_logic));
		}

		public override void RemoveLogicToExecute(IState state)
		{
            var logicToRemove = state.Logic.OfType<T>().ToHashSet();
            m_logic.RemoveAll(logic => logicToRemove.Contains(logic));
		}
	}
}