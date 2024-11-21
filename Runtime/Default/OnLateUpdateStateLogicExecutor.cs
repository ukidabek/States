﻿using UnityEngine;

namespace States.Core
{
	[AddComponentMenu("States/Executors/OnLateUpdateStateLogicExecutor")]
	[DisallowMultipleComponent]
	public class OnLateUpdateStateLogicExecutor : StateLogicExecutor
    {
	    private void LateUpdate()
	    {
		    var timeInfo = GetTimeInfo();
		    m_stateMachine.OnLateUpdate(timeInfo.deltaTime, timeInfo.timeScale);
	    }
	}
}