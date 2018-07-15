using UnityEngine;

using System;
using System.Linq;
using System.Reflection;

namespace BaseGameLogic.States.Utility
{
    using Assembly = System.Reflection.Assembly;
    [Serializable] public class StateInfo
    {
        [SerializeField] TypeInfo _type = null;
        public TypeInfo Type { get { return _type; } }

        [SerializeField] private ConstructorParameter[] _parameters = null;
        public ConstructorParameter[] Parameters { get { return _parameters; } }

        [Serializable] public class TypeInfo
        {
            [SerializeField] private string _assemblFullName = string.Empty;
            public string AssemblFullName { get { return _assemblFullName; } }

            [SerializeField] private string _typeName = string.Empty;
            public string TypeName { get { return _typeName; } }

            private Type _type = null;

            public TypeInfo(Type type)
            {
                _assemblFullName = type.Assembly.GetName().FullName;
                _typeName = type.FullName;
            }



            private bool AssemblyQuiry(Assembly assembly)
            {
                return assembly.GetName().FullName == _assemblFullName;
            }

            public static implicit operator Type(TypeInfo stateInfo)
            {
                if (string.IsNullOrEmpty(stateInfo._assemblFullName) || string.IsNullOrEmpty(stateInfo._typeName))
                    return null;

                if (stateInfo._type == null)
                {
                    Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(stateInfo.AssemblyQuiry);
                    stateInfo._type = assembly.GetType(stateInfo._typeName);
                }

                return stateInfo._type;
            }
        }

        [Serializable] public class ConstructorParameter
        {
            [SerializeField] private string _parameterName = string.Empty;
            public string ParameterName { get { return _parameterName; } }

            [SerializeField] private TypeInfo _type = null;
            public TypeInfo Type { get { return _type; } }


            public UnityEngine.Object ObjectValue;
            public int IntValue = 0;

            public ConstructorParameter(ParameterInfo parameterInfo)
            {
                _type = new TypeInfo(parameterInfo.ParameterType);
                _parameterName = parameterInfo.Name;
            }

            public override int GetHashCode()
            {
                int value = 0;
                for (int i = 0; i < _parameterName.Length; i++)
                    value += _parameterName[i];
                return value;
            }

            public object GetObject()
            {
                Type type = _type;
                switch (type.Name)
                {
                    case "Int32":
                        return IntValue;
                    default:
                        return ObjectValue;
                }
            }
        }

        public StateInfo() { }
        public StateInfo(Type type)
        {
            _type = new TypeInfo(type);
        }

        public StateInfo(ConstructorInfo info) : this(info.DeclaringType)
        {
            var parameters = info.GetParameters();
            _parameters = new ConstructorParameter[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                _parameters[i] = new ConstructorParameter(parameters[i]);
            }
        }

        public IState GetInstance()
        {
            object[] data = new object[_parameters.Length];

            for (int i = 0; i < _parameters.Length; i++)
                data[i] = _parameters[i].GetObject();

            return Activator.CreateInstance(_type, data) as IState;
        }

        public override int GetHashCode()
        {
            int value = 0;
            for (int i = 0; i < _type.TypeName.Length; i++)
                value += _type.TypeName[i];

            for (int i = 0; i < _parameters.Length; i++)
                value += _parameters[i].GetHashCode();

            return value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return GetHashCode() == obj.GetHashCode();
        }

        public static bool operator !=(StateInfo a, StateInfo b)
        {
            return !(a == b);
        }

        public static bool operator ==(StateInfo a, StateInfo b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            else if ((ReferenceEquals(a, null) && !ReferenceEquals(b, null)) ||
                (!ReferenceEquals(a, null) && ReferenceEquals(b, null)))
                return false;

            return a.Equals(b);
        }
    }
}