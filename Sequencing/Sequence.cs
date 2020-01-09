using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing
{
    [CreateAssetMenu(menuName = "AltSalt/Sequencing/Sequence")]
    public class Sequence : JoinerDestination
    {
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _active = false;
        
        public bool active {
            get => _active;
            set => _active = value;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private double _currentTime;

        public double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        [Required]
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private PlayableAsset _sourcePlayable;
        
        public PlayableAsset sourcePlayable {
            get => _sourcePlayable;
            set => _sourcePlayable = value;
        }

        [SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime via a Sequence Config component.")]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private Sequence_Config _sequenceConfig;
        
        public Sequence_Config sequenceConfig {
            get => _sequenceConfig;
            set => _sequenceConfig = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(defaultsTitle))]
        private bool _defaultStatus;

        protected bool defaultStatus
        {
            get => _defaultStatus;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(defaultsTitle))]
        private double _defaultTime;

        private double defaultTime
        {
            get => _defaultTime;
        }

        [TitleGroup("Autopopulated Fields")]
        [SerializeField]
        private bool _videoSequenceActive;
        
        public bool videoSequenceActive
        {
            get => _videoSequenceActive; 
            set => _videoSequenceActive = value;
        }

        private string propertiesTitle => "Properties";
        
        private string defaultsTitle => "Defaults";
        
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
            targetSequence.currentTime += timeModifier;
            return targetSequence;
//          
//            RefreshVideoStatus(targetSequence, targetSequence.playbackModifiers.autoplayThresholds);
//            RefreshPauseMomentum(targetSequence, targetSequence.playbackModifiers.pauseMomentumThresholds);
        }

//        public static Sequence RefreshPauseMomentum(Sequence targetSequence, List<StartEndThreshold> pauseMomentumThresholds)
//        {
//            if (PlaybackModifiers.TimeWithinThreshold(targetSequence.currentTime, pauseMomentumThresholds))
//            {
//                targetSequence.pauseMomentumActive = true;
//            }
//            else
//            {
//                targetSequence.pauseMomentumActive = false;
//            }
//
//            return targetSequence;
//        }
//        
//        public static Sequence RefreshVideoStatus(Sequence targetSequence, List<StartEndThreshold> autoplayThresholds)
//        {
//            if (PlaybackModifiers.TimeWithinVideoThreshold(targetSequence.currentTime, autoplayThresholds))
//            {
//                targetSequence.videoSequenceActive = true;
//            }
//            else
//            {
//                targetSequence.videoSequenceActive = false;
//            }
//
//            return targetSequence;
//        }
        
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
            get => _isReversing.GetValue(sequenceConfig.gameObject);
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
                    targetSequence.sequenceConfig.StartCoroutine(PauseSequence(targetSequence));
                    return true;
                }
            }

            return false;
        }
        
        public static IEnumerator PauseSequence(Sequence targetSequence)
        {
            targetSequence.active = false;
            targetSequence.momentumDisabled = true;
            targetSequence.triggerSpinnerShow.RaiseEvent(targetSequence.sequenceConfig.gameObject);
            yield return new WaitForSeconds(.5f);
            targetSequence.active = true;
            targetSequence.pauseSequenceComplete.RaiseEvent(targetSequence.sequenceConfig.gameObject);
            yield return new WaitForSeconds(.5f);
            targetSequence.triggerSpinnerHide.RaiseEvent(targetSequence.sequenceConfig.gameObject);
            yield return new WaitForSeconds(1f);
            targetSequence.momentumDisabled = false;
        }

    }
}