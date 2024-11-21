using States.Core;

namespace States.Default
{
    public interface IOnFixedUpdateLogic : IUpdateLogic
	{
        void OnFixedUpdate(float deltaTime, float timeScale, Blackboard blackboard);
    }
}