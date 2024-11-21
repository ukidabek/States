using States.Core;

namespace States.Default
{
    public interface IOnUpdateLogic : IUpdateLogic
	{
        void OnUpdate(float deltaTime, float timeScale, Blackboard blackboard);
    }
}