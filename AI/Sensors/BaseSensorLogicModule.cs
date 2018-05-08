using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using BaseGameLogic.LogicModule;
using UnityEngine.Events;

namespace BaseGameLogic.AI.Sensors
{
    public class BaseSensorLogicModule : BaseLogicModule
    {
        [SerializeField] private Queue<GameObject> _targetQueue = new Queue<GameObject>();
        [SerializeField] private GameObject _target = null;
        public GameObject Target { get { return _target; } }

        public TargetChangedUnityEvent TargetChanged = new TargetChangedUnityEvent();

        [SerializeField, Space] private BaseTargetSelector _targetSelector = null;


        public void TargetDetected(GameObject target)
        {
            if(!_targetQueue.Contains(target))
                _targetQueue.Enqueue(target);
        }

        protected override void Update()
        {
            GameObject newTarget = _targetSelector.SelecTarget(_targetQueue);
            if(_target != newTarget)
            {
                _target = newTarget;
                TargetChanged.Invoke(newTarget);
            }
        }
    }

    [System.Serializable]
    public class TargetChangedUnityEvent : UnityEvent<GameObject> { }
}
