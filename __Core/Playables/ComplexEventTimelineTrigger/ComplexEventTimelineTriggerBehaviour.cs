using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTimelineTriggerBehaviour : PlayableBehaviourTriggerBase
    {
        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public bool triggered = false;

        [HideInInspector]
        public BoolReference isReversing;

        public bool disableOnReverse;

        public List<ComplexEventTriggerPackager> complexEventTriggerPackagers;
    }
}