using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Maestro.Sequencing.Touch
{
    [ExecuteInEditMode]
    public class Touch_Controller : Input_Controller
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Swipe Variables")]
        private V2Reference _swipeForce;

        public Vector2 swipeForce
        {
            get => _swipeForce.Value;
            set => _swipeForce.Variable.SetValue(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _ySensitivity;

        public float ySensitivity => _ySensitivity.Variable.value;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _xSensitivity;

        public float xSensitivity => _xSensitivity.Variable.value;

        [ValidateInput("IsPopulated")]
        private FloatReference _timeModifier;
        
        private float timeModifier
        {
            get => _timeModifier.Value;
            set => _timeModifier.Variable.SetValue(value);
        }
        
        [SerializeField]
        [Required]
        private AxisMonitor _axisMonitor;

        public AxisMonitor axisMonitor
        {
            get => _axisMonitor;
            set => _axisMonitor = value;
        }
        
        [SerializeField]
        [Required]
        private SwipeApplier _swipeApplier;
        
        private SwipeApplier swipeApplier
        {
            get => _swipeApplier;
            set => _swipeApplier = value;
        }
        
        [SerializeField]
        [Required]
        private MomentumApplier _momentumApplier;

        public MomentumApplier momentumApplier
        {
            get => _momentumApplier;
            set => _momentumApplier = value;
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
        
        [SerializeField]
        [FoldoutGroup("Momentum Variables")]
        [ValidateInput("IsPopulated")]
        private FloatReference _momentumModifierOutput;
        
        public float momentumModifierOutput
        {
            get => _momentumModifierOutput.Value;
            set => _momentumModifierOutput.Variable.SetValue(value);
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
        private BoolReference _isReversing;

        public bool isReversing
        {
            get => _isReversing.Value;
            set => _isReversing.Variable.SetValue(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private StringReference _swipeDirection;

        public string swipeDirection
        {
            get => _swipeDirection.Value;
            set => _swipeDirection.Variable.SetValue(value);
        }

        [SerializeField]
        private List<Touch_Data> _touchDataList = new List<Touch_Data>();

        public List<Touch_Data> touchDataList
        {
            get => _touchDataList;
        }

        public Vector2 GetDominantTouchForce(Vector2 vector2)
        {
            if (swipeDirection == nameof(SwipeDirection.xPositive) ||
                swipeDirection == nameof(SwipeDirection.xNegative)) {
                return new Vector2(vector2.x, 0);
            }
            
            return new Vector2(0, vector2.y);
        }
        
        public static Touch_Controller RefreshIsReversing(Touch_Controller touchController, SwipeDirection swipeDirection, Axis sourceAxis)
        {
            switch (swipeDirection) {
                
                case SwipeDirection.yPositive:
                case SwipeDirection.xPositive:
                {
                    if (sourceAxis.inverted == false) {
                        touchController.isReversing = false;
                    }
                    else {
                        touchController.isReversing = true;
                    }

                    break;
                }
                case SwipeDirection.yNegative:
                case SwipeDirection.xNegative:
                {
                    if (sourceAxis.inverted == false) {
                        touchController.isReversing = true;
                    }
                    else {
                        touchController.isReversing = false;
                    }

                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(swipeDirection), swipeDirection, null);
            }

            return touchController;
        }

//#if UNITY_EDITOR

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

                    var markerConfig = Tuple.Create(new List<double>(), new List<double>());

                    if (rootTimelineAsset.markerTrack != null) {
                        IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                        markerConfig = GetConfigTimes(markers);
                    }
                        
                    ConfigTrack inputConfigTrack =
                        Utils.GetTrackFromTimelineAsset(rootTimelineAsset, typeof(ConfigTrack)) as ConfigTrack;

                    touchDataList.Add(
                        CreateTouchData(sequence, markerConfig.Item1, markerConfig.Item2, inputConfigTrack, masterSequences[i]));
                }
            }
    
#if UNITY_EDITOR            
            EditorUtility.SetDirty(this);
#endif
            
            axisMonitor.ConfigureData();
        }

        private static Tuple<List<double>, List<double>> GetConfigTimes(IEnumerable<IMarker> markers)
        {
            List<double> pauseMomentumStarts = new List<double>();
            List<double> pauseMomentumEnds = new List<double>();

            foreach (IMarker marker in markers) {
                
                if (marker is MomentumMarker_PauseStart) {
                    pauseMomentumStarts.Add(marker.time);
                }

                if (marker is MomentumMarker_PauseEnd) {
                    pauseMomentumEnds.Add(marker.time);
                }
            }

            return new Tuple<List<double>, List<double>>(pauseMomentumStarts, pauseMomentumEnds);
        }

        private static List<Extents> CreatePauseIntervals(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds)
        {
            List<Extents> pauseIntervals = Extents.CreateExtentsList(pauseStarts, pauseEnds);
            
            if(pauseStarts.Count != pauseEnds.Count) {
                if(pauseStarts.Count != pauseEnds.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }
            
            for(int i=0; i<pauseStarts.Count; i++)
            {
                Extents interval;
                
                if(i <= pauseEnds.Count - 1) {
                    interval = new Extents(pauseStarts[i], pauseEnds[i]);
                } else  {
                    interval = new Extents(pauseStarts[i], 100000);
                }

                pauseIntervals.Add(interval);
            }

            return pauseIntervals;
        }

        private static Touch_Data CreateTouchData(Sequence targetSequence, List<double> pauseStarts, List<double> pauseEnds,
            ConfigTrack inputConfigTrack, MasterSequence masterSequence)
        {
            List<Extents> pauseIntervals = CreatePauseIntervals(targetSequence, pauseStarts, pauseEnds);
            
            return Touch_Data.CreateInstance(targetSequence, pauseIntervals, inputConfigTrack, masterSequence);
        }
//#endif

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
        
        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}