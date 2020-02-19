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
                if (_rootPlayable.Equals(default) && playableDirector.playableGraph.IsValid()) {
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

        [ShowInInspector]
        private bool isForwardAutoplaying;
        
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

//        private void OnDisable()
//        {
//            dynamicElementDisable.RaiseEvent(this.gameObject, this);
//        }
    
#endif

        // Since scriptable objects by default cannot serialize references to
        // game objects and Monobehaviours, set all of the sequence dependencies
        // here. This also allows any script to access related components directly
        // via the sequence scriptable object.
        public void Init(MasterSequence masterSequence)
        {
            this.masterSequence = masterSequence;
            sequence.sequenceController = this;
            sequence.SetDefaults();
            
            if (playableDirector == null) {
                playableDirector = gameObject.GetComponent<PlayableDirector>();
            };
            if (playableDirector.playableAsset == null) {
                playableDirector.playableAsset = sequence.sourcePlayable;
            }
            
            timelineInstanceConfig = gameObject.GetComponent<TimelineInstanceConfig>();
            timelineInstanceConfig.sequence = sequence;
            timelineInstanceConfig.timelineUpdated += OnTimelineUpdated;
            
            playableDirector.Play();
            SetSpeed(0);
        }

        public void ActivateForwardAutoplay(float targetSpeed)
        {
            if (audioMuted == true) {
                EnableAudioSources();
            }
            
            if (isForwardAutoplaying == false ||
                rootPlayable.GetPlayState() != PlayState.Playing || rootPlayable.GetSpeed() < 1) {
                playableDirector.Play();
                SetSpeed(targetSpeed);
                isForwardAutoplaying = true;
            }
        }
        
        public void DeactivateForwardAutoplay()
        {
            if (audioMuted == false) {
                MuteAudioSources();
            }
            
            playableDirector.Play();
            SetSpeed(0);
            isForwardAutoplaying = false;
        }
        
        public void OnTimelineUpdated(object sender, double currentTime)
        {
            if (isForwardAutoplaying == true) {
                sequence.currentTime = currentTime;
                OnSequenceUpdated();
            }
        }
        
        public void SetToBeginning(UnityEngine.Object caller)
        {
            SetSequenceTime(caller, 0);
        }

        public void SetToEnd(UnityEngine.Object caller)
        {
            SetSequenceTime(caller, (float)sequence.sourcePlayable.duration);
        }
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.4f, 1)]
        public void SetSpeed(float targetSpeed)
        {
            currentSpeed = targetSpeed; // This value is for debugging purposes
            if (rootPlayable.IsValid()) {
                rootPlayable.SetSpeed(targetSpeed);
            }
        }
        
        public Sequence ModifySequenceTime(float timeModifier)
        {
            DeactivateForwardAutoplay();
            
            sequence.currentTime += timeModifier;
            RootConfig rootConfig = sequence.sequenceController.masterSequence.rootConfig;

            if (sequence.currentTime < 0) {
                sequence.sequenceController.SetSequenceTime(this, 0);
                sequence.sequenceController.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                rootConfig.joiner.ActivatePreviousSequence(sequence);
                
            } else if (sequence.currentTime > sequence.sourcePlayable.duration) {
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
            if (targetTime > sequence.sourcePlayable.duration) {
                Debug.LogError($"Target time is greater than sequence {sequence.name} duration; unable to set", caller);
            }
            
            DeactivateForwardAutoplay();
            
            sequence.currentTime = targetTime;
            playableDirector.time = sequence.currentTime;
            playableDirector.Evaluate();
                
            OnSequenceUpdated();
            return sequence;
        }

        private void OnSequenceUpdated()
        {
            _sequenceUpdated.Invoke(this, sequence);
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
                            audioSource.volume = 1;
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
                            audioSource.volume = 0;
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
