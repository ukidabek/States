using States.Core;
using UnityEngine;

namespace States.Default
{
    [CreateAssetMenu(fileName = "DedicatedBlackboardFactory", menuName = "States/Blackboard/DedicatedBlackboardFactory")]
    public class DedicatedBlackboardFactory : BlackboardFactory
    {
        public override IBlackboard CreateBlackboard() => new Blackboard(m_initialKeys);
    }
}