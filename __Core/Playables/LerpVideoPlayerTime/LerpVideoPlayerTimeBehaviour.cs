using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class LerpVideoPlayerTimeBehaviour : LerpToTargetBehaviour
    {
        public float initialTime;
        public float targetTime;

#if UNITY_ANDROID
        public float initialTimeAndroid;
        public float targetTimeAndroid;
#endif

#if UNITY_EDITOR
        public bool debugCurrentTime;
#endif
    }
}