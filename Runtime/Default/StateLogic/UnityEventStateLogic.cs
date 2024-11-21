using States.Core;
using UnityEngine;
using UnityEngine.Events;

namespace States.Default
{
	[StateLogicPath("States/StateLogic")]
	public class UnityEventStateLogic : IStateLogic
	{
		[SerializeField] private UnityEvent OnActivate = null;
		[SerializeField] private UnityEvent OnDeactivate = null;
		
		public bool CanBeDeactivated => true;

		public void Activate()
		{
			OnActivate.Invoke();
		}

		public void Deactivate()
		{
			OnDeactivate.Invoke();
		}
	}
}