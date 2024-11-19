using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Utilities.States.Test
{
	public class StateLogicExecutorTest
	{
		private TestStateLogicExecutor m_executor = null;
		private State m_state = null;

		[SetUp]
		public void Setup()
		{
			var gameObject = new GameObject("TextExecutor", typeof(TestStateLogicExecutor));
			m_executor = gameObject.GetComponent<TestStateLogicExecutor>();
			m_state = new State(null, new[] { new TestUpdateLogic() }, Enumerable.Empty<IStateTransition>());
		}

		[Test]
		public void Validate_If_StateLogicExecutor_Dont_Have_Duplicated_Logic_After_Logic_Is_Added()
		{
			m_executor.SetLogicToExecute(m_state);
			m_executor.SetLogicToExecute(m_state);
			Assert.AreEqual(1, m_executor.Logic.Count);
		}

		[TearDown]
		public void TearDown() 
		{
			GameObject.DestroyImmediate(m_executor.gameObject);
		}
	}
}