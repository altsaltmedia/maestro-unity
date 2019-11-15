using System;
using UnityEngine;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpFloatVarBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}