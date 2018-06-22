using UnityEngine;

namespace BaseGameLogic.States
{
    public abstract class BaseStateGraphProvider : MonoBehaviour
    {
        [SerializeField] private StateHandler _stateHandler = null;
        public abstract StateGraph Graph { get; }

        private void Awake()
        {
            _stateHandler.Graph = Graph;
        }
    }
}