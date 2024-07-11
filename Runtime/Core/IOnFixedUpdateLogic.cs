namespace Utilities.States
{
    public interface IOnFixedUpdateLogic : IUpdateLogic
	{
        void OnFixUpdate(float deltaTime, float timeScale);
    }
}