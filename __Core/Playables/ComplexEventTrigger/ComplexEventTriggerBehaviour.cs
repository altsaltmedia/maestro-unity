using System;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTriggerBehaviour : PlayableBehaviour
    {
        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public bool triggered = false;

        public ComplexEventPackager complexEventPackager;
    }
}