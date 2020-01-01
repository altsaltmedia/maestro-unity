using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatBehaviour : LerpToTargetBehaviour
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

#if UNITY_EDITOR
        
        public override object SetInitialValueToTarget()
        {
            initialValue = targetValue;
            return initialValue;
        }
        
#endif
        
    }
}