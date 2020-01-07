using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpVideoPlayerTimeBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialValue")]
        [SerializeField]
        private float _initialValueIOS;

        public float initialValueIOS
        {
            get => _initialValueIOS;
            set => _initialValueIOS = value;
        }

        [FormerlySerializedAs("targetValue")]
        [SerializeField]
        private float _targetValueIOS;

        public float targetValueIOS
        {
            get => _targetValueIOS;
            set => _targetValueIOS = value;
        }
        
        [FormerlySerializedAs("initialTimeAndroid")]
        [SerializeField]
        private float _initialValueAndroid;

        public float initialValueAndroid
        {
            get => _initialValueAndroid;
            set => _initialValueAndroid = value;
        }

        [FormerlySerializedAs("targetTimeAndroid")]
        [SerializeField]
        private float _targetValueAndroid;

        public float targetValueAndroid
        {
            get => _targetValueAndroid;
            set => _targetValueAndroid = value;
        }

#if UNITY_EDITOR
        public bool debugCurrentTime;
#endif
    }
}