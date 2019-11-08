using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class LerpFloatVarBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}