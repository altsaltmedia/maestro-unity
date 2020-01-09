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
        public BoolReference _isReversing = new BoolReference();

        public bool isReversing
        {
            get => _isReversing.GetValue(this.directorObject);
            set => _isReversing.GetVariable(this.directorObject).SetValue(value);
        }
        
        [SerializeField]
        private FloatReference _frameStepValue = new FloatReference();

        public FloatReference frameStepValue
        {
            get => _frameStepValue;
            set => _frameStepValue = value;
        }
        
        [SerializeField]
        private FloatReference _swipeModifier = new FloatReference();

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