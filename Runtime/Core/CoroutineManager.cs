using System.Collections;
using UnityEngine;

namespace Utilities.States
{
    public class CoroutineManager
    {
        private readonly MonoBehaviour _coroutineRunnerMonoBehaviour;
        private IEnumerator _coroutine;

        public CoroutineManager(MonoBehaviour coroutineRunnerMonoBehaviour)
        {
            _coroutineRunnerMonoBehaviour = coroutineRunnerMonoBehaviour;
        }

        public CoroutineManager(MonoBehaviour coroutineRunnerMonoBehaviour, IEnumerator coroutine)
        {
            _coroutineRunnerMonoBehaviour = coroutineRunnerMonoBehaviour;
            _coroutine = coroutine;
        }

        private void ManageCoroutine(IEnumerator enumerator)
        {
            Stop();
            SetCoroutine(enumerator);
            _coroutineRunnerMonoBehaviour.StartCoroutine(_coroutine);
        }

        public void SetCoroutine(IEnumerator enumerator) => _coroutine = enumerator;

        public void Stop()
        {
            if (_coroutine != null)
                _coroutineRunnerMonoBehaviour.StopCoroutine(_coroutine);
        }

        public void Run(IEnumerator enumerator)
        {
            ManageCoroutine(enumerator);
        }
        
        public void Run()
        {
            if (_coroutine != null)
                ManageCoroutine(_coroutine);
        }
    }
}