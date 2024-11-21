using System;
using System.Collections.Generic;
using UnityEngine;

namespace States.Core
{
    public class Key : ScriptableObject, IEquatable<Key>
    {
        private static Dictionary<int, Key> m_registeredKeys = new Dictionary<int, Key>();
        
        [SerializeField, HideInInspector] private int m_hash = default;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Clear() => m_registeredKeys = new Dictionary<int, Key>();

        private void OnEnable()
        {
            if(m_registeredKeys.ContainsKey(m_hash)) return;
            m_registeredKeys.Add(m_hash, this);
        }

        public static Key GetKey(int hash) => m_registeredKeys.GetValueOrDefault(hash);

        private void OnValidate() => m_hash = FNVHash(name);

        private void Reset() => m_hash = FNVHash(name);

        private int FNVHash(string value)
        {
            uint offset = 2166136261;
            uint primeNumber = 16777619;
            uint hash = offset;
			
            foreach (var VARIABLE in value)
            {
                hash ^= VARIABLE;
                hash *= primeNumber;
            }

            return (int)hash;
        }

        public static bool operator ==(Key a, Key b) => a.m_hash == b.m_hash;

        public static bool operator !=(Key a, Key b) => a.m_hash != b.m_hash;
        
        public bool Equals(Key other)
        {
            if (other is null) return false;
            return base.Equals(other) && m_hash == other.m_hash;
        }

        public override bool Equals(object obj) => obj is Key other && Equals(other);

        public override int GetHashCode() => m_hash;
    }
}