using States.Core;

namespace States.Default
{
	public interface IOnLateUpdateLogic : IUpdateLogic
    {
        void OnLateUpdate(float deltaTime, float timeScale, IBlackboard blackboard);
    }
}