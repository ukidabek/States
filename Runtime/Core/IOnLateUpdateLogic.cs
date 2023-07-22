namespace Utilities.States
{
	public interface IOnLateUpdateLogic : IUpdateLogic
    {
        void OnLateUpdate(float deltaTime, float timeScale);
    }
}