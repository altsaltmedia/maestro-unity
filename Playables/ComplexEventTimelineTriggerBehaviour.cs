using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTimelineTriggerBehaviour : TimelineTriggerBehaviour
    {
        [FormerlySerializedAs("complexEventTriggerPackagers")]
        [SerializeField]
        private List<ComplexEventTriggerPackager> _complexEventTriggerPackagers;

        public List<ComplexEventTriggerPackager> complexEventTriggerPackagers
        {
            get => _complexEventTriggerPackagers;
            set => _complexEventTriggerPackagers = value;
        }
    }
}