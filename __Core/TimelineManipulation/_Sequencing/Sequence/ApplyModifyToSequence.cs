using System;
using System.Security.Policy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Sequencing
{
    [RequireComponent(typeof(PlayableDirector))]
    [RequireComponent(typeof(ConfigureSequence))]
    public class ApplyModifyToSequence : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        [InfoBox("This value must be set at runtime by a SequenceConfig component.")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        private bool CanExecuteModify(EventPayload eventPayload, out float timeModifier)
        {
            float modifier = eventPayload.GetFloatValue();
            
            if (sequence.active == false || eventPayload.GetScriptableObjectValue() != sequence || float.IsNaN(modifier) == true)
            {
                timeModifier = float.NaN;
                return false;
            }
            
            timeModifier = modifier;
            return true;
        }

        public void ModifySequenceWithAutoplay(EventPayload eventPayload)
        {
            if (sequence.autoplayActive == false || sequence.isLerping == true) return;

            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;
            
            if (PlaybackModifiers.TimeWithinThreshold(sequence.currentTime,
                    sequence.playbackModifiers.autoplayThresholds, out var currentInterval) == false)
                return;

            timeModifier *= CalculateAutoplayModifier(sequence, currentInterval, isReversing, autoplayEaseThreshold, easingUtility);

            AutoplaySequence(sequence, timeModifier);
            
            if (sequence.currentTime > currentInterval.endTime)
            {
                sequence.autoplayActive = false;
            }
        }

        public void ModifySequenceWithSwipe(EventPayload eventPayload)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Sequence.AndroidVideoOverride(sequence) == true)
                {
                    return;
                }
            }

            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;

            Sequence.ModifySequence(sequence, timeModifier);
        }

        public void ModifySequenceWithMomentum(EventPayload eventPayload)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                if (sequence.momentumDisabled == true) return;
            }
            
            if (sequence.pauseMomentumActive == true) return;
            
            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;

            Sequence.ModifySequence(sequence, timeModifier);
        }    
    }
}