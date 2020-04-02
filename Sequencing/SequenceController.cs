using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PlayableDirector))]
    [RequireComponent(typeof(TimelineInstanceConfig))]
    public class SequenceController : MonoBehaviour, IDynamicLayoutElement
    {
        private bool isReversing =>
            masterSequence.rootConfig.appSettings.GetIsReversing(this, masterSequence.rootConfig.inputGroupKey);
        
        [Required]
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [ReadOnly]
        [SerializeField]
        [InfoBox("This value must be set by a Master Sequence component")]
        private MasterSequence _masterSequence;

        public MasterSequence masterSequence
        {
            get => _masterSequence;
            private set => _masterSequence = value;
        }
        
        [Required]
        [SerializeField]
        private PlayableDirector _playableDirector;

        public PlayableDirector playableDirector
        {
            get
            {
                if (_playableDirector == null) {
                    _playableDirector = GetComponent<PlayableDirector>();
                }

                return _playableDirector;
            }
            private set => _playableDirector = value;
        }

        [FormerlySerializedAs("_trackAssetConfig"),Required]
        [SerializeField]
        private TimelineInstanceConfig _timelineInstanceConfig;

        public TimelineInstanceConfig timelineInstanceConfig
        {
            get => _timelineInstanceConfig;
            private set => _timelineInstanceConfig = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public string elementName => gameObject.name;
        
        public Scene parentScene => gameObject.scene;

        [SerializeField]
        private int _priority = -10;
        
        public int priority => _priority;
        
        [SerializeField]
        private bool _logElementOnLayoutUpdate = false;

        public bool logElementOnLayoutUpdate {
            get
            {
                if (_logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                    return true;
                }

                return false;
            }
        }

        private Playable _rootPlayable;
        
        private Playable rootPlayable
        {
            get
            {
                if (playableDirector.playableGraph.IsValid()) {
                    _rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
                }

                return _rootPlayable;
            }
            set => _rootPlayable = value;
        }

        [ShowInInspector]
        private bool audioMuted;

        [ShowInInspector]
        [DisplayAsString]
        private float currentSpeed;
        
        private enum SequenceUpdateState { ForwardAutoplay, ManualUpdate }

        [ShowInInspector]
        private SequenceUpdateState _sequenceUpdateState = SequenceUpdateState.ManualUpdate;

        private SequenceUpdateState sequenceUpdateState
        {
            get => _sequenceUpdateState;
            set => _sequenceUpdateState = value;
        }

        public delegate void SequenceUpdatedHandler(object sender, Sequence updatedSequence);

        private event SequenceUpdatedHandler _sequenceUpdated = (sender, updatedSequence) => { };
        
        public event SequenceUpdatedHandler sequenceUpdated
        {
            add
            {
                if (_sequenceUpdated == null
                    || _sequenceUpdated.GetInvocationList().Contains(value) == false) {
                    _sequenceUpdated += value;
                }
            }
            remove => _sequenceUpdated -= value;
        }

#if UNITY_EDITOR
        private void Awake()
        {
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
            
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }
#endif
        
        /// <summary>
        /// Since scriptable objects by default cannot serialize references to
        /// game objects and Monobehaviours, we set the sequence controller
        /// here. This also allows any script to these control functions directly
        /// via the sequence scriptable object. 
        /// </summary>
        /// <param name="masterSequence"></param>
        public void Init(MasterSequence masterSequence)
        {
            this.masterSequence = masterSequence;
            sequence.sequenceController = this;
            sequence.SetToDefaults();
            
            if (playableDirector == null) {
                playableDirector = gameObject.GetComponent<PlayableDirector>();
            };
            if (playableDirector.playableAsset == null) {
                playableDirector.playableAsset = sequence.sourcePlayable;
            }
            
            timelineInstanceConfig = gameObject.GetComponent<TimelineInstanceConfig>();
            timelineInstanceConfig.connectedToSequence = true;
            timelineInstanceConfig.timelineUpdated += OnTimelineUpdated;
            timelineInstanceConfig.pauseSequenceRequested += OnPauseSequenceRequested;
            timelineInstanceConfig.resumeSequenceRequested += OnResumeSequenceRequested;
        }

        private void OnDisable()
        {
            timelineInstanceConfig.timelineUpdated -= OnTimelineUpdated;
            timelineInstanceConfig.pauseSequenceRequested -= OnPauseSequenceRequested;
            timelineInstanceConfig.resumeSequenceRequested -= OnResumeSequenceRequested;
//            dynamicElementDisable.RaiseEvent(this.gameObject, this);
        }

        /// <summary>
        /// If the timeline is autoplaying, then we need to refresh our sequence
        /// state on every frame to sync with our custom configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentTime"></param>
        public void OnTimelineUpdated(object sender, double currentTime)
        {
            if (Application.isPlaying == false) return;
            
            if (sequenceUpdateState == SequenceUpdateState.ForwardAutoplay) {
                sequence.currentTime = currentTime;
                sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
                OnSequenceUpdated();
            }
        }

        public void OnPauseSequenceRequested(object sender)
        {
            sequence.paused = true;
            SetSpeed(0);
        }
        
        public void OnResumeSequenceRequested(object sender, bool activateForwardAutoplay)
        {
            sequence.paused = false;
            if (activateForwardAutoplay == true) {
                ActivateForwardAutoplayState(1);
            }
        }

        private void OnSequenceUpdated()
        {
            if (Application.isPlaying == false) return;
            
            _sequenceUpdated.Invoke(this, sequence);
        }

        public void ActivateForwardAutoplayState(float targetSpeed)
        {
            if (audioMuted == true) {
                EnableAudioSources();
            }
            
            if (sequenceUpdateState == SequenceUpdateState.ManualUpdate ||
                rootPlayable.GetPlayState() != PlayState.Playing || rootPlayable.GetSpeed() < 1) {
                playableDirector.Play();
                SetSpeed(targetSpeed);
                sequenceUpdateState = SequenceUpdateState.ForwardAutoplay;
            }
        }
        
        public void ActivateManualUpdateState()
        {
            if (audioMuted == false) {
                MuteAudioSources();
            }
            
            // We must be playing in manual update state in order
            // for timeline to evaluate animation tracks
            playableDirector.Play();
            SetSpeed(0);
            sequenceUpdateState = SequenceUpdateState.ManualUpdate;
        }
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.4f, 1)]
        public void SetSpeed(float targetSpeed)
        {
            currentSpeed = targetSpeed; // This value is for debugging purposes
            if (rootPlayable.IsValid() == true) {
                rootPlayable.SetSpeed(targetSpeed);
            }
        }
        
        public Sequence ModifySequenceTime(float timeModifier)
        {
            if (sequence.paused == true) return sequence;
            
            sequence.currentTime += timeModifier;
            RootConfig rootConfig = sequence.sequenceController.masterSequence.rootConfig;
            
            ActivateManualUpdateState();

            if (sequence.currentTime < 0) {
                sequence.sequenceController.SetSequenceTime(this, 0);
                sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                rootConfig.joiner.ActivatePreviousSequence(sequence);
                
            } else if (sequence.currentTime > sequence.duration) {
                sequence.sequenceController.SetSequenceTime(this, (float)sequence.sourcePlayable.duration);
                sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                rootConfig.joiner.ActivateNextSequence(sequence);
                
            } else {
                playableDirector.time = sequence.currentTime;
                playableDirector.Evaluate();
                sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
            }
            
            OnSequenceUpdated();
            return sequence;
        }

        public Sequence SetSequenceTime(UnityEngine.Object caller, float targetTime)
        {
            if (sequence.paused == true) return sequence;
            
            ActivateManualUpdateState();
            
            sequence.currentTime = targetTime;
            playableDirector.time = sequence.currentTime;
            playableDirector.Evaluate();
            sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
             
            OnSequenceUpdated();
            return sequence;
        }

        public void EnableAudioSources()
        {
            audioMuted = false;
            
            var timelineAsset = playableDirector.playableAsset as TimelineAsset;
            
            foreach (var track in timelineAsset.GetOutputTracks()) {
                
                if (track is AudioTrack audioTrack) {
            
                    foreach (var playableBinding in audioTrack.outputs) {
                        
                        Object objectBinding = playableDirector.GetGenericBinding(playableBinding.sourceObject);
            
                        if (objectBinding is AudioSource audioSource) {
                            audioSource.mute = false;
                        }
                        
                    }
                }
            }
        }
        
        public void MuteAudioSources()
        {
            audioMuted = true;
            
            var timelineAsset = playableDirector.playableAsset as TimelineAsset;
            
            foreach (var track in timelineAsset.GetOutputTracks()) {
                
                if (track is AudioTrack audioTrack) {
            
                    foreach (var playableBinding in audioTrack.outputs) {
                        
                        Object objectBinding = playableDirector.GetGenericBinding(playableBinding.sourceObject);
            
                        if (objectBinding is AudioSource audioSource) {
                            audioSource.mute = true;
                        }
                    }
                }
            }
        }

        public void CallExecuteLayoutUpdate(Object callingObject)
        {
#if UNITY_EDITOR
            if (sequence == null || sequence.sequenceController == null ||
                TimelineEditor.inspectedDirector != sequence.sequenceController.playableDirector) {
                return;
            }
            
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }
            
            sequence.sequenceController.playableDirector.Evaluate();
#endif
        }

        [Button(ButtonSizes.Large)]
        public void CallSetSequenceTime(float targetTime)
        {
            SetSequenceTime(this, targetTime);
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
