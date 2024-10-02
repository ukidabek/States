using System.Collections.Generic;

namespace Utilities.States
{
    public interface IState
    {
        public bool IsStatic { get; }
        string Name { get; }
        IStateID ID { get; }
        IEnumerable<IStateLogic> Logic { get; }
        void Enter();
        bool CanExit { get; }
        void Exit();
    }
}