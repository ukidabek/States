using UnityEngine;
using UnityEngine.Events;

namespace States.Core.Default
{
	[StateLogicPath("States/StateLogic")]
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