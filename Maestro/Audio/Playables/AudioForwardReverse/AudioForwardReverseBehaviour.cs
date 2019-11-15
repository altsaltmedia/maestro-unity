using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioForwardReverseBehaviour : LerpToTargetBehaviour
    {
        public float clipEndTime;

        [HideInInspector]
        public BoolReference isReversing;

        [HideInInspector]
        public FloatReference frameStepValue;

        [HideInInspector]
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