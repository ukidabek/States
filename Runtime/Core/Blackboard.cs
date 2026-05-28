using System.Collections.Generic;
using UnityEngine.Events;
using Utilities.General;

namespace States.Core
{
    public class Blackboard : IBlackboard
    {
        public Blackboard()
        {
        }

        public Blackboard(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
                m_blackboardData.Add(key, default);
        }

        private readonly Dictionary<Key, object> m_blackboardData = new Dictionary<Key, object>(10);
        private readonly Dictionary<Key, UnityAction<object>> m_onValueChangeListeners = new Dictionary<Key, UnityAction<object>>(10);

        public bool ContainsKey(Key key) => m_blackboardData.ContainsKey(key);

        public T GetValue<T>(Key key, T defaultValue = default)
        {
            if (TryGetValue(key, out object value))
                return value is T result ? result : default;
            
            return defaultValue;
        }

        public bool TryGetValue<T>(Key key, out T value)
        {
            if (m_blackboardData.TryGetValue(key, out var data) && data is T typedData)
            {
                value = typedData;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue(Key key, object value)
        {
            if (m_blackboardData.TryAdd(key, value))
            {
                InvokeDelegateIfExist(key, value);
                return;
            }

            var odlValue = m_blackboardData[key];

            if (odlValue != null && odlValue.Equals(value)) return;

            m_blackboardData[key] = value;
            InvokeDelegateIfExist(key, value);
            
            return;

            void InvokeDelegateIfExist(Key key, object value)
            {
                if (!m_onValueChangeListeners.TryGetValue(key, out var action)) return;
                action(value);
            }
        }

        public void AddOnValueChangeListener(Key key, UnityAction<object> action)
        {
            if (m_onValueChangeListeners.TryAdd(key, action)) return;
            m_onValueChangeListeners[key] += action;
        }

        public void RemoveOnValueChangeListener(Key key, UnityAction<object> action)
        {
            if (!m_onValueChangeListeners.ContainsKey(key)) return;
            m_onValueChangeListeners[key] -= action;
        }
    }
}