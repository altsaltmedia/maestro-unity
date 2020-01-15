using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Sequencing
{
    [RequireComponent(typeof(Sequence_Config))]
    [RequireComponent(typeof(Sequence_SyncTimeline))]
    [RequireComponent(typeof(PlayableDirector))]
    public class Sequence_ProcessModify : MonoBehaviour
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

        public void ModifySequence(float timeModifier)
        {
            sequence.currentTime += timeModifier;
            
//            if (sequence.invert == false) {
//                sequence.currentTime += timeModifier;
//            }
//            else {
//                sequence.currentTime += timeModifier * -1f;
//            }

            RootConfig rootConfig = sequence.sequenceConfig.masterSequence.rootConfig;
            
            if (sequence.currentTime < 0) {
                sequence.currentTime = 0;
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                rootConfig.sequenceModified.RaiseEvent(this.gameObject, sequence);
                rootConfig.joiner.ActivatePreviousSequence(sequence);
                
            } else if (sequence.currentTime > sequence.sourcePlayable.duration) {
                sequence.currentTime = sequence.sourcePlayable.duration;
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                rootConfig.sequenceModified.RaiseEvent(this.gameObject, sequence);
                rootConfig.joiner.ActivateNextSequence(sequence);
                
            } else  {
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                rootConfig.sequenceModified.RaiseEvent(this.gameObject, sequence);
            }
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
        
        private static bool IsPopulated(ComplexEventManualTrigger attribute) {
            return Utils.IsPopulated(attribute);
        }
    }
}