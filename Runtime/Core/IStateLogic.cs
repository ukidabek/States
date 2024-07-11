using System;
using System.Collections.Generic;

namespace Utilities.States
{
    public interface IStateLogic : IContextDestination
    {
        IEnumerable<IContextDestination> ContextDestinations { get; }
        bool CanBeDeactivated { get; }
		void Activate();
        void Deactivate();
    }
}