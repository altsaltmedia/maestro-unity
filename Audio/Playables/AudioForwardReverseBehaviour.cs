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

        public float clipEndTime => _clipEndTime;

        public bool isReversing => trackAssetConfig.isReversing;

        public float frameStepValue => trackAssetConfig.frameStepValue;
        
        public float swipeModifierOutput => trackAssetConfig.swipeModifierOutput;

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