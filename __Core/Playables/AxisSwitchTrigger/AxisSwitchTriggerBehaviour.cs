using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class AxisSwitchTriggerBehaviour : LerpToTargetBehaviour
    {
        [ReadOnly]
        public SwitchType switchType;

        [HideInInspector]
        public float midPoint;

        [ReadOnly]
        [ShowIf(nameof(switchType), SwitchType.AxisSwitch)]
        public AxisSwitch axisSwitch;

        [ReadOnly]
        [ShowIf(nameof(switchType), SwitchType.ForkSwitch)]
        public ForkSwitch forkSwitch;

        [ReadOnly]
        [ShowIf(nameof(switchType), SwitchType.InvertSwitch)]
        public InvertSwitch invertSwitch;
    }
}