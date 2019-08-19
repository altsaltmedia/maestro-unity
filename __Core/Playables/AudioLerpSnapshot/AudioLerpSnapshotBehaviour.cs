using System;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class AudioLerpSnapshotBehaviour : LerpToTargetBehaviour
    {
        [ValueDropdown(nameof(crossfadeValues))]
        [PropertySpace(6, 6)]
        public bool crossfade;

        ValueDropdownList<bool> crossfadeValues = new ValueDropdownList<bool>() {
            { "No", false},
            { "Yes", true }
        };

        [ShowIf(nameof(crossfade))]
        [PropertySpace(0, 6)]
        public FadeType fadeType;

        [BoxGroup("Starting Snapshot")]
        [PropertySpace(0, 6)]
        public AudioMixerSnapshot fromSnapshot;

        [BoxGroup("Target Snapshot")]
        public AudioMixerSnapshot targetSnapshot;

        [BoxGroup("Target Snapshot")]
        public float targetStartingWeight;

        [BoxGroup("Target Snapshot")]
        public float targetEndingWeight;

    }
}
