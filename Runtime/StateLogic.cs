using UnityEngine;

namespace Utilities.States
{
	public abstract class StateLogic : MonoBehaviour, IStateLogic
    {
        public virtual void Activate() {}
        public virtual void Deactivate() {}
        
        [ContextMenu("Change object name")]
        private void ChangeObjectName()
        {
            if (gameObject.GetComponents<StateLogic>().Length > 1) return;
            gameObject.name = GetType().Name;
        }
    }

    public abstract class StateLogicScriptableObject : ScriptableObject, IStateLogic
    {
        public virtual void Activate() {}
        public virtual void Deactivate() {}
    }
}