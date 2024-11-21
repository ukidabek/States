using System.Collections.Generic;

namespace States.Core.Test
{
	public class TestUpdateLogic : IStateLogic, IUpdateLogic
	{
		[ContextField] public TestBaseClass BaseClass = null;
		
		public bool CanBeDeactivated => true;

		public void Activate() { }

		public void Deactivate() { }
	}
}