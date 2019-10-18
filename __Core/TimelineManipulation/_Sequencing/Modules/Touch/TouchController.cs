using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    [ExecuteInEditMode]
    public partial class TouchController : InputController
    {
        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        private V2Reference _swipeForce;

        public Vector2 swipeForce
        {
            get => _swipeForce.Value;
            set => _swipeForce.Variable.SetValue(value);
        }
        
        [ValidateInput("IsPopulated")]
        private FloatReference _timeModifier;
        
        private float timeModifier
        {
            get => _timeModifier.Value;
            set => _timeModifier.Variable.SetValue(value);
        }
        
        private SwipeModifier _swipeModifier;
        
        private SwipeModifier swipeModifier
        {
            get => _swipeModifier;
            set => _swipeModifier = value;
        }
        
        private MomentumModifier _momentumModifier;

        private MomentumModifier momentumModifier
        {
            get => _momentumModifier;
            set => _momentumModifier = value;
        }

        private AxisModifier _axisModifier;

        public AxisModifier axisModifier
        {
            get => _axisModifier;
            set => _axisModifier = value;
        }
        
        [ValidateInput("IsPopulated")]
        [SerializeField]
        [FoldoutGroup("Swipe Variables")]
        private FloatReference _swipeModifierOutput;
        
        public float swipeModifierOutput
        {
            get => _swipeModifierOutput.Value;
            set => _swipeModifierOutput.Variable.SetValue(value);
        }

        [Required]
        [SerializeField]
        [FoldoutGroup("Swipe Variables")]
        private Axis _ySwipeAxis;

        public Axis ySwipeAxis
        {
            get => _ySwipeAxis;
        }
        
        [Required]
        [SerializeField]
        [FoldoutGroup("Swipe Variables")]
        private Axis _xSwipeAxis;

        public Axis xSwipeAxis
        {
            get => _xSwipeAxis;
        }
        
        [ShowInInspector]
        [FoldoutGroup("Swipe Variables")]
        private float[] _swipeYHistory = new float[10];

        public float[] swipeYHistory
        {
            get => _swipeYHistory;
            set => _swipeYHistory = value;
        }

        [ShowInInspector]
        [FoldoutGroup("Swipe Variables")]
        private float[] _swipeXHistory = new float[10];
        
        public float[] swipeXHistory
        {
            get => _swipeXHistory;
            set => _swipeXHistory = value;
        }

        [ShowInInspector]
        [FoldoutGroup("Swipe Variables")]
        private int _swipeHistoryIndex;
        
        public int swipeHistoryIndex
        {
            get => _swipeHistoryIndex;
            set => _swipeHistoryIndex = value;
        }
        
        [SerializeField]
        [FoldoutGroup("Momentum Variables")]
        [ValidateInput("IsPopulated")]
        private FloatReference _momentumModifierOutput;
        
        public float momentumModifierOutput
        {
            get => _momentumModifierOutput.Value;
            set => _momentumModifierOutput.Variable.SetValue(value);
        }
        
        [SerializeField]
        [FoldoutGroup("Momentum Variables")]
        [ValidateInput("IsPopulated")]
        private V2Reference _momentumForce;

        public Vector2 momentumForce
        {
            get => _momentumForce.Value;
            set => _momentumForce.Variable.SetValue(value);
        }
        
        [Required]
        [SerializeField]
        [FoldoutGroup("Momentum Variables")]
        private Axis _yMomentumAxis;
        
        public Axis yMomentumAxis
        {
            get => _yMomentumAxis;
        }

        // Momentum variables
        [Required]
        [SerializeField]
        [FoldoutGroup("Momentum Variables")]
        private Axis _xMomentumAxis;

        public Axis xMomentumAxis
        {
            get => _xMomentumAxis;
        }
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _forkActive;

        public bool forkActive
        {
            get => _forkActive.Value;
            set => _forkActive.Variable.SetValue(value);
        }
        
        private List<TouchData> _touchDataList = new List<TouchData>();

        public List<TouchData> touchDataList
        {
            get => _touchDataList;
        }


        private void Start()
        { 
            
        }

        public TouchController AddToSwipeHistory(Vector2 swipeForce)
        {
            if (swipeHistoryIndex < swipeYHistory.Length) {
                swipeYHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.y);
                swipeXHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.x);
                swipeHistoryIndex++;
            }

            return this;
        }
        
        public TouchController ResetSwipeHistory()
        {
            swipeYHistory = new float[10];
            swipeXHistory = new float[10];
            swipeHistoryIndex = 0;

            return this;
        }

        public Vector2 GetDominantSwipeForce(Vector2 sourceSwipeForce)
        {
            if(swipeYHistory.Length != swipeXHistory.Length) {
                Debug.LogError("History counts must of equal length");
                return Vector3.zero;
            }

            Vector2 modifiedSwipeForce;

            float totalYForce = 0;
            float totalXForce = 0;

            for (int z = 0; z < swipeYHistory.Length; z++) {
                totalYForce += swipeYHistory[z];
                totalXForce += swipeXHistory[z];
            }

            if (totalYForce > totalXForce) {
                modifiedSwipeForce = new Vector3(sourceSwipeForce.y, 0);
            } else {
                modifiedSwipeForce = new Vector3(0, sourceSwipeForce.x);
            }

            return modifiedSwipeForce;
        }

