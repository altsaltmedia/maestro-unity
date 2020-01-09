using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTimelineTriggerBehaviour : TimelineTriggerBehaviour
    {
        [FormerlySerializedAs("simpleEventTriggers")]
        [SerializeField]
        private List<SimpleEventTrigger> _simpleEventTriggers;

        public List<SimpleEventTrigger> simpleEventTriggers
        {
            get => _simpleEventTriggers;
            set => _simpleEventTriggers = value;
        }
    }
}