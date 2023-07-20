using UnityEngine;
using UnityEngine.Events;

namespace Utilities.States
{
	public class UnityEventStateLogic : StateLogic
	{
		[SerializeField] private UnityEvent OnActivate = null;
		[SerializeField] private UnityEvent OnDeactivate = null;
		public override void Activate()
		{
			base.Activate();
			OnActivate.Invoke();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			OnDeactivate.Invoke();
		}
	}
}