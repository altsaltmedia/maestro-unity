using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class SimpleEventTimelineTriggerBehaviour : PlayableBehaviourTriggerBase
    {
        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public bool triggered = false;

        public List<SimpleEventTrigger> simpleEventTriggers;
    }
}