﻿namespace States.Core
{
    public interface IStateTransition : IContextDestination
    {
        IState StateToEnter { get; }
        bool Validate(IBlackboard blackboard);
    }
}