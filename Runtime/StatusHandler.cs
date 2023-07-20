using System;
using UnityEngine;

namespace Stages
{
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
	public abstract class StatusHandler<T> : StatusHandler where T : UnityEngine.Object, new()
	{
		[SerializeField] protected T m_managedObject = default;

		protected StatusHandler(T managedObject, bool status)
		{
			this.m_status = status;
			this.m_managedObject = managedObject;
		}

		public override UnityEngine.Object ManagedObject => m_managedObject;
	}
}