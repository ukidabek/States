using NUnit.Framework;
using UnityEngine;

namespace States.Core.Test
{
    public class KeyTests
    {
        private Key m_keyA = null;
        
        public static Key BuildKey(string keyName)
        {
            var instance = ScriptableObject.CreateInstance<Key>();
            instance.name = keyName;
            TestUtilities.InvokeLogic<Key>("Reset", instance);
            return instance;
        }
        
        [SetUp]
        public void Setup()
        {
            m_keyA = BuildKey("KeyA");
        }
        
        [Test]
        public void Validate_If_Key_Is_Registered()
        {
            var hash = m_keyA.GetHashCode();
            TestUtilities.InvokeLogic<Key>("Clear");
            TestUtilities.InvokeLogic<Key>("OnEnable", m_keyA);

            var key = Key.GetKey(hash);

            Assert.AreEqual(m_keyA, key);
        }
    }
}