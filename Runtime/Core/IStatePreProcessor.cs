namespace States.Core
{
	public interface IStateProcessor
	{
		void Process(IState state);
	}

	public interface IStatePreProcessor : IStateProcessor
	{
	}

	public interface IStatePostProcessor : IStateProcessor
	{
	}
}