using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class LerpVideoPlayerTimeBehaviour : LerpToTargetBehaviour
    {
        public float initialTime;
        public float targetTime;

#if UNITY_EDITOR
        public bool debugCurrentTime;
#endif
    }
}