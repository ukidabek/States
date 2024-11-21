namespace States.Core
{
    public interface IOnFixedUpdateLogic : IUpdateLogic
	{
        void OnFixexUpdate(float deltaTime, float timeScale);
    }
}