using System.Reflection;

namespace States.Core.Test
{
    public static class TestUtilities
    {
        public static object InvokeLogic<T>(string methodName, object target = null, object[] data = null, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        {
            var type = typeof(T);
            var cacheLogic = type.GetMethod(methodName, bindingFlags);
            return cacheLogic.Invoke(target, data);
        }
    }
}