﻿using NUnit.Framework;
using UnityEngine;

namespace Utilities.States.Test
{
	public class StateLogic : IStateLogic, IOnUpdateLogic, IOnFixUpdateLogic, IOnLateUpdateLogic
	{
		[ContextField] private BoxCollider m_boxCollider = null;
		public BoxCollider BoxCollider => m_boxCollider;

		[ContextField] private Rigidbody m_rigidbody = null;
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

		public void Activate() { }

		public void Deactivate() { }

		public void OnUpdate(float deltaTime, float timeScale)
		{
			Assert.AreEqual(1f, timeScale);
			UpdateCount += deltaTime;
		}

		public void OnFixUpdate(float deltaTime, float timeScale)
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