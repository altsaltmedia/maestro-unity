using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class TouchController
    {
        public partial class TouchData
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
            private List<StartEndThreshold> _pauseMomentumIntervals;

            public List<StartEndThreshold> pauseMomentumIntervals
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

            [ReadOnly]
            [SerializeField]
            [FoldoutGroup("Data")]
            private AxisSwitchTrack _axisSwitchTrack;

            public AxisSwitchTrack axisSwitchTrack
            {
                get => _axisSwitchTrack;
                set => _axisSwitchTrack = value;
            }

            public static TouchData CreateInstance(Sequence sequence, List<StartEndThreshold> pauseMomentumIntervals,
                AxisSwitchTrack axisSwitchTrack, MasterSequence masterSequence)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(TouchData)) as TouchData;
                var inputData = new TouchData();

                inputData.sequence = sequence;
                inputData.pauseMomentumIntervals = pauseMomentumIntervals;
                inputData.axisSwitchTrack = axisSwitchTrack;
                inputData.masterSequence = masterSequence;

                return inputData;
            }
        }
    }
}