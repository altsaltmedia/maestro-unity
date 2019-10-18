using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing
{
    [CreateAssetMenu(menuName = "AltSalt/Sequence")]
    public class Sequence : ScriptableObject
    {
        private bool _active = true;
        
        public bool active {
            get => _active;
            set => _active = value;
        }

        [Required]
        [SerializeField]
        private PlayableAsset _sourcePlayable;
        
        public PlayableAsset sourcePlayable {
            get => _sourcePlayable;
            set => _sourcePlayable = value;
        }

        [SerializeField]
        private SyncTimelineToSequence _syncer;
        
        public SyncTimelineToSequence syncer {
            get => _syncer;
            set => _syncer = value;
        }

        [SerializeField]
        private bool _defaultStatus;

        protected bool defaultStatus
        {
            get => _defaultStatus;
        }

        [SerializeField]
        private double _currentTime;

        public double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        [SerializeField]
        private double _defaultTime;

        private double defaultTime
        {
            get => _defaultTime;
        }

        [SerializeField]
        private bool _invert = false;
        
        public bool invert
        {
            get => _invert;
        }

        [TitleGroup("Autopopulated Fields")]
        [SerializeField]
        private bool _videoSequenceActive;
        
        public bool videoSequenceActive
        {
            get => _videoSequenceActive; 
            set => _videoSequenceActive = value;
        }

        [TitleGroup("Playback Modifiers")]
        [SerializeField]
        private PlaybackModifiers _playbackModifiers;

        public PlaybackModifiers playbackModifiers
        {
            get => _playbackModifiers;
        }
        
        [PropertySpace(10)]
        
        private void Start()
        {
            currentTime = 0f;
        }

        public void SetStatus(bool targetStatus)
        {
            active = targetStatus;
        }

        public void SetDefaults()
        {
            active = defaultStatus;
            currentTime = defaultTime;
        }

        public static Sequence ModifySequence(Sequence targetSequence, float timeModifier)
        {
            if (targetSequence.invert == true) timeModifier *= -1f;

            if (targetSequence.forceForward == true)
            {
                timeModifier = Mathf.Abs(timeModifier);
            }
            else if (targetSequence.forceBackward == true)
            {
                timeModifier = Mathf.Abs(timeModifier) * -1f;
            }

            targetSequence.currentTime += timeModifier;
                
            RefreshVideoStatus(targetSequence, targetSequence.playbackModifiers.autoplayThresholds);
            RefreshPauseMomentum(targetSequence, targetSequence.playbackModifiers.pauseMomentumThresholds);
        }

        public static Sequence RefreshPauseMomentum(Sequence targetSequence, List<StartEndThreshold> pauseMomentumThresholds)
        {
            if (PlaybackModifiers.TimeWithinThreshold(targetSequence.currentTime, pauseMomentumThresholds))
            {
                targetSequence.pauseMomentumActive = true;
            }
            else
            {
                targetSequence.pauseMomentumActive = false;
            }

            return targetSequence;
        }
        
        public static Sequence RefreshVideoStatus(Sequence targetSequence, List<StartEndThreshold> autoplayThresholds)
        {
            if (PlaybackModifiers.TimeWithinVideoThreshold(targetSequence.currentTime, autoplayThresholds))
            {
                targetSequence.videoSequenceActive = true;
            }
            else
            {
                targetSequence.videoSequenceActive = false;
            }

            return targetSequence;
        }
        
        // ==================== //
        // ANDROID DEPENDENCIES //
        // ==================== //
        // On Android devices, we need to briefly pause the sequence to allow the device
        // to catch up when in a video sequence and abruptly changing directions.
        
        [ReadOnly]
        [TitleGroup("Android Dependencies")]
        private bool _momentumDisabled;

        public bool momentumDisabled
        {
            get => _momentumDisabled;
            set => _momentumDisabled = value;
        }
        
        [ValidateInput("IsPopulated")]
        [BoxGroup("Android Dependencies")]
        private BoolReference _isReversing;

        [SerializeField]

        public bool isReversing
        {
            get => _isReversing.Value;
        }

        [Required]
        [BoxGroup("Android Dependencies")]
        private SimpleEventTrigger _triggerSpinnerShow;

        private SimpleEventTrigger triggerSpinnerShow
        {
            get => _triggerSpinnerShow;
        }
        
        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        private SimpleEventTrigger _triggerSpinnerHide;

        private SimpleEventTrigger triggerSpinnerHide
        {
            get => _triggerSpinnerHide;
        }

        [SerializeField]
        [Required]
        [BoxGroup("Android dependencies")]
        private SimpleEventTrigger _pauseSequenceComplete;

        private SimpleEventTrigger pauseSequenceComplete
        {
            get => _pauseSequenceComplete;
        }

        private bool _internalIsReversingVal;

        public bool internalIsReversingVal
        {
            get => _internalIsReversingVal;
            set => _internalIsReversingVal = value;
        }
        
        public static bool AndroidVideoOverride(Sequence targetSequence)
        {
            if (targetSequence.videoSequenceActive == true) {
                
                if (targetSequence.internalIsReversingVal != targetSequence.isReversing) {
                    targetSequence.internalIsReversingVal = targetSequence.isReversing;
                    targetSequence.syncer.StartCoroutine(PauseSequence(targetSequence));
                    return true;
                }
            }

            return false;
        }
        
        public static IEnumerator PauseSequence(Sequence targetSequence)
        {
            targetSequence.active = false;
            targetSequence.momentumDisabled = true;
            targetSequence.triggerSpinnerShow.RaiseEvent(targetSequence.syncer.gameObject);
            yield return new WaitForSeconds(.5f);
            targetSequence.active = true;
            targetSequence.pauseSequenceComplete.RaiseEvent(targetSequence.syncer.gameObject);
            yield return new WaitForSeconds(.5f);
            targetSequence.triggerSpinnerHide.RaiseEvent(targetSequence.syncer.gameObject);
            yield return new WaitForSeconds(1f);
            targetSequence.momentumDisabled = false;
        }

    }
}