using System.Collections.Generic;
using States.Core;
using UnityEngine;
using Utilities.General;

namespace States.Default
{
    public abstract class BlackboardFactory : ScriptableObject
    {
        [SerializeField] protected List<Key> m_initialKeys = new List<Key>();
        public abstract IBlackboard CreateBlackboard();

        protected virtual void OnEnable() { }
    }
}