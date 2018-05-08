using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaseGameLogic.States.Assembly
{
    public static class StatesAssemblyExtension
    {
        /// <summary>
        /// Returns all types that extend type T.
        /// </summary>
        /// <typeparam name="T">Base type.</typeparam>
        /// <returns>List of derived types.</returns>
        public static Type[] GetDerivedTypes<T>()
        {
            return GetDerivedTypes(typeof(T));
        }

        public static Type[] GetDerivedTypes(Type baseType)
        {
            return baseType.Assembly.GetTypes().Where(type => (type.IsSubclassOf(baseType) && !type.IsAbstract)).ToArray();
        }

        public static FieldInfo[] GetAllFieldsWithAttribute<Type, AttributeType>()
        {
            return GetAllFieldsWithAttribute(typeof(Type), typeof(AttributeType));
        }
        public static FieldInfo[] GetAllFieldsWithAttribute<T>(this Type type)
        {
            return GetAllFieldsWithAttribute(type, typeof(T));
        }

        private static FieldInfo[] GetAllFieldsWithAttribute(Type inType, Type attributeType)
        {
            return inType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(
                field => field.GetCustomAttributes(false).Any(attribute => attribute.GetType() == attributeType)).ToArray();
        }

        public static List<FieldInfo> GetAllFieldsWithAttribute(Type inType, Type attributeType, bool inBaseType = false)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            if (inType == typeof(MonoBehaviour)) return fields;

            fields.AddRange(GetAllFieldsWithAttribute(inType, attributeType));
            if (inBaseType)
                fields.AddRange(GetAllFieldsWithAttribute(inType.BaseType, attributeType, inBaseType));

            return fields;
        }
    }
}