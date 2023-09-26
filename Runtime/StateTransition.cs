using System;

namespace Utilities.States
{
	public class StateTransition : IStateTransition
	{
		protected interface ITransitionCondition
		{
			bool Validate(IState fromA, IState fromB, IState toA, IState toB);
		}

		private class FormTo : ITransitionCondition
		{
			public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => fromA == fromB && toA == toB;
		}

		private class Form : ITransitionCondition
		{
			public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => fromA == fromB;
		}

		private class To : ITransitionCondition
		{
			public bool Validate(IState fromA, IState fromB, IState toA, IState toB) => toA == toB;
		}

		private IState m_formState = null, m_toState = null;
		private Action m_perform = null, m_cancel = null;

		private ITransitionCondition m_condition = null;

		public StateTransition(IState formState, IState toState, TransitionMode condition, Action perform, Action cancel)
		{
			m_formState = formState;
			m_toState = toState;
			m_perform = perform;
			m_cancel = cancel;

			m_condition = condition switch
			{
				TransitionMode.FromTo => new FormTo(),
				TransitionMode.From => new Form(),
				TransitionMode.To => new To(),
				_ => null,
			};
		}

		public void Cancel() => m_cancel();

		public void Perform(IState from, IState to)
		{
			if (m_condition.Validate(m_formState, from, m_toState, to))
				m_perform();
		}
	}
}