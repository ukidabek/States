using UnityEngine;

namespace BaseGameLogic.States
{
    internal class TransitionInfo
    {
        public Rect Rect;
        public int NodeIndex = 0;
        public int TransitionIndex = 0;

        public TransitionInfo(Rect rect, int nodeInde, int transitionIndex)
        {
            Rect = rect;
            NodeIndex = nodeInde;
            TransitionIndex = transitionIndex;
        }
    }
}