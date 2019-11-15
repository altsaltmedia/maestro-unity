using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformScaleBehaviour : LerpToTargetBehaviour
    {
        public Vector3 initialScale = new Vector3(0, 0, 0);
        public Vector3 targetScale = new Vector3(0, 0, 0);
    }
}