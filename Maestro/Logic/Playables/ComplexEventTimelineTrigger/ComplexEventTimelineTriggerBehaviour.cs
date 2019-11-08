using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTimelineTriggerBehaviour : PlayableBehaviourTriggerBase
    {
        [HideInInspector]
        public BoolReference isReversing;

        public bool disableOnReverse;

        public List<ComplexEventTriggerPackager> complexEventTriggerPackagers;
    }
}