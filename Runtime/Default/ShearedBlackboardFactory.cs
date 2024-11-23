using States.Core;
using UnityEngine;

namespace States.Default
{
    [CreateAssetMenu(fileName = "ShearedBlackboardFactory", menuName = "States/Blackboard/ShearedBlackboardFactory")]
    public class ShearedBlackboardFactory : BlackboardFactory
    {
        private IBlackboard m_blackboard;
		
        protected override void OnEnable()
        {
            base.OnEnable();
            m_blackboard = null;
        }

        public override IBlackboard CreateBlackboard() => m_blackboard ??= new Blackboard(m_initialKeys);
    }
}