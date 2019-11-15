using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioFadePlayVolumeBehaviour : LerpToTargetBehaviour
    {
#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
        [ReadOnly]
        public List<AudioSource> targetAudioSources = new List<AudioSource>();
    }

}