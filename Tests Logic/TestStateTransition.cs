using UnityEngine;

namespace Utilities.States.Test
{
    public class TestStateTransition : IStateTransition
    {
        [ContextField] public Rigidbody Rigidbody = null;

        public IState StateToEnter { get; set; }
        public bool Result { get; set; } = false;
        
        public bool Validate() => Result;
    }
}