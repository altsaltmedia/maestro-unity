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
        public Vector2 swipeForce => appSettings.GetSwipeForce(this.gameObject, inputGroupKey);

        [SerializeField]
        [Required]
        private AxisMonitor _axisMonitor;

        public AxisMonitor axisMonitor => _axisMonitor;

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
        
        public float swipeModifierOutput
        {
            get => appSettings.GetSwipeModifierOutput(this.gameObject, inputGroupKey);
            set => appSettings.SetSwipeModifierOutput(this.gameObject, inputGroupKey, value);
        }
        
        public AxisReference ySwipeAxis => appSettings.GetYSwipeAxisReference(this.gameObject, inputGroupKey);

        public AxisReference xSwipeAxis => appSettings.GetXSwipeAxisReference(this.gameObject, inputGroupKey);
        
        public float momentumModifierOutput
        {
            get => appSettings.GetMomentumModifierOutput(this.gameObject, inputGroupKey);
            set => appSettings.SetMomentumModifierOutput(this.gameObject, inputGroupKey, value);
        }
        
        public AxisReference yMomentumAxis => appSettings.GetYMomentumAxisReference(this.gameObject, inputGroupKey);
        
        public AxisReference xMomentumAxis => appSettings.GetXMomentumAxisReference(this.gameObject, inputGroupKey);
        
        public bool isReversing
        {
            get => appSettings.GetIsReversing(this.gameObject, inputGroupKey);
            set => appSettings.SetIsReversing(this.gameObject, inputGroupKey, value);
        }
        
        public string swipeDirection => appSettings.GetSwipeDirection(this.gameObject, inputGroupKey);

        [SerializeField]
        private List<Touch_Data> _touchDataList = new List<Touch_Data>();

        public List<Touch_Data> touchDataList => _touchDataList;

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
                for (int q = 0; q < masterSequences[i].sequenceControllers.Count; q++)
                {
                    var sequence = masterSequences[i].sequenceControllers[q].sequence;
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

    }

}