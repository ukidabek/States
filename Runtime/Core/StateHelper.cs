using System.Collections.Generic;

namespace Utilities.States
{
	public static class StateHelper
    {
        public static void Process(this IEnumerable<IStateProcessor> processors, IState state)
        {
            if (processors == null) return;

            foreach (var preProcessor in processors)
                preProcessor.Process(state);
        }
    }
}