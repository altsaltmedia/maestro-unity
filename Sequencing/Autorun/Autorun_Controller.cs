using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using System.Linq;
using AltSalt.Maestro.Sequencing.Navigate;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [ExecuteInEditMode]
    [Serializable]
    public class Autorun_Controller : Input_Controller
    {
        [SerializeField]
        private Autoplayer _autoplayer;

        private Autoplayer autoplayer => _autoplayer;

        [SerializeField]
        private Lerper _lerper;

        public Lerper lerper => _lerper;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private BoolReference _isReversing;

        public bool isReversing => _isReversing.Value;

        [SerializeField]
        [InfoBox("Autorun data is populated dynamically from connected Master Sequences")]
        private List<Autorun_Data> _autorunData = new List<Autorun_Data>();

        public List<Autorun_Data> autorunData => _autorunData;

//#if UNITY_EDITOR

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

                    var markerConfig = Tuple.Create(new List<double>(), new List<double>(), new List<int>(), new List<string>(), new List<int>());

                    if (rootTimelineAsset.markerTrack != null) {
                        IEnumerable<IMarker> markers = rootTimelineAsset.markerTrack.GetMarkers().OrderBy(s => s.time);
                        markerConfig = GetConfigTimes(markers);
                    }

                    autorunData.Add(
                        CreateAutorunData(sequence, markerConfig.Item1, markerConfig.Item2, markerConfig.Item3, markerConfig.Item4, markerConfig.Item5));
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
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
                
                if (marker is AutorunMarker_Start) {
                    autoplayStarts.Add(marker.time);
                }

                else if (marker is AutorunMarker_End) {
                    autoplayEnds.Add(marker.time);
                    isEndIds.Add(markerId - 1);
                }

                else if(marker is AutorunMarker_Pause) {
                    autoplayEnds.Add(marker.time);
                    autoplayStarts.Add(marker.time);
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
            List<AutorunExtents> autorunIntervals = CreateAutorunExtents(autoplayStarts, autoplayEnds, videoIntervalIds, descriptions, isEndIds);
            
            return Autorun_Data.CreateInstance(targetSequence, autorunIntervals);
        }
        
        private static List<AutorunExtents> CreateAutorunExtents(List<double> startTimes, List<double> endTimes, List<int> videoIntervalIds, List<string> descriptions, List<int> isEndIds)
        {
            List<AutorunExtents> autorunExtents = new List<AutorunExtents>();

            if(startTimes.Count != endTimes.Count) {
                if(startTimes.Count != endTimes.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }

            for(int i=0; i<startTimes.Count; i++)
            {
                AutorunExtents extents;
                
                if(i <= endTimes.Count - 1) {
                    extents = new AutorunExtents(startTimes[i], endTimes[i], descriptions[i]);
                } else {
                    extents = new AutorunExtents(startTimes[i], double.MaxValue, descriptions[i]);
                }
                
                if(videoIntervalIds.Contains(i) == true) {
                    extents.isVideoSequence = true;
                }

                if (isEndIds.Contains(i) == true)
                {
                    extents.isEnd = true;
                }
                
                autorunExtents.Add(extents);
            }

            return autorunExtents;
        }

//#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}