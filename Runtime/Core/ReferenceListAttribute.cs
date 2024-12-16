using UnityEngine;

namespace States.Core
{
    public class ReferenceListAttribute :
#if UNITY_2023
        PropertyCollectionAttribute
#else
        PropertyAttribute
#endif
    {
#if UNITY_6000
        public ReferenceListAttribute() : base(true)
        {
        }
#endif
    }
}