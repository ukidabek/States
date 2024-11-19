using System.Collections.Generic;
using System.Linq;

namespace Utilities.States
{
	public static class IStateHelper
	{
		public static IEnumerable<IContextDestination> GetContextDestination(this IState state)
		{
			var logic = state.Logic;
			var transitions = state.Transitions;
			return logic.OfType<IContextDestination>().Concat(transitions).Distinct();
		}
		
		public static void FillList<T>(this IEnumerable<IStateLogic> logic,  IList<T> listToFill) where T : IUpdateLogic
		{
			if(listToFill.Any()) return;
			var logicToSet = logic.OfType<T>();
			foreach (var logicToSetItem in logicToSet)
				listToFill.Add(logicToSetItem);
		}
	}
}