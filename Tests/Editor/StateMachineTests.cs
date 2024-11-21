using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States.Core.Test
{
	public class StateMachineTests
	{
		private GameObject m_gameObject = null;
		private StateMachine m_stateMachine = null;

		private IEnumerable<Context> m_contexts = null;
		
		private State GenerateStateWitchContextDestination<T>(out T contextDestination, bool isStatic = false) where T :  IContextDestination, new()
		{
			contextDestination = new T();
			var state = new State(
				string.Empty,
				new Id(),
				new IContextDestination[] { contextDestination },
				Enumerable.Empty<IStateTransition>())
			{
				IsStatic = isStatic,
			};
			
			return state;
		}
		
		[Test]
		public void Validate_If_StateMachine_Enter_State()
		{
			m_contexts = Array.Empty<Context>();
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			var stateA = new State(new Id(), Enumerable.Empty<IStateTransition>());
			var stateB = new State(new Id(), Enumerable.Empty<IStateTransition>());

			m_stateMachine.EnterState(stateA);
			m_stateMachine.EnterState(stateB);

			Assert.AreEqual(stateB, m_stateMachine.CurrentState);
			Assert.AreEqual(stateA, m_stateMachine.PreviousState);
		}

		[Test]
		public void Validate_If_State_Wont_Change_When_Current_State_Cant_Exit()
		{
			m_contexts = Array.Empty<Context>();
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			var stateA = new State(new Id(), Enumerable.Empty<IStateTransition>());
			var stateB = new State(new Id(), Enumerable.Empty<IStateTransition>());

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
			var state = new State(new Id(), Enumerable.Empty<IStateTransition>());

			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			for (var i = 0; i < iterations; i++)
			{
				m_stateMachine.OnUpdate(1f,1f);
				m_stateMachine.OnFixedUpdate(1f,1f);
				m_stateMachine.OnLateUpdate(1f,1f);
			}

			Assert.AreEqual((float)iterations, state.UpdateCount);
			Assert.AreEqual((float)iterations, state.LateUpdateCount);
			Assert.AreEqual((float)iterations, state.FixUpdateCount);

		}

		private IEnumerable<Context> GenerateContext(params UnityEngine.Object[] components) => components.Select(component => new Context(component));

		[TestCase(false)]
		[TestCase(true)]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly(bool cacheContext)
		{
			if (cacheContext) 
				TestUtilities.InvokeLogic<ContextHandler>("GenerateStateLogicCache");

			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination);
			
			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var boxCollider = m_gameObject.GetComponent<BoxCollider>();
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = GenerateContext(boxCollider, rigidbody);
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(contextDestination.BoxCollider, boxCollider);
			Assert.AreEqual(contextDestination.Rigidbody, rigidbody);
		}
		
		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_Is_State_Logic_Inherit_Context_Fields()
		{
			var state = GenerateStateWitchContextDestination<ExtendedTestContextDestination>(out var contextDestination);
			
			m_gameObject = new GameObject("TestObject", typeof(BoxCollider), typeof(Rigidbody));
			var boxCollider = m_gameObject.GetComponent<BoxCollider>();
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = GenerateContext(boxCollider, rigidbody);
			m_stateMachine = new StateMachine( m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(contextDestination.BoxCollider, boxCollider);
			Assert.AreEqual(contextDestination.Rigidbody, rigidbody);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_Using_ID()
		{
			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination);

			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new[] { new Context("Test", rigidbody) };
			m_stateMachine = new StateMachine( m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(contextDestination.RigidbodyWithID, rigidbody);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_To_Property()
		{
			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination);

			m_gameObject = new GameObject("TestObject", typeof(SphereCollider));
			var sphereCollider = m_gameObject.GetComponent<SphereCollider>();

			m_contexts = GenerateContext(sphereCollider);
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(contextDestination.SphereCollider, sphereCollider);
		}

		[Test]
		public void Validate_If_References_Form_Context_Are_Injected_Correctly_To_Property_Using_ID()
		{
			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination);

			m_gameObject = new GameObject("TestObject", typeof(SphereCollider));
			var animator = m_gameObject.GetComponent<Animator>();

			m_contexts = new[] { new Context("Test_3", animator) };
			m_stateMachine = new StateMachine( m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(contextDestination.Animator, animator);
		}

		[Test]
		public void Validate_If_References_Are_Injected_ToStateTransition()
		{
			var stateTransition = new TestStateTransition();
			var state = new State(new Id(), new[] { stateTransition });
			
			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();
			
			m_contexts = new[] { new Context(rigidbody) };
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);
			
			Assert.AreEqual(stateTransition.Rigidbody, rigidbody);
		}
		
		[Test]
		public void Validate_If_Valid_Transition_Will_Trigger_State_Enter()
		{
			var stateTransition = new TestStateTransition()
			{
				Result = true,
				StateToEnter = new State(new Id(), Enumerable.Empty<IStateTransition>()),
			};
			
			var state = new State(new Id(), new[] { stateTransition });
			
			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new[] { new Context(rigidbody) };
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);
			m_stateMachine.OnUpdate(1f, 1f);
			Assert.AreEqual(m_stateMachine.CurrentState, stateTransition.StateToEnter);
		}
		
		[Test]
		public void Validate_If_State_Marked_As_Static_Is_Ignored_When_State_Machine_Enter_That_State_Again()
		{
			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination, true);
			
			var stateA = new State(new Id(), Enumerable.Empty<IStateTransition>());

			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var extensionClass = m_gameObject.GetComponent<Rigidbody>();

			m_contexts = new[] { new Context(extensionClass) };
			m_stateMachine = new StateMachine(m_contexts, new Blackboard());
			m_stateMachine.EnterState(state);

			Assert.AreEqual(extensionClass, contextDestination.Rigidbody);
		
			contextDestination.Rigidbody = null;

			m_stateMachine.EnterState(stateA);
			m_stateMachine.EnterState(state);

			Assert.IsNull(contextDestination.Rigidbody);
		}

		[Test]
		public void Validate_If_Blackboard_Provided_To_State_Is_Correct()
		{
			var state = GenerateStateWitchContextDestination<TestContextDestination>(out var contextDestination);
			var blackboard = new Blackboard();
			
			m_gameObject = new GameObject("TestObject", typeof(Rigidbody));
			var rigidbody = m_gameObject.GetComponent<Rigidbody>();
			
			m_contexts = new[] { new Context(rigidbody) };
			m_stateMachine = new StateMachine(m_contexts, blackboard);
			m_stateMachine.EnterState(state);

			m_stateMachine.OnUpdate(1f, 1f);
			Assert.AreSame(blackboard, state.Blackboard);
			m_stateMachine.OnFixedUpdate(1f, 1f);
			Assert.AreSame(blackboard, state.Blackboard);
			m_stateMachine.OnLateUpdate(1f, 1f);
			Assert.AreSame(blackboard, state.Blackboard);
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