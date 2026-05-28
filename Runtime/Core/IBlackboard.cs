using UnityEngine.Events;
using Utilities.General;

namespace States.Core
{
    public interface IBlackboard
    {
        bool ContainsKey(Key key);
        T GetValue<T>(Key key, T defaultValue = default);
        bool TryGetValue<T>(Key key, out T value);
        void SetValue(Key key, object value);
        void AddOnValueChangeListener(Key key, UnityAction<object> action);
        void RemoveOnValueChangeListener(Key key, UnityAction<object> action);
    }
}