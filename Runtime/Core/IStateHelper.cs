using System.Collections.Generic;
using System.Linq;

namespace Utilities.States
{
	public static class IStateHelper
	{
		public static IEnumerable<IContextDestination> GetContextDestination(this IState state)
		{
			var logic = state.Logic;
			var contextDestination = logic.OfType<IContextDestination>();
			var otherContextDestination = logic.SelectMany(logic => logic.ContextDestinations);
			return contextDestination.Concat(otherContextDestination).Distinct();
		}
	}
}