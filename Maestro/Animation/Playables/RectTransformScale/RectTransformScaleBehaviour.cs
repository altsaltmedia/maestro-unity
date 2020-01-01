using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformScaleBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialScale")]
        [SerializeField]
        private Vector3 _initialValue = new Vector3(0, 0, 0);

        public Vector3 initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [FormerlySerializedAs("targetScale")]
        [SerializeField]
        private Vector3 _targetValue = new Vector3(0, 0, 0);

        public Vector3 targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }
    }
}