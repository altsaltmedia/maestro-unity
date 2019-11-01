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
    public partial class Autorun_Controller : Input_Controller
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
        private List<Autorun_Data> _autorunData = new List<Autorun_Data>();

        public List<Autorun_Data> autorunData
        {
            get => _autorunData;
        }

#if UNITY_EDITOR

        public override void ConfigureData()
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
                    var (item1, item2, item3, item4, item5) = GetConfigTimes(markers);

                    autorunData.Add(
                        CreateAutorunData(sequence, item1, item2, item3, item4, item5));
                }
            }
            EditorUtility.SetDirty(this);
        }

        private static Tuple<List<double>, List<double>, List<int>, List<string>, List<int>> GetConfigTimes(IEnumerable<IMarker> markers)
        {
            List<double> autoplayStarts = new List<double>();
            List<double> autoplayEnds = new List<double>();
            List<int> videoIntervalIds = new List<int>();
            List<string> descriptions = new List<string>();
            List<int> isEndIds = new List<int>();

            int markerId = 0;

            foreach (IMarker marker in markers) {
                
                if (marker is Autorun_Start) {
                    autoplayStarts.Add(marker.time);
                }

                else if (marker is Autorun_End) {
                    autoplayEnds.Add(marker.time);
                    isEndIds.Add(markerId - 1);
                }

                else if(marker is Autorun_Pause) {
                    autoplayEnds.Add(marker.time - .015d);
                    autoplayStarts.Add(marker.time + .015d);
                }

                if (marker is IVideoConfigurator videoConfigurator && videoConfigurator.isVideoSequence == true)  {
                    videoIntervalIds.Add(markerId);
                }
                
                if (marker is IMarkerDescription markerDescription)  {
                    descriptions.Add(markerDescription.description);
                }

                markerId++;
            }

            return new Tuple<List<double>, List<double>, List<int>, List<string>, List<int>>(autoplayStarts, autoplayEnds, videoIntervalIds, descriptions, isEndIds);
        }
        
        private static Autorun_Data CreateAutorunData(Sequence targetSequence, List<double> autoplayStarts, List<double> autoplayEnds, List<int> videoIntervalIds, List<string> descriptions, List<int> isEndIds)
        {
            List<Autorun_Interval> autorunIntervals = CreateAutorunIntervals(autoplayStarts, autoplayEnds, videoIntervalIds, descriptions, isEndIds);
            
            return Autorun_Data.CreateInstance(targetSequence, autorunIntervals);
        }
        
        private static List<Autorun_Interval> CreateAutorunIntervals(List<double> startTimes, List<double> endTimes, List<int> videoIntervalIds, List<string> descriptions, List<int> isEndIds)
        {
            List<Autorun_Interval> autorunIntervals = new List<Autorun_Interval>();

            if(startTimes.Count != endTimes.Count) {
                if(startTimes.Count != endTimes.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }

            for(int i=0; i<startTimes.Count; i++)
            {
                Autorun_Interval interval;
                
                if(i <= endTimes.Count - 1) {
                    interval = new Autorun_Interval(startTimes[i], endTimes[i], descriptions[i]);
                } else  {
                    interval = new Autorun_Interval(startTimes[i], 100000, descriptions[i]);
                }
                
                if(videoIntervalIds.Contains(i) == true) {
                    interval.isVideoSequence = true;
                }

                if (isEndIds.Contains(i) == true)
                {
                    interval.isEnd = true;
                }
                
                autorunIntervals.Add(interval);
            }

            return autorunIntervals;
        }

#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}