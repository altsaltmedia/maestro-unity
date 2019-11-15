using System;
using UnityEngine;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpVideoPlayerTimeBehaviour : LerpToTargetBehaviour
    {
        public float initialTime;
        public float targetTime;
        public float initialTimeAndroid;
        public float targetTimeAndroid;

#if UNITY_EDITOR
        public bool debugCurrentTime;
#endif
    }
}