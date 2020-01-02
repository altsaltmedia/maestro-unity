using System;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProCharSpacingBehaviour : LerpToTargetBehaviour
    {
        public float initialValue;
        public float targetValue;
    }
}