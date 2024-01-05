using System.Collections.Generic;
using UnityEngine;

namespace Utilities.States.Test
{
    public class TestStateLogic : IStateLogic
	{
        public class TestClass : IContextDestination
		{
			[ContextField] private Rigidbody rigidbody1;
			public Rigidbody Rigidbody => rigidbody1;

			[ContextField("Test_1")] public Rigidbody Rigidbody2;
		} 

		public TestClass Test = new TestClass();

        public TestStateLogic()
        {
			ContextDestinations = new [] {Test };
        }

		public IEnumerable<IContextDestination> ContextDestinations {get; private set;}

		public void Activate() { }

		public void Deactivate() { }
	}
}