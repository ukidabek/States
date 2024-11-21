using UnityEngine;

namespace States.Core.Test
{
    public class TestStateTransition : IStateTransition
    {
        [ContextField] public Rigidbody Rigidbody = null;

        public IState StateToEnter { get; set; }
        public bool Result { get; set; } = false;
        
        public bool Validate() => Result;
    }
}