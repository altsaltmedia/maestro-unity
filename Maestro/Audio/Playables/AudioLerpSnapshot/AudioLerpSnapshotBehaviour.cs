using System;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    [ExecuteInEditMode]
    public class AudioLerpSnapshotBehaviour : LerpToTargetBehaviour
    {
        [ValueDropdown(nameof(crossfadeValues))]
        [PropertySpace(6, 6)]
        [FormerlySerializedAs("crossfade")]
        [SerializeField]
        private bool _crossfade;

        public bool crossfade
        {
            get => _crossfade;
            set => _crossfade = value;
        }

        private ValueDropdownList<bool> crossfadeValues = new ValueDropdownList<bool>() {
            { "No", false},
            { "Yes", true }
        };

        [ShowIf(nameof(crossfade))]
        [PropertySpace(0, 6)]
        [FormerlySerializedAs("fadeType")]
        [SerializeField]
        private FadeType _fadeType;

        public FadeType fadeType
        {
            get => _fadeType;
            set => _fadeType = value;
        }

        [BoxGroup("Starting Snapshot")]
        [PropertySpace(0, 6)]
        [FormerlySerializedAs("fromSnapshot")]
        [SerializeField]
        private AudioMixerSnapshot _fromSnapshot;

        public AudioMixerSnapshot fromSnapshot
        {
            get => _fromSnapshot;
            set => _fromSnapshot = value;
        }

        [BoxGroup("Target Snapshot")]
        [FormerlySerializedAs("targetSnapshot")]
        [SerializeField]
        private AudioMixerSnapshot _targetSnapshot;

        public AudioMixerSnapshot targetSnapshot
        {
            get => _targetSnapshot;
            set => _targetSnapshot = value;
        }

        [BoxGroup("Target Snapshot")]
        [FormerlySerializedAs("targetStartingWeight")]
        [SerializeField]
        private float _targetStartingWeight;

        public float targetStartingWeight
        {
            get => _targetStartingWeight;
            set => _targetStartingWeight = value;
        }

        [BoxGroup("Target Snapshot")]
        [FormerlySerializedAs("targetEndingWeight")]
        [SerializeField]
        private float _targetEndingWeight;

        public float targetEndingWeight
        {
            get => _targetEndingWeight;
            set => _targetEndingWeight = value;
        }
    }
}
