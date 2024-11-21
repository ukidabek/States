using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace States.Core.Test
{
	public class ExtendedStateLogic : StateLogic { }
	public class StateLogic : IStateLogic, IOnUpdateLogic, IOnFixedUpdateLogic, IOnLateUpdateLogic
	{
		[ContextField] protected BoxCollider m_boxCollider = null;
		public BoxCollider BoxCollider => m_boxCollider;

		[ContextField] protected Rigidbody m_rigidbody = null;
		public Rigidbody Rigidbody => m_rigidbody;

		[ContextField("Test")] private Rigidbody m_rigidbodyWithID = null;
		public Rigidbody RigidbodyWithID => m_rigidbodyWithID;

		[ContextField] private ITestInterface m_testInterface = null;
		public ITestInterface TestInterface => m_testInterface;

		[ContextField("Test_1")] private ITestInterface1 m_testInterface1 = null;
		public ITestInterface1 TestInterface1 => m_testInterface1;

		[ContextField] public SphereCollider SphereCollider { get; set; }

		[ContextField("Test_3")] public Animator Animator { get; private set; }

		public float UpdateCount = 0;
		public float FixUpdateCount = 0;
		public float LateUpdateCount = 0;

		public virtual IEnumerable<IContextDestination> ContextDestinations { get; protected set; } = Array.Empty<IContextDestination>();

		public bool CanBeDeactivated { get; set; }

		public void Activate() { }

		public void Deactivate() { }

		public void OnUpdate(float deltaTime, float timeScale)
		{
			Assert.AreEqual(1f, timeScale);
			UpdateCount += deltaTime;
		}

		public void OnFixexUpdate(float deltaTime, float timeScale)
		{
			Assert.AreEqual(1f, timeScale);
			FixUpdateCount += deltaTime;
		}

		public void OnLateUpdate(float deltaTime, float timeScale)
		{
			Assert.AreEqual(1f, timeScale);
			LateUpdateCount += deltaTime;
		}
	}
}