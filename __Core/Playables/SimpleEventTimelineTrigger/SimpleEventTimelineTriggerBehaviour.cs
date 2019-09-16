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
        public BoolReference isReversing;

        public bool disableOnReverse;

        public List<SimpleEventTrigger> simpleEventTriggers;
    }
}