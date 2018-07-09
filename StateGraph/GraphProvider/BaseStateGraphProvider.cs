using UnityEngine;

namespace BaseGameLogic.States.Graph.Providers
{
    public abstract class BaseStateGraphProvider : MonoBehaviour
    {
        [SerializeField] private StateHandler _stateHandler = null;
        public abstract StateGraph Graph { get; }

        private void Awake()
        {
            Graph.Handler = _stateHandler;
            _stateHandler.KeepStatesOnStack = Graph.Type == GraphType.Stack;
        }

        private void Start()
        {
            _stateHandler.EnterState(Graph.RootState);
        }
    }
}