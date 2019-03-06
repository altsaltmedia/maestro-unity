using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class LerpAudioVolumeBehaviour : LerpToTargetBehaviour
    {
        public float initialVolume;
        public float targetVolume;

#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
    }
}