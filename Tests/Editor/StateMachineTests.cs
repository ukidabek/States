using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.States.Test
{
	[CreateAssetMenu]
	public class StateMachineTests
	{
		private GameObject m_gameObject = null;
		private StateMachine m_stateMachine = null;

		private IEnumerable<Context> m_contexts = null;

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var boxCollider = m_gameObject.GetComponent<BoxCollider>();
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new Component[] { boxCollider, rigidbody }.Select(component => new Context(component));
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransitionLogic>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.BoxCollider, boxCollider);
			Assert.AreEqual(stateLogic.Rigidbody, rigidbody);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_Using_ID()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new[] { new Context("Test", rigidbody) };
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransitionLogic>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.RigidbodyWithID, rigidbody);
		}

		[TearDown]
		public void TearDown()
		{
			m_contexts = null;
			GameObject.DestroyImmediate(m_gameObject);
			m_stateMachine.Dispose();
			m_stateMachine = null;
		}
	}
}