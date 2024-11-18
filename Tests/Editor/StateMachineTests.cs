using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utilities.States.Test
{
	public class StateMachineTests
	{
		private GameObject m_gameObject = null;
		private StateMachine m_stateMachine = null;

		private IEnumerable<Context> m_contexts = null;

		[Test]
		public void Validate_If_StateMachine_Enter_State()
		{
			m_contexts = Array.Empty<Context>();
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			var stateA = new State(new StateID(), new[] { new StateLogic() });
			var stateB = new State(new StateID(), new[] { new StateLogic() });

			m_stateMachine.EnterState(stateA);
			m_stateMachine.EnterState(stateB);

			Assert.AreEqual(stateB, m_stateMachine.CurrentState);
			Assert.AreEqual(stateA, m_stateMachine.PreviousState);
		}

		[Test]
		public void Validate_If_State_Wont_Change_When_Current_State_Cant_Exit()
		{
			m_contexts = Array.Empty<Context>();
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			var stateA = new State(new StateID(), new[] { new StateLogic() });
			var stateB = new State(new StateID(), new[] { new StateLogic() });

			m_stateMachine.EnterState(stateA);
			Assert.AreEqual(stateA, m_stateMachine.CurrentState);

			stateA.CanExit = false;
			m_stateMachine.EnterState(stateB);

			Assert.AreNotEqual(stateB, m_stateMachine.CurrentState);
			Assert.AreEqual(stateA, m_stateMachine.CurrentState);
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void Validate_If_StateLogic_Are_Update(int iterations)
		{
			m_contexts = Array.Empty<Context>();
			var executor = new StateLogicExecutor();
			var logic = new StateLogic();
			var state = new State(new StateID(), new[] { logic });

			m_stateMachine = new StateMachine(new[] { executor }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			for (int i = 0; i < iterations; i++)
				executor.Update();

			Assert.AreEqual((float)iterations, logic.UpdateCount);
			Assert.AreEqual((float)iterations, logic.LateUpdateCount);
			Assert.AreEqual((float)iterations, logic.FixUpdateCount);

		}

		private IEnumerable<Context> GenerateContext(params UnityEngine.Object[] components) => components.Select(component => new Context(component));

		[TestCase(false)]
		[TestCase(true)]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly(bool cacheContext)
		{
			if (cacheContext)
			{
				var type = typeof(ContextHandler);
				var bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
				var cacheLogic = type.GetMethod("GenerateStateLogicCache", bindingFlags);
				cacheLogic.Invoke(null, null);
			}
			
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var boxCollider = m_gameObject.GetComponent<BoxCollider>();
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = GenerateContext(boxCollider, rigidbody);
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.BoxCollider, boxCollider);
			Assert.AreEqual(stateLogic.Rigidbody, rigidbody);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_Is_State_Logic_Inherit_Context_Fields()
		{
			var stateLogic = new ExtendedStateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var boxCollider = m_gameObject.GetComponent<BoxCollider>();
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = GenerateContext(boxCollider, rigidbody);
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
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
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.RigidbodyWithID, rigidbody);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_When_Context_Field_Type_Is_Interface()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var testInterface = ScriptableObject.CreateInstance<TestInterface>();

			m_contexts = GenerateContext(testInterface);
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.TestInterface, testInterface);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_When_Context_Field_Type_Is_Interface_Using_ID()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var testInterface = ScriptableObject.CreateInstance<TestInterface>();

			m_contexts = new[] { new Context("Test_1", testInterface) };
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.TestInterface, testInterface);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_To_Property()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(SphereCollider));
			var sphereCollider = m_gameObject.GetComponent<SphereCollider>();

			m_contexts = GenerateContext(sphereCollider);
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.SphereCollider, sphereCollider);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_To_Property_Using_ID()
		{
			var stateLogic = new StateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(SphereCollider));
			var animator = m_gameObject.GetComponent<Animator>();

			m_contexts = new[] { new Context("Test_3", animator) };
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.Animator, animator);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_To_Destinations_Provided_By_StateLogic()
		{
			var stateLogic = new TestUpdateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(SphereCollider));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new[] { new Context(rigidbody), new Context("Test_1", rigidbody) };
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.Test.Rigidbody, rigidbody);
			Assert.AreEqual(stateLogic.Test.Rigidbody2, rigidbody);
		}

		[Test]
		public void Validate_If_Class_With_Extend_Context_Type_Are_Injectec_Correctly()
		{
			var stateLogic = new TestUpdateLogic();
			var state = new State(new StateID(), new[] { stateLogic });

			m_gameObject = new GameObject("TestObject", typeof(TextExtensionClass));
			var extensionClass = m_gameObject.GetComponent<TextExtensionClass>();

			m_contexts = new[] { new Context(extensionClass)};
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(extensionClass, stateLogic.Test.BaseClass);
		}

		[Test]
		public void Validate_If_State_Marked_As_Static_Is_Ignored_When_State_Machine_Enter_That_State_Again()
		{
			var stateLogic = new TestUpdateLogic();
			var state = new State(new StateID(), new[] { stateLogic })
			{
				IsStatic = true,
			};

			var stateA = new State(new StateID(), Array.Empty<IStateLogic>());

			m_gameObject = new GameObject("TestObject", typeof(TextExtensionClass));
			var extensionClass = m_gameObject.GetComponent<TextExtensionClass>();

			m_contexts = new[] { new Context(extensionClass) };
			m_stateMachine = new StateMachine(new[] { new StateLogicExecutor() }, Array.Empty<IStateTransition>(), m_contexts);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(extensionClass, stateLogic.Test.BaseClass);
		
			stateLogic.Test.BaseClass = null;

			m_stateMachine.EnterState(stateA);
			m_stateMachine.EnterState(state);

			Assert.AreEqual(stateLogic.Test.BaseClass, null);

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