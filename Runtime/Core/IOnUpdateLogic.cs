namespace Utilities.States
{
    public interface IOnUpdateLogic : IUpdateLogic
	{
        void OnUpdate(float deltaTime, float timeScale);
    }
}