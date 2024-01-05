using System.Collections.Generic;
using System.Linq;

namespace Utilities.States
{
    public static class IStateHelper
    {
        public static IEnumerable<IContextDestination> GetContextDestination(this IState state)
        {
            var logic = state.Logic;
            return logic.OfType<IContextDestination>().Concat(logic.SelectMany(logic => logic.ContextDestinations));
        }
    }
}