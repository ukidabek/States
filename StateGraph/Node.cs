using UnityEngine;

using System;

namespace BaseGameLogic.States
{
    [Serializable]
    public class Node
    {
        public Rect Rect = new Rect(Vector2.zero, new Vector2(120, 30));
        public BaseState State = null;

        public bool IsDragged = false;
        public bool IsSelected = false;
        public bool RemoveNode = false;
        public bool IsReference = false;

        public ConnectionPoint In = null;
        public ConnectionPoint Out = null;

        [SerializeField] private Color _bacgroundColor = Color.white;
        public Color BacgroundColor
        {
            get { return _bacgroundColor; }
            set { _bacgroundColor = value; }
        }

        public event Action<ConnectionPointType, BaseState> OnConnectionPointClicked = null;

        public Node()
        {
            CalculateConnectionPointPosition();
            GetRandomColor();
        }

        private void GetRandomColor()
        {
            System.Random random = new System.Random();

            float r = (float)(random.NextDouble() * (1 - 0) + 0);
            float g = (float)(random.NextDouble() * (1 - 0) + 0);
            float b = (float)(random.NextDouble() * (1 - 0) + 0);

            _bacgroundColor = new Color(r, g, b, .75f);
        }

        private void CalculateConnectionPointPosition()
        {
            Vector2 pointPosition = new Vector2(Rect.x, Rect.y + Rect.height / 2);
            In = new ConnectionPoint(pointPosition);
            pointPosition = new Vector2(Rect.x + Rect.width, Rect.y + Rect.height / 2);
            Out = new ConnectionPoint(pointPosition);
        }

        public Node(Vector2 position, BaseState state)
        {
            Rect.position = position;
            CalculateConnectionPointPosition();
            State = state;
            GetRandomColor();
        }

        public Node(Vector2 position, Node node)
        {
            Rect.position = position;
            CalculateConnectionPointPosition();
            _bacgroundColor = node._bacgroundColor;
            State = node.State;
            IsReference = true;
        }

        public void Draw()
        {
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = IsSelected ? Color.red : _bacgroundColor;

            GUI.Box(Rect, State != null ? State.GetType().Name : "Any state");

            GUI.backgroundColor = oldColor;

            if (In.Draw() && OnConnectionPointClicked != null)
            {
                OnConnectionPointClicked(ConnectionPointType.In, State);
            }

            if (Out.Draw() && OnConnectionPointClicked != null)
            {
                OnConnectionPointClicked(ConnectionPointType.Out, State);
            }
        }

        public bool ProcessEvents(Event currentEvent, Vector2 offset)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    bool contains = Rect.Contains(currentEvent.mousePosition - offset);
                    IsSelected = false;
                    switch(currentEvent.button)
                    {
                        case 0:
                        case 1:
                            if (contains)
                            {
                                IsSelected = IsDragged = true;
                                return true;
                            }
                            break;
                    }
                    break;

                case EventType.MouseUp:
                    IsDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (currentEvent.button == 0 && IsDragged)
                    {
                        Drag(currentEvent.delta);
                        currentEvent.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void Drag(Vector2 delta)
        {
            Rect.position += delta;
            In.Rect.position += delta;
            Out.Rect.position += delta;
        }

        public void Remove()
        {
            GameObject.DestroyImmediate(State);
            RemoveNode = true;
        }
    }
}