using System.Collections.Generic;
using System.Linq;

namespace Utilities.States.Test
{
	public class StateLogicExecutor : IStateLogicExecutor
	{
		public bool Enabled { get; set; }

		private List<IOnUpdateLogic> m_updateLogic = new List<IOnUpdateLogic>();
		private List<IOnLateUpdateLogic> m_lateUpdateLogic = new List<IOnLateUpdateLogic>();
		private List<IOnFixedUpdateLogic> m_fixUpdateLogic = new List<IOnFixedUpdateLogic>();

		public void SetLogicToExecute(IState state)
		{
			m_updateLogic.AddRange(state.Logic.OfType<IOnUpdateLogic>());
			m_lateUpdateLogic.AddRange(state.Logic.OfType<IOnLateUpdateLogic>());
			m_fixUpdateLogic.AddRange(state.Logic.OfType<IOnFixedUpdateLogic>());
		}

		public void RemoveLogicToExecute(IState state)
		{
			m_updateLogic = m_updateLogic.Except(state.Logic.OfType<IOnUpdateLogic>()).ToList();
			m_lateUpdateLogic = m_lateUpdateLogic.Except(state.Logic.OfType<IOnLateUpdateLogic>()).ToList();
			m_fixUpdateLogic = m_fixUpdateLogic.Except(state.Logic.OfType<IOnFixedUpdateLogic>()).ToList();
		}

		public void Update()
		{
			m_updateLogic.ForEach(logic => logic.OnUpdate(1f, 1f));
			m_lateUpdateLogic.ForEach(logic => logic.OnLateUpdate(1f, 1f));
			m_fixUpdateLogic.ForEach(logic => logic.OnFixexUpdate(1f, 1f));
		}
	}
}