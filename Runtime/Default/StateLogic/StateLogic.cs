using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.States.Default
{
    public abstract class StateLogic : MonoBehaviour, IStateLogic
    {
        public virtual IEnumerable<IContextDestination> ContextDestinations { get; protected set; } = Array.Empty<IContextDestination>();

        public virtual bool CanBeDeactivated => true;

		public virtual void Activate() { }
        public virtual void Deactivate() { }

        [ContextMenu("Change object name")]
        private void ChangeObjectName()
        {
            if (gameObject.GetComponents<StateLogic>().Length > 1) return;
            gameObject.name = GetType().Name;
        }
    }

    public abstract class StateLogicScriptableObject : ScriptableObject, IStateLogic
    {
        public virtual IEnumerable<IContextDestination> ContextDestinations { get; protected set; } = Array.Empty<IContextDestination>();
		public virtual bool CanBeDeactivated => true;
		
        public virtual void Activate() { }
        public virtual void Deactivate() { }
    }
}