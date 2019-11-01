using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Sequencing.Touch
{
    [ExecuteInEditMode]
    public class Touch_Controller : Input_Controller
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
        
        [SerializeField]
        private AxisModifier _axisModifier;

        public AxisModifier axisModifier
        {
            get => _axisModifier;
            set => _axisModifier = value;
        }
        
        [SerializeField]
        private SwipeModifier _swipeModifier;
        
        private SwipeModifier swipeModifier
        {
            get => _swipeModifier;
            set => _swipeModifier = value;
        }
        
        [SerializeField]
        private MomentumModifier _momentumModifier;

        private MomentumModifier momentumModifier
        {
            get => _momentumModifier;
            set => _momentumModifier = value;
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
        [ReadOnly]
        private int _swipeHistoryIndex;
        
        public int swipeHistoryIndex
        {
            get => _swipeHistoryIndex;
            set => _swipeHistoryIndex = value;
        }
        
        [ShowInInspector]
        [FoldoutGroup("Swipe Variables")]
        [ReadOnly]
        private float[] _swipeYHistory = new float[10];

        public float[] swipeYHistory
        {
            get => _swipeYHistory;
            set => _swipeYHistory = value;
        }

        [ShowInInspector]
        [FoldoutGroup("Swipe Variables")]
        [ReadOnly]
        private float[] _swipeXHistory = new float[10];
        
        public float[] swipeXHistory
        {
            get => _swipeXHistory;
            set => _swipeXHistory = value;
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
        [ValidateInput("IsPopulated")]
        private BoolReference _isReversing;

        public bool isReversing
        {
            get => _isReversing.Value;
            set => _isReversing.Variable.SetValue(value);
        }
        
        [SerializeField]
        [ReadOnly]
        private List<Touch_Data> _touchDataList = new List<Touch_Data>();

        public List<Touch_Data> touchDataList
        {
            get => _touchDataList;
        }

        public void UpdateSwipeHistory()
        {
            if (swipeHistoryIndex < swipeYHistory.Length) {
                swipeYHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.y);
                swipeXHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.x);
                swipeHistoryIndex++;
            }
        }
        
        public void ResetSwipeHistory()
        {
            swipeYHistory = new float[10];
            swipeXHistory = new float[10];
            swipeHistoryIndex = 0;
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

        public override void ConfigureData()
        {
            //if (Application.isPlaying == true) return;
            
            touchDataList.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceConfigs.Count; q++)
                {
                    var sequence = masterSequences[i].sequenceConfigs[q].sequence;
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;

                    IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                    var (item1, item2) = GetConfigTimes(markers);
                    
                    Input_Track inputConfigTrack =
                        Utils.GetTrackFromTimelineAsset(rootTimelineAsset, typeof(Input_Track)) as Input_Track;

                    touchDataList.Add(
                        CreateTouchData(sequence, item1, item2, inputConfigTrack, masterSequences[i]));
                }
            }
            
            EditorUtility.SetDirty(this);
            
            axisModifier.ConfigureData();
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

        private static List<Input_Extents> CreatePauseIntervals(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds)
        {
            List<Input_Extents> pauseIntervals = Input_Extents.CreateExtentsList(pauseStarts, pauseEnds);
            
            if(pauseStarts.Count != pauseEnds.Count) {
                if(pauseStarts.Count != pauseEnds.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }
            
            for(int i=0; i<pauseStarts.Count; i++)
            {
                Input_Extents interval;
                
                if(i <= pauseEnds.Count - 1) {
                    interval = new Input_Extents(pauseStarts[i], pauseEnds[i]);
                } else  {
                    interval = new Input_Extents(pauseStarts[i], 100000);
                }

                pauseIntervals.Add(interval);
            }

            return pauseIntervals;
        }

        private static Touch_Data CreateTouchData(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds,
            Input_Track inputConfigTrack, MasterSequence masterSequence)
        {
            List<Input_Extents> pauseIntervals = CreatePauseIntervals(targetSequence, pauseStarts, pauseEnds);
            
            return Touch_Data.CreateInstance(targetSequence, pauseIntervals, inputConfigTrack, masterSequence);
        }
#endif

        private static bool IsPopulated(V2Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}