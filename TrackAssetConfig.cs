using System;
using AltSalt.Maestro.Sequencing;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class TrackAssetConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        public AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        [SerializeField]
        [ReadOnly]
        [InfoBox("Sequence is populated dynamically when this component is used with a SequenceConfig")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        private double _currentTime;

        public double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        [SerializeField]
#if UNITY_EDITOR        
        [HideIf(nameof(SequencePopulated))]
#endif        
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        public InputGroupKey inputGroupKey
        {
            get
            {
                if (sequence != null) {
                    return sequence.sequenceConfig.masterSequence.rootConfig.inputGroupKey;
                }
                
                return _inputGroupKey.GetVariable() as InputGroupKey;
            }
        }
        
        public BoolVariable isReversingVariable =>
            appSettings.GetIsReversingReference(this.gameObject, inputGroupKey).GetVariable() as BoolVariable;

        public float timelineDebugTime
        {
            get => appSettings.timelineDebugTime;
            set => appSettings.timelineDebugTime = value;
        }

        public SimpleEventTrigger onEditorGraphStart => appSettings.onEditorGraphStart;

        public float frameStepValue => appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        public float swipeModifierOutput => appSettings.GetSwipeModifierOutput(this.gameObject, inputGroupKey);

        public bool logGlobalResponsiveElementActions => AppSettings.logGlobalResponsiveElementActions;
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));

            // If part of a sequence config, we'll get the group key from there.
            // Otherwise, we need to look for one.
            if (sequence == null) {
                PopulateGroupKey();
            }
        }

        private void PopulateGroupKey()
        {
            // First, check to see if we can populate the variable already using an already existing reference name
            _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));
            
            // If not, that means the reference name hasn't been populated,
            // so we'll set a default and attempt to repopulate again
            if (inputGroupKey == null) {
                InputGroupKey mainInputKey = Utils.GetCustomKey(nameof(appSettings.mainInput).Capitalize()) as InputGroupKey;
                if (mainInputKey != null) {
                    _inputGroupKey.referenceName = appSettings.mainInput.name; 
                    _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));
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