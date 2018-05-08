using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BaseGameLogic.AI.Sensors
{
    public abstract class BaseSensor : MonoBehaviour
    {
        [SerializeField] protected TargetDetectedUnityEvent targetDetected = new TargetDetectedUnityEvent();

        public virtual bool PositiveReading() { return false; }
    }

    [System.Serializable]
    public class TargetDetectedUnityEvent : UnityEvent<GameObject> {}
}