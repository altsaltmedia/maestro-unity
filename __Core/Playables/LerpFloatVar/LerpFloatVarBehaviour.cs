using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class LerpFloatVarBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}