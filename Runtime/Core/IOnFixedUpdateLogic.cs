namespace Utilities.States
{
    public interface IOnFixedUpdateLogic : IUpdateLogic
	{
        void OnFixexUpdate(float deltaTime, float timeScale);
    }
}