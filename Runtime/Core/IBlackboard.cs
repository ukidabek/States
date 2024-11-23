namespace States.Core
{
    public interface IBlackboard
    {
        bool ContainsKey(Key key);
        T GetValue<T>(Key key, T defaultValue = default);
        bool TryGetValue<T>(Key key, out T value);
        void SetValue(Key key, object value);
    }
}