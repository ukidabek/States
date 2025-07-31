using System.Collections.Generic;
using System.Linq;
using States.Core;

namespace States.Default
{
    public interface IReferenceBaker
    {
        IList<State> BackedStates { get; }
        IEnumerable<State> StateToBake { get; }

        public void Bake(IEnumerable<Context> context)
        {
            var contextHandler = new ContextHandler();
            BackedStates.Clear();
            var backedStates = StateToBake
                .Where(state => state.IsStatic)
                .Select(state =>
                {
                    (state as IReferenceBaker)?.Bake(context);
                    contextHandler.FillState(state, context);
                    BackedStates.Add(state);
                    return state;
                });
			
            foreach (var _ in backedStates) { }
        }

        public void Clear()
        {
            var contextHandler = new ContextHandler();
            var statesToClear = StateToBake
                .Where(state => state.IsStatic)
                .Select(state =>
                {
                    (state as IReferenceBaker)?.Clear();
                    contextHandler.ClearState(state);
                    return state;
                });
            
            foreach (var _ in statesToClear) { }
            BackedStates.Clear();
        }
    }
}