namespace Utilities.States
{
    public interface IStateLogicExecutor
    {
        bool Enabled { get; set; }
        void SetLogicToExecute(IState state);
    }
}