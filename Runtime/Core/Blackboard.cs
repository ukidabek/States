using System.Collections.Generic;

namespace States.Core
{
    public class Blackboard
    {
        public Blackboard() { }

        public Blackboard(IEnumerable<Key> keys)
        {
            foreach (var key in keys) 
                m_blackboardData.Add(key, default);
        }
        
        private Dictionary<Key, object> m_blackboardData = new Dictionary<Key, object>();

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
            if (m_blackboardData.TryAdd(key, value)) return;
            m_blackboardData[key] = value;
        }
    }
}