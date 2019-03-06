using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class RectTransformRotationBehaviour : LerpToTargetBehaviour
    {
        public Vector3 initialRotation = new Vector3(0, 0, 0);
        public Vector3 targetRotation = new Vector3(0, 0, 0);
    }
}