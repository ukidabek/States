using NUnit.Framework;
using UnityEngine;

namespace States.Core.Test
{
    public class KeyTests
    {
        private Key m_keyA = null;
        private Key m_keyB = null;
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
            m_keyB = BuildKey("KeyB");
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

        [Test]
        public void Validate_If_Equals_Operator_Work_Correctly()
        {
            Key A = m_keyA, B = m_keyA;
            var result = A == B;
            Assert.IsTrue(result);
            A = m_keyB; 
            B = m_keyB;
            result = A == B;
            Assert.IsTrue(result);
            A = m_keyA;
            result = A == B;
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Validate_If_Not_Equals_Operator_Work_Correctly()
        {
            Key A = m_keyA, B = m_keyB;
            var result = A != B;
            Assert.IsTrue(result);
            A = m_keyB; 
            B = m_keyA;
            result = A != B;
            Assert.IsTrue(result);
            A = m_keyA;
            result = A != B;
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Validate_If_Equals_Method_Work_Correctly()
        {
            Key A = m_keyA, B = m_keyA;
            var result = A.Equals(B);
            Assert.IsTrue(result);
            A = m_keyB; 
            B = m_keyB;
            result = A.Equals(B);
            Assert.IsTrue(result);
            A = m_keyA;
            result = A.Equals(B);
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Validate_If_Generic_Equals_Method_Work_Correctly()
        {
            Key A = m_keyA, B = m_keyA;
            var result = A.Equals((object)B);
            Assert.IsTrue(result);
            A = m_keyB; 
            B = m_keyB;
            result = A.Equals((object)B);
            Assert.IsTrue(result);
            A = m_keyA;
            result = A.Equals((object)B);
            Assert.IsFalse(result);
        }
    }
}