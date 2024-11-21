using NUnit.Framework;

namespace States.Core.Test
{
    public class BlackboardTests
    {
        private class TestClass
        {
        }

        private struct TestStruct
        {
            public int Value { get; set; }
        }
        
        private Key m_keyA = null;
        
        private Blackboard m_blackboard = null;
        
        [SetUp]
        public void Setup()
        {
            m_keyA = KeyTests.BuildKey("KeyA");
            m_blackboard = new Blackboard();
        }

        [Test]
        public void Validate_If_Return_False_If_Key_Is_Not_Found()
        {
            var result = m_blackboard.ContainsKey(m_keyA);
            Assert.IsFalse(result);
        }

        [Test]
        public void Validate_If_To_Get_Value_When_Key_Is_Not_Found_Set_The_Default_Value()
        {
            Validate_If_Return_False_If_Key_Is_Not_Found();
            var defaultValue = 10;
            var result = m_blackboard.GetValue(m_keyA, defaultValue);
            Assert.AreEqual(defaultValue, result);
            Validate_If_Return_False_If_Key_Is_Not_Found();
        }

        [Test]
        public void Validate_If_Setting_Value_Works_Properly()
        {
            Validate_If_Return_False_If_Key_Is_Not_Found();
            int valueA = 10, valueB = 20;
            m_blackboard.SetValue(m_keyA, valueA);
            var containsKey = m_blackboard.ContainsKey(m_keyA);
            Assert.IsTrue(containsKey);
            var result = m_blackboard.GetValue<int>(m_keyA);
            Assert.AreEqual(valueA, result);
            m_blackboard.SetValue(m_keyA, valueB);
            result = m_blackboard.GetValue<int>(m_keyA);
            Assert.AreEqual(valueB, result);
        }

        [Test]
        public void Validate_If_Reference_Value_Will_Be_Returned_Properly()
        {
            var testObject = new TestClass();
            m_blackboard.SetValue(m_keyA, testObject);
            var result = m_blackboard.GetValue<TestClass>(m_keyA);
            Assert.AreSame(testObject, result);
        }

        [Test]
        public void Validate_If_Result_Will_Be_Null_When_Key_Contains_Non_Reference_Value()
        {
            var value = 10;
            m_blackboard.SetValue(m_keyA, value);
            m_blackboard.TryGetValue<TestClass>(m_keyA, out var result);
            Assert.IsNull(result);
            result = m_blackboard.GetValue<TestClass>(m_keyA);
            Assert.IsNull(result);
        }
        
        [Test]
        public void Validate_If_Struct_Value_Will_Be_Returned_Properly()
        {
            var testStruct = new TestStruct();
            m_blackboard.SetValue(m_keyA, testStruct);
            var result = m_blackboard.GetValue<TestStruct>(m_keyA);
            Assert.AreEqual(testStruct.Value, result.Value);
            testStruct.Value = 15;
            Assert.AreNotEqual(testStruct.Value, result.Value);
        }
    }
}