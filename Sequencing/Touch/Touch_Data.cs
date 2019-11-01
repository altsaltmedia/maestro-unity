using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Sequencing.Touch
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
        private List<Input_Extents> _pauseMomentumIntervals;

        public List<Input_Extents> pauseMomentumIntervals
        {
            get => _pauseMomentumIntervals;
            set => _pauseMomentumIntervals = value;
        }

        [ReadOnly]
        [ShowInInspector]
        [FoldoutGroup("Data")]
        private bool _forceForward;

        public bool forceForward
        {
            get => _forceForward;
            set => _forceForward = value;
        }

        [ReadOnly]
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
        private Input_Track _inputConfigTrack;

        public Input_Track inputConfigTrack
        {
            get => _inputConfigTrack;
            set => _inputConfigTrack = value;
        }

        public static Touch_Data CreateInstance(Sequence sequence, List<Input_Extents> pauseMomentumIntervals,
            Input_Track inputConfigTrack, MasterSequence masterSequence)
        {
            //var inputData = ScriptableObject.CreateInstance(typeof(TouchData)) as TouchData;
            var inputData = new Touch_Data();

            inputData.sequence = sequence;
            inputData.pauseMomentumIntervals = pauseMomentumIntervals;
            inputData.inputConfigTrack = inputConfigTrack;
            inputData.masterSequence = masterSequence;

            return inputData;
        }
    }
}