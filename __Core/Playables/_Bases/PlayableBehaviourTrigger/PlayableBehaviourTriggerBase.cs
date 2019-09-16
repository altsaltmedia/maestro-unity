using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    public abstract class PlayableBehaviourTriggerBase : PlayableBehaviour {

        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public bool triggered = false;

    }
}