using UnityEngine;

namespace BaseGameLogic.States
{
    public class FromPrefabGraphProvider : BaseStateGraphProvider
    {
        [SerializeField] private GameObject _stateGraphPrefab = null;
        private StateGraph _graph;

        public override StateGraph Graph
        {
            get
            {
                if (_graph == null)
                    return GetGrapch();

                return _graph;
            }
        }

        private StateGraph GetGrapch()
        {
            if (_stateGraphPrefab != null)
            {
                var graphGameObject = Instantiate(_stateGraphPrefab, this.transform, false);
                graphGameObject.transform.localPosition = Vector3.zero;
                graphGameObject.transform.rotation = Quaternion.identity;
                graphGameObject.transform.localScale = Vector3.one;

                return _graph = graphGameObject.GetComponent<StateGraph>();
            }

            return null;
        }
    }
}