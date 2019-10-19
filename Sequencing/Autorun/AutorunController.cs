using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using System.Linq;

namespace AltSalt.Sequencing.Autorun
{
    [ExecuteInEditMode]
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

        private List<AutorunData> _autorunData = new List<AutorunData>();

        public List<AutorunData> autorunData
        {
            get => _autorunData;
        }

#if UNITY_EDITOR

        public void ConfigureData()
        {
            autorunData.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceConfigs.Count; q++)
                {
                    var sequence = masterSequences[i].sequenceConfigs[q].sequence;
                    
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;
                    IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                    var (item1, item2, item3) = GetConfigTimes(markers);

                    autorunData.Add(
                        CreateAutorunData(sequence, item1, item2, item3));
                }
            }
        }

        private static Tuple<List<double>, List<double>, List<int>> GetConfigTimes(IEnumerable<IMarker> markers)
        {
            List<double> autoplayStarts = new List<double>();
            List<double> autoplayEnds = new List<double>();
            List<int> videoIntervalIds = new List<int>();

            int videoId = 0;

            foreach (IMarker marker in markers) {
                
                if (marker is AutoplayStart) {
                    autoplayStarts.Add(marker.time);
                }

                else if (marker is AutoplayEnd) {
                    autoplayEnds.Add(marker.time);
                }

                else if(marker is AutoplayPause) {
                    autoplayEnds.Add(marker.time - .03d);
                    autoplayStarts.Add(marker.time + .03d);
                }

                if (marker is IVideoConfigurator videoConfigurator)  {
                    videoIntervalIds.Add(videoId);
                }

                videoId++;
            }

            return new Tuple<List<double>, List<double>, List<int>>(autoplayStarts, autoplayEnds, videoIntervalIds);
        }
        
        private static AutorunData CreateAutorunData(Sequence targetSequence, List<double> autoplayStarts, List<double> autoplayEnds, List<int> videoIntervalIds)
        {
            List<Interval> autorunIntervals = CreateAutorunIntervals(autoplayStarts, autoplayEnds, videoIntervalIds);
            
            return AutorunData.CreateInstance(targetSequence, autorunIntervals);
        }
        
        private static List<Interval> CreateAutorunIntervals(List<double> startTimes, List<double> endTimes, List<int> videoIntervalIds)
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
                    interval = new Interval(startTimes[i], endTimes[i]);
                } else  {
                    interval = new Interval(startTimes[i], 100000);
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
                var inputData = ScriptableObject.CreateInstance(typeof(AutorunData)) as AutorunData;
                
                inputData.sequence = sequence;
                inputData.autorunIntervals = autorunIntervals;

                return inputData;
            }
            
        }
    }
}