#if UNITY_EDITOR

        public void ConfigureData()
        {
            touchDataList.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequences.Count; q++)
                {
                    var sequence = masterSequences[i].sequences[q];
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;
                    
                    IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                    var (item1, item2) = GetConfigTimes(markers);
                    
                    AxisSwitchTrack axisSwitchTrack =
                        Utils.GetTrackFromTimelineAsset(rootTimelineAsset, typeof(AxisSwitchTrack)) as AxisSwitchTrack;

                    touchDataList.Add(
                        CreateTouchData(sequence, item1, item2, axisSwitchTrack, masterSequences[i]));
                }
            }
        }

        private static Tuple<List<double>, List<double>> GetConfigTimes(IEnumerable<IMarker> markers)
        {
            List<double> pauseMomentumStarts = new List<double>();
            List<double> pauseMomentumEnds = new List<double>();

            foreach (IMarker marker in markers) {
                
                if (marker is PauseMomentumStart) {
                    pauseMomentumStarts.Add(marker.time);
                }

                if (marker is PauseMomentumEnd) {
                    pauseMomentumEnds.Add(marker.time);
                }
            }

            return new Tuple<List<double>, List<double>>(pauseMomentumStarts, pauseMomentumEnds);
        }

        private static List<StartEndThreshold> CreatePauseIntervals(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds)
        {
            List<StartEndThreshold> pauseIntervals = StartEndThreshold.CreateStartEndThresholds(pauseStarts, pauseEnds);
            
            if(pauseStarts.Count != pauseEnds.Count) {
                if(pauseStarts.Count != pauseEnds.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }
            
            for(int i=0; i<pauseStarts.Count; i++)
            {
                StartEndThreshold interval;
                
                if(i <= pauseEnds.Count - 1) {
                    interval = new StartEndThreshold(pauseStarts[i], pauseEnds[i]);
                } else  {
                    interval = new StartEndThreshold(pauseStarts[i], 100000);
                }

                pauseIntervals.Add(interval);
            }

            return pauseIntervals;
        }

        private static TouchData CreateTouchData(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds,
            AxisSwitchTrack axisSwitchTrack, MasterSequence masterSequence)
        {
            List<StartEndThreshold> pauseIntervals = CreatePauseIntervals(targetSequence, pauseStarts, pauseEnds);
            
            return TouchData.CreateInstance(targetSequence, pauseIntervals, axisSwitchTrack, masterSequence);
        }
#endif
        
        [Serializable]
        public partial class TouchData : InputData
        {
        }
        
        private static bool IsPopulated(V2Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}