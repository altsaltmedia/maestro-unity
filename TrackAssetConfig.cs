using System;
using AltSalt.Maestro.Sequencing;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class TrackAssetConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            private set => _appSettings = value;
        }
        
        [SerializeField]
        [ReadOnly]
        [InfoBox("Sequence is populated dynamically when this component is used with a SequenceConfig")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [SerializeField]
        [HideIf(nameof(SequencePopulated))]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        private InputGroupKey inputGroupKey
        {
            get
            {
                if (sequence != null) {
                    return sequence.sequenceConfig.masterSequence.rootConfig.inputGroupKey;
                }
                
                return _inputGroupKey.GetVariable() as InputGroupKey;
            }
        }
        
        public bool isReversing
        {
            get => appSettings.GetIsReversing(this.gameObject, inputGroupKey);
            private set => appSettings.SetIsReversing(this.gameObject, inputGroupKey, value);
        }
        
        public bool scrubberActive => appSettings.GetScrubberActive(this.gameObject, inputGroupKey);
        
        public float timelineDebugTime
        {
            get => appSettings.timelineDebugTime;
            set => appSettings.timelineDebugTime = value;
        }

        public SimpleEventTrigger onEditorGraphStart => appSettings.onEditorGraphStart;

        public float frameStepValue => appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        public float swipeModifierOutput => appSettings.GetSwipeModifierOutput(this.gameObject, inputGroupKey);

        public bool logGlobalResponsiveElementActions => appSettings.logGlobalResponsiveElementActions;
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));

            if (inputGroupKey == null) {
                InputGroupKey mainInputKey = Utils.GetCustomKey(nameof(appSettings.mainInput).Capitalize()) as InputGroupKey;
                if (mainInputKey != null) {
                    _inputGroupKey.SetVariable(mainInputKey);
                }
            }
        }

        private bool SequencePopulated()
        {
            if (sequence != null) {
                return true;
            }

            return false;
        }

#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}