using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Sequence_SyncTimeline))]
    [RequireComponent(typeof(Sequence_ProcessModify))]
    [RequireComponent(typeof(PlayableDirector))]
    [RequireComponent(typeof(TrackAssetConfig))]
    public class Sequence_Config : MonoBehaviour
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
            get => _playableDirector;
            private set => _playableDirector = value;
        }

        [Required]
        [SerializeField]
        private Sequence_ProcessModify _processModify;

        public Sequence_ProcessModify processModify
        {
            get => _processModify;
            private set => _processModify = value;
        }
        
        [Required]
        [SerializeField]
        private Sequence_SyncTimeline _syncTimeline;

        public Sequence_SyncTimeline syncTimeline
        {
            get => _syncTimeline;
            private set => _syncTimeline = value;
        }
        
        [Required]
        [SerializeField]
        private TrackAssetConfig _trackAssetConfig;

        public TrackAssetConfig trackAssetConfig
        {
            get => _trackAssetConfig;
            private set => _trackAssetConfig = value;
        }
        
        private void Start()
        {
            Init();
        }

        // Since scriptable objects by default cannot serialize references to
        // game objects and Monobehaviours, set all of the sequence dependencies
        // here. This also allows any script to access related components directly
        // via the sequence scriptable object.
        public void Init()
        {
            sequence.sequenceConfig = this;
            
            GetPlayableDirector();
            if (playableDirector.playableAsset == null) {
                playableDirector.playableAsset = sequence.sourcePlayable;
            }
            
            syncTimeline = gameObject.GetComponent<Sequence_SyncTimeline>();
            syncTimeline.sequence = sequence;

            processModify = gameObject.GetComponent<Sequence_ProcessModify>();
            processModify.sequence = sequence;
            
            trackAssetConfig = gameObject.GetComponent<TrackAssetConfig>();
            trackAssetConfig.sequence = sequence;
        }
        
        private void GetPlayableDirector()
        {
            if (playableDirector == null) {
                playableDirector = gameObject.GetComponent<PlayableDirector>();
            }
        }

        public Sequence_Config SetMasterSequence(MasterSequence masterSequence)
        {
            this.masterSequence = masterSequence;
            return this;
        }
    }
}
