using UnityEngine;

namespace Utilities.States.Test
{
	public class StateLogic : IStateLogic
	{
		[ContextField] private BoxCollider m_boxCollider = null;
		public BoxCollider BoxCollider => m_boxCollider;

		[ContextField] private Rigidbody m_rigidbody = null;
		public Rigidbody Rigidbody => m_rigidbody;

		[ContextField("Test")] private Rigidbody m_rigidbodyWithID = null;
		public Rigidbody RigidbodyWithID => m_rigidbodyWithID;

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}
	}
}