using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class DebugTimelineBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}