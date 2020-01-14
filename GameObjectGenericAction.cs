using System;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro
{
    [Serializable]
    public class GameObjectGenericAction : UnityEvent<GameObject> { }
}