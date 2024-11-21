namespace States.Core
{
    public interface IOnUpdateLogic : IUpdateLogic
	{
        void OnUpdate(float deltaTime, float timeScale);
    }
}