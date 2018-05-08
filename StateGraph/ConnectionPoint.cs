using UnityEngine;

using System;

namespace BaseGameLogic.States
{
    [Serializable]
    public partial class ConnectionPoint
    {
        public Rect Rect = new Rect();

        public ConnectionPoint()
        {
            Rect.size = new Vector2(10, 10);
        }

        public ConnectionPoint(Vector2 postion)
        {
            Rect.size = new Vector2(10, 10);
            Rect.center = postion;
        }

        public bool Draw()
        {
            return GUI.Button(Rect, "");
        }
    }
}