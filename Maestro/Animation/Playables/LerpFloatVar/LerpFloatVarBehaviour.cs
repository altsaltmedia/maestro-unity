using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpFloatVarBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialValue")]
        [SerializeField]
        private float _initialValue;

        public float initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [FormerlySerializedAs("targetValue")]
        [SerializeField]
        private float _targetValue;

        public float targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }
    }
}