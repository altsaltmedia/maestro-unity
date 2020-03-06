using System;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    [ExecuteInEditMode]
    public class AudioLerpSnapshotBehaviour : LerpToTargetBehaviour
    {
        [ValueDropdown(nameof(crossfadeValues))]
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

        // NEEDS TO BE REMOVED - NO LONGER NEEDED
        [ReadOnly]
        [FormerlySerializedAs("fadeType")]
        [SerializeField]
        private FadeType _fadeType;

        public FadeType fadeType
        {
            get => _fadeType;
            set => _fadeType = value;
        }

        [FormerlySerializedAs("_fromSnapshot")]
        [FormerlySerializedAs("fromSnapshot")]
        [SerializeField]
        [TitleGroup("Snapshots")]
        [LabelText("$"+nameof(GetSnapshotALabel))]
        private AudioMixerSnapshot _snapshotA;

        public AudioMixerSnapshot snapshotA
        {
            get => _snapshotA;
            set => _snapshotA = value;
        }

        [FormerlySerializedAs("_targetSnapshot")]
        [FormerlySerializedAs("targetSnapshot")]
        [SerializeField]
        [TitleGroup("Snapshots")]
        [LabelText("$"+nameof(GetSnapshotBLabel))]
        private AudioMixerSnapshot _snapshotB;

        public AudioMixerSnapshot snapshotB
        {
            get => _snapshotB;
            set => _snapshotB = value;
        }
        
        [FormerlySerializedAs("_targetStartingWeight")]
        [FormerlySerializedAs("targetStartingWeight")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateInitialLabels))]
        [Range(0, 1)]
        [TitleGroup("Initial")]
        private float _initialBlend;
        
        public float initialBlend
        {
            get => _initialBlend;
            set => _initialBlend = value;
        }

        [ShowInInspector]
        [DisplayAsString]
        [TitleGroup("Initial")]
        private string _initialAWeight;

        private string initialAWeight
        {
            get => _initialAWeight;
            set => _initialAWeight = value;
        }

        [ProgressBar(0, 100, ColorMember = "GetSnapshotAColor", BackgroundColorMember = "GetSnapshotBColor", DrawValueLabel = false)]
        [ShowInInspector]
        [TitleGroup("Initial")]
        [LabelText(" ")]
        [ReadOnly]
        private float _initialAWeightBar;

        private float initialAWeightBar
        {
            get => _initialAWeightBar;
            set => _initialAWeightBar = value;
        }

        [ShowInInspector]
        [DisplayAsString]
        [TitleGroup("Initial")]
        private string _initialBWeight;

        private string initialBWeight
        {
            get => _initialBWeight;
            set => _initialBWeight = value;
        }
        
        [ProgressBar(0, 100, ColorMember = "GetSnapshotAColor", BackgroundColorMember = "GetSnapshotBColor", DrawValueLabel = false)]
        [ShowInInspector]
        [TitleGroup("Initial")]
        [LabelText(" ")]
        [ReadOnly]
        private float _initialBWeightBar;

        private float initialBWeightBar
        {
            get => _initialBWeightBar;
            set => _initialBWeightBar = value;
        }
        
        [FormerlySerializedAs("_targetEndingWeight")]
        [FormerlySerializedAs("targetEndingWeight")]
        [OnValueChanged(nameof(UpdateTargetLabels))]
        [SerializeField]
        [Range(0, 1)]
        [TitleGroup("Target")]
        private float _targetBlend;
        
        public float targetBlend
        {
            get => _targetBlend;
            set => _targetBlend = value;
        }

        [ShowInInspector]
        [DisplayAsString]
        [TitleGroup("Target")]
        private string _targetAWeight;

        private string targetAWeight
        {
            get => _targetAWeight;
            set => _targetAWeight = value;
        }
        
        [ProgressBar(0, 100, ColorMember = "GetSnapshotAColor", BackgroundColorMember = "GetSnapshotBColor", DrawValueLabel = false)]
        [ShowInInspector]
        [TitleGroup("Target")]
        [LabelText(" ")]
        [ReadOnly]
        private float _targetAWeightBar;

        private float targetAWeightBar
        {
            get => _targetAWeightBar;
            set => _targetAWeightBar = value;
        }

        [ShowInInspector]
        [DisplayAsString]
        [TitleGroup("Target")]
        private string _targetBWeight;

        private string targetBWeight
        {
            get => _targetBWeight;
            set => _targetBWeight = value;
        }
        
        [ProgressBar(0, 100, ColorMember = "GetSnapshotAColor", BackgroundColorMember = "GetSnapshotBColor", DrawValueLabel = false)]
        [ShowInInspector]
        [TitleGroup("Target")]
        [LabelText(" ")]
        [ReadOnly]
        private float _targetBWeightBar;

        private float targetBWeightBar
        {
            get => _targetBWeightBar;
            set => _targetBWeightBar = value;
        }
        
        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            UpdateInitialLabels();
            UpdateTargetLabels();
        }

        private string GetSnapshotALabel()
        {
            if (crossfade == true) {
                return "Off Snapshot";
            }

            return "Snapshot A";
        }
        
        private string GetSnapshotBLabel()
        {
            if (crossfade == true) {
                return "On Snapshot";
            }

            return "Snapshot B";
        }

        private Color GetSnapshotAColor()
        {
            return Color.green;
        }
        
        private Color GetSnapshotBColor()
        {
            return Color.magenta;
        }

        private void UpdateInitialLabels()
        {
            if (snapshotA == null || snapshotB == null) {
                initialAWeight = "Please populate both snapshots A and B";
                initialBWeight = "Please populate both snapshots A and B";
                return;
            }
            
            initialAWeightBar = Mathf.Abs(initialBlend - 1f) * 100f;
            initialAWeight = $"{snapshotA.name} : {initialAWeightBar:F1}%";
            
            initialBWeightBar = initialBlend * 100f;
            initialBWeight = $"{snapshotB.name} : {initialBWeightBar:F1}%";
        }
        
        private void UpdateTargetLabels()
        {
            if (snapshotA == null || snapshotB == null) {
                targetAWeight = "Please populate both snapshots A and B";
                targetBWeight = "Please populate both snapshots A and B";
                return;
            }
            
            targetAWeightBar = Mathf.Abs(targetBlend - 1f) * 100f;
            targetAWeight = $"{snapshotA.name} : {targetAWeightBar:F1}%";
            
            targetBWeightBar = targetBlend * 100f;
            targetBWeight = $"{snapshotB.name} : {targetBWeightBar:F1}%";
        }

        [Button(ButtonSizes.Large)]
        public void RefreshLabels()
        {
            UpdateInitialLabels();
            UpdateTargetLabels();
        }
    }
}
