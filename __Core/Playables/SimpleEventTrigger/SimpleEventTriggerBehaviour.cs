using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class SimpleEventTriggerBehaviour : PlayableBehaviour
    {
        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public bool triggered = false;
    }
}