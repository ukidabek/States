using System.Collections.Generic;
using System.Linq;

namespace States.Core
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
		
		public static void Process(this IEnumerable<IStateProcessor> processors, IState state)
		{
			if (processors == null) return;
			foreach (var preProcessor in processors)
				preProcessor.Process(state);
		}
	}
}