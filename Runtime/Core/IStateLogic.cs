using System;
using System.Collections.Generic;

namespace Utilities.States
{
    public interface IStateLogic : IContextDestination
    {
        IEnumerable<IContextDestination> ContextDestinations => Array.Empty<IContextDestination>();
        void Activate();
        void Deactivate();
    }
}