using System;

namespace Utilities.States
{
    [Obsolete("Logic moved to State Machine Manager. Set DefaultStateSetter field in that component!")]
    public class DefaultStateSetter : StateSetter
    {
        private void OnEnable() => SetState();

	}
}