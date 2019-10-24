using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using System.Linq;
using AltSalt.Sequencing.Navigate;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Sequencing.Autorun
{
    [ExecuteInEditMode]
    [Serializable]
    public class AutorunController : InputController
    {
        private Autoplayer _autoplayer;

        private Autoplayer autoplayer
        {
            get => _autoplayer;
        }
        
        private Lerper _lerper;

        private Lerper lerper
        {
            get => _lerper;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private BoolReference _isReversing;

        public bool isReversing
        {
            get => _isReversing.Value;
        }

        [SerializeField]
        [InfoBox("Autorun data is populated dynamically from connected Master Sequences")]
        private List<AutorunData> _autorunData = new List<AutorunData>();

        public List<AutorunData> autorunData
        {
            get => _autorunData;
        }

#if UNITY_EDITOR

        public void ConfigureData()
        {
            if (Application.isPlaying == true) return;
            
            autorunData.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceConfigs.Count; q++)
                {
                    var sequence = masterSequences[i].sequenceConfigs[q].sequence;
                    
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;

                    IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                    var (item1, item2, item3, item4) = GetConfigTimes(markers);

                    autorunData.Add(
                        CreateAutorunData(sequence, item1, item2, item3, item4));
                }
            }
            EditorUtility.SetDirty(this);
        }

        private static Tuple<List<double>, List<double>, List<int>, List<string>> GetConfigTimes(IEnumerable<IMarker> markers)
        {
            List<double> autoplayStarts = new List<double>();
            List<double> autoplayEnds = new List<double>();
            List<int> videoIntervalIds = new List<int>();
            List<string> descriptions = new List<string>();

            int videoId = 0;

            foreach (IMarker marker in markers) {
                
                if (marker is AutoplayStart) {
                    autoplayStarts.Add(marker.time);
                }

                else if (marker is AutoplayEnd) {
                    autoplayEnds.Add(marker.time);
                }

                else if(marker is AutoplayPause) {
                    autoplayEnds.Add(marker.time - .015d);
                    autoplayStarts.Add(marker.time + .015d);
                }

                if (marker is IVideoConfigurator videoConfigurator && videoConfigurator.isVideoSequence == true)  {
                    videoIntervalIds.Add(videoId);
                }
                
                if (marker is IMarkerDescription markerDescription)  {
                    descriptions.Add(markerDescription.description);
                } else  {
                    descriptions.Add("");
                }

                videoId++;
            }

            return new Tuple<List<double>, List<double>, List<int>, List<string>>(autoplayStarts, autoplayEnds, videoIntervalIds, descriptions);
        }
        
        private static AutorunData CreateAutorunData(Sequence targetSequence, List<double> autoplayStarts, List<double> autoplayEnds, List<int> videoIntervalIds, List<string> descriptions)
        {
            List<Interval> autorunIntervals = CreateAutorunIntervals(autoplayStarts, autoplayEnds, videoIntervalIds, descriptions);
            
            return AutorunData.CreateInstance(targetSequence, autorunIntervals);
        }
        
        private static List<Interval> CreateAutorunIntervals(List<double> startTimes, List<double> endTimes, List<int> videoIntervalIds, List<string> descriptions)
        {
            List<Interval> autorunIntervals = new List<Interval>();

            if(startTimes.Count != endTimes.Count) {
                if(startTimes.Count != endTimes.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }

            for(int i=0; i<startTimes.Count; i++)
            {
                Interval interval;
                
                if(i <= endTimes.Count - 1) {
                    interval = new Interval(startTimes[i], endTimes[i], descriptions[i]);
                } else  {
                    interval = new Interval(startTimes[i], 100000, descriptions[i]);
                }
                
                if(videoIntervalIds.Contains(i) == true) {
                    interval.isVideoSequence = true;
                }
                
                autorunIntervals.Add(interval);
            }

            return autorunIntervals;
        }

#endif
        [Serializable]
        public class AutorunData : InputData
        {
            [SerializeField]
            [ReadOnly]
            private List<Interval> _autorunIntervals;

            public List<Interval> autorunIntervals
            {
                get => _autorunIntervals;
                set => _autorunIntervals = value;
            }
            
            private bool _autoplayActive;

            public bool autoplayActive
            {
                get => _autoplayActive;
                set => _autoplayActive = value;
            }

            private bool _isLerping;

            public bool isLerping
            {
                get => _isLerping;
                set => _isLerping = value;
            }
            
            public static AutorunData CreateInstance(Sequence sequence, List<Interval> autorunIntervals)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(AutorunData)) as AutorunData;
                var inputData = new AutorunData {sequence = sequence, autorunIntervals = autorunIntervals};
                
                return inputData;
            }
            
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}