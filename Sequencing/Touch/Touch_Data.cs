using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing.Touch
{

    [Serializable]
    public class Touch_Data : Input_Data
    {
        [SerializeField]
        private MasterSequence _masterSequence;

        public MasterSequence masterSequence
        {
            get => _masterSequence;
            set => _masterSequence = value;
        }

        [SerializeField]
        [FoldoutGroup("Data")]
        private List<Extents> _pauseMomentumIntervals;

        public List<Extents> pauseMomentumIntervals
        {
            get => _pauseMomentumIntervals;
            set => _pauseMomentumIntervals = value;
        }

        
        [ShowInInspector]
        [FoldoutGroup("Data")]
        private bool _forceForward;

        public bool forceForward
        {
            get => _forceForward;
            set => _forceForward = value;
        }

        
        [ShowInInspector]
        [FoldoutGroup("Data")]
        private bool _forceBackward;

        public bool forceBackward
        {
            get => _forceBackward;
            set => _forceBackward = value;
        }

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Data")]
        private bool _pauseMomentumActive;

        public bool pauseMomentumActive
        {
            get => _pauseMomentumActive;
            set => _pauseMomentumActive = value;
        }

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Data")]
        private bool _momentumDisabled;

        public bool momentumDisabled
        {
            get => _momentumDisabled;
            set => _momentumDisabled = value;
        }

        [FormerlySerializedAs("_axisSwitchTrack"),ReadOnly]
        [SerializeField]
        [FoldoutGroup("Data")]
        private ConfigTrack _inputConfigTrack;

        public ConfigTrack inputConfigTrack
        {
            get => _inputConfigTrack;
            set => _inputConfigTrack = value;
        }

        protected override string dataTitle => sequence.name;

        public static Touch_Data CreateInstance(Sequence sequence, List<Extents> pauseMomentumIntervals,
            ConfigTrack inputConfigTrack, MasterSequence masterSequence)
        {
            var inputData = new Touch_Data
            {
                sequence = sequence,
                pauseMomentumIntervals = pauseMomentumIntervals,
                inputConfigTrack = inputConfigTrack,
                masterSequence = masterSequence
            };
            
            return inputData;
        }
    }
}