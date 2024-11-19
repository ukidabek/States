using System.Collections.Generic;

namespace Utilities.States.Test
{
	public class TestUpdateLogic : IStateLogic, IUpdateLogic
	{
		[ContextField] public TestBaseClass BaseClass = null;
		
		public bool CanBeDeactivated => true;

		public void Activate() { }

		public void Deactivate() { }
	}
}