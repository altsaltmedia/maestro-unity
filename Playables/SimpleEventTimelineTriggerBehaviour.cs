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

        [SerializeField]
        private bool _forceActivateOnForward = true;

        public override bool forceActivateOnForward => _forceActivateOnForward;
        
        [SerializeField]
        private bool _forceActivateOnReverse = true;
        
        public override bool forceActivateOnReverse => _forceActivateOnReverse;

    }
}