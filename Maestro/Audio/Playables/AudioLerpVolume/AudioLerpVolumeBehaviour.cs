using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioLerpVolumeBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialVolume")]
        [SerializeField]
        private float _initialValue;

        public float initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [FormerlySerializedAs("targetVolume")]
        [SerializeField]
        private float _targetValue;

        public float targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
    }
}