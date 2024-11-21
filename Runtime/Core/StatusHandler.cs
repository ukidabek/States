using System;
using UnityEngine;

namespace States.Core
{
	[Serializable]
	public abstract class StatusHandler
	{
		[SerializeField] protected bool m_status = false;
		public abstract UnityEngine.Object ManagedObject { get; }

		public abstract void Set();
		public abstract void Reset();

		public override int GetHashCode() => ManagedObject.GetHashCode();

		public override bool Equals(object obj) => ManagedObject == (obj as StatusHandler).ManagedObject;

		public static bool operator ==(StatusHandler a, StatusHandler b) => a.Equals(b);

		public static bool operator !=(StatusHandler a, StatusHandler b) => !a.Equals(b);
	}

	[Serializable]
	public class GenericStatusHandler : StatusHandler
	{
		[SerializeField] private UnityEngine.Object m_managedObject = null;
		public override UnityEngine.Object ManagedObject => m_managedObject;

		public override void Reset() => Set(!m_status);

		public override void Set() => Set(m_status);

		private void Set(bool status)
		{
			switch (m_managedObject)
			{
				case Behaviour behaviour:
					behaviour.enabled = status;
					break;
				case GameObject gameObject:
					gameObject.SetActive(status);
					break;
				case Rigidbody body:
					body.isKinematic = !status;
					break;
				case Rigidbody2D body:
					body.isKinematic = !status;
					break;
			}
		}

		public GenericStatusHandler()
		{
		}

		public GenericStatusHandler(StatusHandler managedObject)
		{
			m_managedObject = managedObject.ManagedObject;
		}
	}


	[Serializable]
	public abstract class StatusHandler<T> : StatusHandler where T : UnityEngine.Object, new()
	{
		[SerializeField] protected T m_managedObject = default;

		protected StatusHandler(T managedObject, bool status)
		{
			m_status = status;
			m_managedObject = managedObject;
		}

		public override UnityEngine.Object ManagedObject => m_managedObject;
	}
}