using System;
using UnityEngine;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioLerpVolumeBehaviour : LerpToTargetBehaviour
    {
        public float initialVolume;
        public float targetVolume;

#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
    }
}