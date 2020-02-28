using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class TimelineInstanceConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        public AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        [SerializeField]
        [ReadOnly]
        [InfoBox("Sequence is populated dynamically when this component is used with a SequenceConfig")]
        private bool _connectedToSequence;

        public bool connectedToSequence
        {
            get => _connectedToSequence;
            set => _connectedToSequence = value;
        }

        private double _currentTime;

        public double currentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                _timelineUpdated.Invoke(this, _currentTime);
            }
        }

        public delegate void TimelineUpdateHandler(object sender, double currentTime);
        
        private event TimelineUpdateHandler _timelineUpdated = (sender, currentTime) => { };
        
        public event TimelineUpdateHandler timelineUpdated
        {
            add
            {
                if (_timelineUpdated == null
                    || _timelineUpdated.GetInvocationList().Contains(value) == false) {
                    _timelineUpdated += value;
                }
            }
            remove => _timelineUpdated -= value;
        }

        public delegate void PauseSequenceHandler(object sender);
        
        private PauseSequenceHandler _pauseSequenceRequested = sender => { };
        
        public event PauseSequenceHandler pauseSequenceRequested
        {
            add
            {
                if (_pauseSequenceRequested == null
                    || _pauseSequenceRequested.GetInvocationList().Contains(value) == false) {
                    _pauseSequenceRequested += value;
                }
            }
            remove => _pauseSequenceRequested -= value;
        }
        
        public delegate void ResumeSequenceHandler(object sender, bool activateForwardAutoplay);
        
        private ResumeSequenceHandler _resumeSequenceRequested = (sender, activateForwardAutoplay) => { };
        
        public event ResumeSequenceHandler resumeSequenceRequested
        {
            add
            {
                if (_resumeSequenceRequested == null
                    || _resumeSequenceRequested.GetInvocationList().Contains(value) == false) {
                    _resumeSequenceRequested += value;
                }
            }
            remove => _resumeSequenceRequested -= value;
        }

        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        public InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;

        public BoolVariable isReversingVariable =>
            appSettings.GetIsReversingReference(this.gameObject, inputGroupKey).GetVariable() as BoolVariable;

        public float timelineDebugTime
        {
            get => appSettings.timelineDebugTime;
            set => appSettings.timelineDebugTime = value;
        }

        public SimpleEventTrigger onEditorGraphStart => appSettings.onEditorGraphStart;

        public bool bookmarkLoadingCompleted => appSettings.GetBookmarkLoadingCompleted(this, inputGroupKey);

        public float frameStepValue => appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        public float swipeModifierOutput => appSettings.GetSwipeModifierOutput(this.gameObject, inputGroupKey);

        public bool logGlobalResponsiveElementActions => AppSettings.logGlobalResponsiveElementActions;
        
#if UNITY_EDITOR
        private void Awake()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));

            // If part of a sequence config, we'll get the group key from there.
            // Otherwise, we need to look for one.
            if (connectedToSequence == false) {
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
#endif

        public PauseSequenceHandler GetPauseSequenceHandler(GameObject gameObject)
        {
            return _pauseSequenceRequested;
        }
        
        public ResumeSequenceHandler GetResumeSequenceHandler(GameObject gameObject)
        {
            return _resumeSequenceRequested;
        }
    }
}