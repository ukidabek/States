using System.Collections.Generic;

namespace Utilities.States.Test
{
	public class TestStateLogicExecutor : StateLogicExecutor<TestUpdateLogic>
	{
		public List<TestUpdateLogic> Logic => m_logic;
	}
}