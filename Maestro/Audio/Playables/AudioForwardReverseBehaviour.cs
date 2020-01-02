using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioForwardReverseBehaviour : LerpToTargetBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("clipEndTime")]
        private float _clipEndTime;

        public float clipEndTime
        {
            get => _clipEndTime;
            set => _clipEndTime = value;
        }
        
        [SerializeField]
        private BoolReference _isReversing;

        public BoolReference isReversing
        {
            get => _isReversing;
            set => _isReversing = value;
        }
        
        [SerializeField]
        private FloatReference _frameStepValue;

        public FloatReference frameStepValue
        {
            get => _frameStepValue;
            set => _frameStepValue = value;
        }
        
        [SerializeField]
        private FloatReference _swipeModifier;

        public FloatReference swipeModifier
        {
            get => _swipeModifier;
            set => _swipeModifier = value;
        }
        
        private bool _playingTriggered = false;

        public bool playingTriggered
        {
            get => _playingTriggered;
            set => _playingTriggered = value;
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}