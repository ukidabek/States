using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace States.Default
{
    [Serializable, Preserve]
    public class LogBlackboardValueChangeHandler : IBlackboardValueChangeHandler
    {
        public void HandleValueChange(object value) => Debug.Log($"Blackboard value is:{value}");
    }
}