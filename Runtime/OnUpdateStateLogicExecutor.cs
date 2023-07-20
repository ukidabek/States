using System.Collections.Generic;
using System.Linq;

namespace Utilities.States
{
    public class OnUpdateStateLogicExecutor : StateLogicExecutor
    {
        private IEnumerable<IOnUpdateLogic> _logic = new List<IOnUpdateLogic>();
        
        public override void SetLogicToExecute(IState state)
        {
            _logic = state.Logic.OfType<IOnUpdateLogic>();
        }

        private void Update()
        {
			var timeInfo = GetTimeInfo();
			foreach (var onUpdateLogic in _logic) 
                onUpdateLogic.OnUpdate(timeInfo.Item1, timeInfo.Item2);
        }
    }
}