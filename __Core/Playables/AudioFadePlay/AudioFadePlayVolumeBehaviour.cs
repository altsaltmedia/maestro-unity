using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class AudioFadePlayVolumeBehaviour : LerpToTargetBehaviour
    {
#if UNITY_EDITOR
        public bool debugCurrentVolume;
#endif
    }
}