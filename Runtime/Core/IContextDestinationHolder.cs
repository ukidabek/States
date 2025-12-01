using System.Collections.Generic;

namespace States.Core
{
    public interface IContextDestinationHolder
    {
        IEnumerable<IContextDestination> ContextDestinations { get; }
    }
}