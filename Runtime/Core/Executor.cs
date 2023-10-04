using System;

namespace Utilities.States
{
	[Flags]
	public enum Executor
	{
		Update = 1 << 0, 
		LateUpdate = 1 << 1,
		FixUpdate = 1 << 2
	}
}
