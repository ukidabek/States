using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States
{
    public class UpdateLateUpdateStateLogicExecutor : StateLogicExecutor
    {
        private IEnumerable<IOnUpdateLogic> m_updateLogic;
        private IEnumerable<IOnLateUpdateLogic> m_lateUpdateLogic;

        public override void SetLogicToExecute(IState state)
        {
            var logic = state.Logic;
            m_updateLogic = logic.OfType<IOnUpdateLogic>();
            m_lateUpdateLogic = logic.OfType<IOnLateUpdateLogic>();
        }

        public void Update()
        {
            var timeInfo = GetTimeInfo();
            foreach (var onUpdateLogic in m_updateLogic) 
                onUpdateLogic.OnUpdate(timeInfo.Item1, timeInfo.Item2);
        }

        public void LateUpdate()
        {
			var timeInfo = GetTimeInfo();
			foreach (var lateUpdateLogic in m_lateUpdateLogic) 
                lateUpdateLogic.OnLateUpdate(timeInfo.Item1, timeInfo.Item2);
        }
    }
}