using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaseGameLogic.States.Utility.Assembly
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

        public static Type[] GetDerivedTypes(Type baseType, bool isAbstract = false) 
        {
            return baseType.Assembly.GetTypes().Where(type => (type.IsSubclassOf(baseType) && type.IsAbstract == isAbstract)).ToArray();
        }

        public static FieldInfo[] GetAllFieldsWithAttribute<Type, AttributeType>()
        {
            return GetAllFieldsWithAttribute(typeof(Type), typeof(AttributeType));
        }

        private static FieldInfo[] GetAllFieldsWithAttribute(Type inType, Type attributeType)
        {
            return inType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(
                field => field.GetCustomAttributes(false).Any(attribute => attribute.GetType() == attributeType)).ToArray();
        }

        public static List<FieldInfo> GetAllFieldsWithAttribute(Type inType, Type attributeType, Type baseType = null)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            var currentType = baseType;
            if (inType == currentType)
                return fields;

            fields.AddRange(GetAllFieldsWithAttribute(inType, attributeType));
            fields.AddRange(GetAllFieldsWithAttribute(inType.BaseType, attributeType, baseType));

            return fields;
        }
    }
}