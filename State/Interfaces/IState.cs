namespace BaseGameLogic.States
{
    public interface IState
    {
        StateHandler ControlledObject { get; set; }
        void OnEnter();
        void OnExit();
    }
}