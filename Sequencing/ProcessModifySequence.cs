using System;
using System.Security.Policy;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Sequencing
{
    [RequireComponent(typeof(SequenceConfig))]
    [RequireComponent(typeof(SyncTimelineToSequence))]
    [RequireComponent(typeof(PlayableDirector))]
    public class ProcessModifySequence : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime by a SequenceConfig component.")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        private Sequence _previousSequence;

        private Sequence previousSequence
        {
            get =>  _previousSequence;
            set =>  _previousSequence= value;
        }

        [SerializeField]
        private Sequence _nextSequence;

        private Sequence nextSequence
        {
            get => _nextSequence;
            set => _nextSequence = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private SimpleEventTrigger _boundaryReached;

        public SimpleEventTrigger boundaryReached
        {
            get => _boundaryReached;
            set => _boundaryReached = value;
        }

        public void ModifySequence(float timeModifier)
        {
            sequence.currentTime += timeModifier;

            if (sequence.currentTime < 0) {
                sequence.currentTime = 0;
                sequence.sequenceConfig.syncTimelineToSequence.RefreshPlayableDirector();

                if (previousSequence != null)
                {
                    previousSequence.currentTime = previousSequence.sourcePlayable.duration;
                    previousSequence.active = true;
                    previousSequence.sequenceConfig.gameObject.SetActive(true);
                    previousSequence.sequenceConfig.syncTimelineToSequence.RefreshPlayableDirector();
                    
                    sequence.sequenceConfig.gameObject.SetActive(false);
                    sequence.active = false;
                }
                else
                {
                    boundaryReached.RaiseEvent(this.gameObject);
                }
                
            } else if (sequence.currentTime > sequence.sourcePlayable.duration) {
                sequence.currentTime = sequence.sourcePlayable.duration;
                sequence.sequenceConfig.syncTimelineToSequence.RefreshPlayableDirector();
                
                if (nextSequence != null)
                {
                    nextSequence.currentTime = 0;
                    nextSequence.active = true;
                    nextSequence.sequenceConfig.gameObject.SetActive(true);
                    nextSequence.sequenceConfig.syncTimelineToSequence.RefreshPlayableDirector();
                    
                    sequence.sequenceConfig.gameObject.SetActive(false);
                    sequence.active = false;
                }
                else
                {
                    boundaryReached.RaiseEvent(this.gameObject);
                }
            }
            else
            {
                sequence.sequenceConfig.syncTimelineToSequence.RefreshPlayableDirector();
            }
        }

        public static ProcessModifySequence SetPreviousSequence(ProcessModifySequence processModifySequence, Sequence targetSequence)
        {
            processModifySequence.previousSequence = targetSequence;
            return processModifySequence;
        }
        
        public static ProcessModifySequence SetNextSequence(ProcessModifySequence processModifySequence, Sequence targetSequence)
        {
            processModifySequence.nextSequence = targetSequence;
            return processModifySequence;
        }

//
//        private bool CanExecuteModify(EventPayload eventPayload, out float timeModifier)
//        {
//            float modifier = eventPayload.GetFloatValue();
//            
//            if (sequence.active == false || eventPayload.GetScriptableObjectValue() != sequence || float.IsNaN(modifier) == true)
//            {
//                timeModifier = float.NaN;
//                return false;
//            }
//            
//            timeModifier = modifier;
//            return true;
//        }
//
//        public void ModifySequenceWithAutoplay(EventPayload eventPayload)
//        {
//            if (sequence.autoplayActive == false || sequence.isLerping == true) return;
//
//            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;
//            
//            if (PlaybackModifiers.TimeWithinThreshold(sequence.currentTime,
//                    sequence.playbackModifiers.autoplayThresholds, out var currentInterval) == false)
//                return;
//
//            timeModifier *= CalculateAutoplayModifier(sequence, currentInterval, isReversing, autoplayEaseThreshold, easingUtility);
//
//            AutoplaySequence(sequence, timeModifier);
//            
//            if (sequence.currentTime > currentInterval.endTime)
//            {
//                sequence.autoplayActive = false;
//            }
//        }
//
//        public void ModifySequenceWithSwipe(EventPayload eventPayload)
//        {
//            if (Application.platform == RuntimePlatform.Android)
//            {
//                if (Sequence.AndroidVideoOverride(sequence) == true)
//                {
//                    return;
//                }
//            }
//
//            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;
//
//            Sequence.ModifySequence(sequence, timeModifier);
//        }
//
//        public void ModifySequenceWithMomentum(EventPayload eventPayload)
//        {
//            if(Application.platform == RuntimePlatform.Android)
//            {
//                if (sequence.momentumDisabled == true) return;
//            }
//            
//            if (sequence.pauseMomentumActive == true) return;
//            
//            if (CanExecuteModify(eventPayload, out var timeModifier) == false) return;
//
//            Sequence.ModifySequence(sequence, timeModifier);
//        }

        private static bool IsPopulated(SimpleEventTrigger attribute) {
            return Utils.IsPopulated(attribute);
        }
    }
}