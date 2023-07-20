using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
    public class OnFixedUpdateStateLogicExecutor : StateLogicExecutor
    {
        private IEnumerable<IOnFixUpdateLogic> _logic = new List<IOnFixUpdateLogic>();
        
        public override void SetLogicToExecute(IState state)
        {
            _logic = state.Logic.OfType<IOnFixUpdateLogic>();
        }

        private void Update()
        {
			var timeInfo = GetTimeInfo();
			foreach (var onUpdateLogic in _logic)
				onUpdateLogic.OnFixUpdate(timeInfo.Item1, timeInfo.Item2);
		}
	}
}