namespace Utilities.States
{
    public interface IOnFixUpdateLogic : IUpdateLogic
	{
        void OnFixUpdate(float deltaTime, float timeScale);
    }
}