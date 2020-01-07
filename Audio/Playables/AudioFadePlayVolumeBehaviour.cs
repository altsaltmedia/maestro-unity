using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioFadePlayVolumeBehaviour : LerpToTargetBehaviour
    {
#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
        [ReadOnly]
        [FormerlySerializedAs("targetAudioSources")]
        [SerializeField]
        private List<AudioSource> _targetAudioSources = new List<AudioSource>();

        public List<AudioSource> targetAudioSources
        {
            get => _targetAudioSources;
            set => _targetAudioSources = value;
        }
    }

}