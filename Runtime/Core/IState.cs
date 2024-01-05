using System.Collections.Generic;

namespace Utilities.States
{
    public interface IState
    {
        IStateID ID { get; }
        IEnumerable<IStateLogic> Logic { get; }
        void Enter();
        void Exit();
    }
}