using UnityEngine;

namespace Utilities.States.Default
{
    public abstract class SwitchStateCondition : MonoBehaviour, ISwitchStateCondition
    {
        public abstract bool Condition { get; }
        public virtual void Activate() {}
        public virtual void Deactivate() {}
    }
}