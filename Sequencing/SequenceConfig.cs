using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SyncTimelineToSequence))]
    [RequireComponent(typeof(ProcessModifySequence))]
    [RequireComponent(typeof(PlayableDirector))]
    public class SequenceConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            private set => _sequence = value;
        }

        [Required]
        [SerializeField]
        AppSettings appSettings;

        [Required]
        [SerializeField]
        SimpleEventTrigger boundaryReached;

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
        private ProcessModifySequence _processModifySequence;

        public ProcessModifySequence processModifySequence
        {
            get => _processModifySequence;
            private set => _processModifySequence = value;
        }
        
        [Required]
        [SerializeField]
        private SyncTimelineToSequence _syncTimelineToSequence;

        public SyncTimelineToSequence syncTimelineToSequence
        {
            get => _syncTimelineToSequence;
            private set => _syncTimelineToSequence = value;
        }

        void Start()
        {
            GetPlayableDirector();
            
            syncTimelineToSequence = gameObject.GetComponent<SyncTimelineToSequence>();
            syncTimelineToSequence.sequence = sequence;

            processModifySequence = gameObject.GetComponent<ProcessModifySequence>();
            processModifySequence.sequence = sequence;

            sequence.sequenceConfig = this;
        }

        void GetPlayableDirector()
        {
            if (playableDirector == null) {
                playableDirector = gameObject.GetComponent<PlayableDirector>();
            }
        }
    }
}
