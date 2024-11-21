namespace States.Core
{
	public interface IOnLateUpdateLogic : IUpdateLogic
    {
        void OnLateUpdate(float deltaTime, float timeScale);
    }
}