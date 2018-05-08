using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace BaseGameLogic.AI.Sensors
{
    public abstract class BaseTargetSelector : MonoBehaviour
    {
        public abstract GameObject SelecTarget(Queue<GameObject> targetQueue);
    }
}