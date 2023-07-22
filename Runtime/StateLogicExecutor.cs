using System.Collections.Generic;
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

        protected (float, float) GetTimeInfo() => (Time.deltaTime, Time.timeScale);
    }

	public abstract class StateLogicExecutor<T> : StateLogicExecutor where T : IUpdateLogic
	{
        protected readonly List<T> _logic = new List<T>(30);

		public override void RemoveLogicToExecute(IState state)
		{
			var logicToAdd = _logic.OfType<T>();
            _logic.AddRange(logicToAdd);
		}

		public override void SetLogicToExecute(IState state)
		{
            var logicToRemove = _logic.OfType<T>().ToHashSet();
            _logic.RemoveAll(logic => logicToRemove.Contains(logic));
		}
	}
}