using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class AudioForwardReverseBehaviour : LerpToTargetBehaviour
    {
        public float clipEndTime;

        [ValidateInput("IsPopulated")]
        public BoolReference isReversing;

        [ValidateInput("IsPopulated")]
        public FloatReference frameStepValue;

        [ValidateInput("IsPopulated")]
        public FloatReference swipeModifier;

        [HideInInspector]
        public bool playingTriggered = false;

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}