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
    [RequireComponent(typeof(PlayableDirector))]
    [RequireComponent(typeof(ApplyModifyToSequence))]
    public partial class ConfigureSequence : MonoBehaviour
    {
        [Required]
        [SerializeField]
        Sequence sequence;

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
        private ApplyModifyToSequence _applyModifyToSequence;

        public ApplyModifyToSequence applyModifyToSequence
        {
            get => _applyModifyToSequence;
            private set => _applyModifyToSequence = value;
        }

        void Start()
        {
            GetPlayableDirector();
            sequence.syncer = gameObject.GetComponent<SyncTimelineToSequence>();
            applyModifyToSequence = gameObject.GetComponent<ApplyModifyToSequence>();
            applyModifyToSequence.sequence = sequence;
        }

        void GetPlayableDirector()
        {
            if (playableDirector == null) {
                playableDirector = gameObject.GetComponent<PlayableDirector>();
            }
        }
    }
}
