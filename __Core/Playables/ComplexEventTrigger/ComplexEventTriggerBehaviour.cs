using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTriggerBehaviour : PlayableBehaviour
    {
        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        public string stringValue;
    }
}