using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}