using System;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Maestro.Sequencing;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    public static class TimelineUtils
    {
        private static readonly FloatReference _currentTime = new FloatReference();
        
        public static float currentTime {

            get {
                PopulateTimeReference();
                return _currentTime.Value;
            }

            set {
                PopulateTimeReference();
                _currentTime.Variable.SetValue(value);
            }

        }
        
        private static FloatReference currentTimeReference {

            get {
                PopulateTimeReference();
                return _currentTime;
            }
        }

        private static TimelineAsset _currentTimelineAsset;

        private static TimelineAsset currentTimelineAsset
        {
            get => _currentTimelineAsset;
            set => _currentTimelineAsset = value;
        }
            
        
        private static bool _createDebugTrackCalled;

        private static bool createDebugTrackCalled
        {
            get => _createDebugTrackCalled;
            set => _createDebugTrackCalled = value;
        }


        private static bool _debugTrackCreated;

        public static bool debugTrackCreated {

            get {
                if(TimelineEditor.inspectedAsset == null) {
                    return false;
                }

                if(createDebugTrackCalled == true || currentTimelineAsset == null || currentTimelineAsset != TimelineEditor.inspectedAsset) {
                    currentTimelineAsset = TimelineEditor.inspectedAsset;
                    if (FindDebugTrack(currentTimelineAsset) == true) {
                        _debugTrackCreated = true;
                    } else {
                        _debugTrackCreated = false;
                    }
                    createDebugTrackCalled = false;
                }

                return _debugTrackCreated;
            }
        }

        private static ConfigTrack _configTrack;

        public static ConfigTrack configTrack
        {
            get
            {
                if (TimelineEditor.inspectedAsset == null) {
                    return null;
                }
                
                if (_configTrack == null || currentTimelineAsset != TimelineEditor.inspectedAsset) {
                    currentTimelineAsset = TimelineEditor.inspectedAsset;
                    _configTrack = Utils.GetTrackFromTimelineAsset(currentTimelineAsset, typeof(ConfigTrack)) as ConfigTrack;
                }

                return _configTrack;
            }
        }

        private static FloatReference PopulateTimeReference()
        {
            if(_currentTime.Variable == null) {
                _currentTime.Variable = Utils.GetFloatVariable("TimelineCurrentTime");
            }
            return _currentTime;
        }

        public static List<TrackAsset> GetAllTracks()
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            foreach (TrackAsset rootTrack in TimelineEditor.inspectedAsset.GetRootTracks()) {
                trackAssets.AddRange(GetChildTracks(rootTrack));
            }
            return trackAssets;
        }

        public static List<TrackAsset> GetChildTracks(TrackAsset trackAsset)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            trackAssets.Add(trackAsset);
            foreach (TrackAsset childTrack in trackAsset.GetChildTracks()) {
                trackAssets.AddRange(GetChildTracks(childTrack));
            }
            return trackAssets;
        }

        public static TimelineClip GetTimelineClipFromTrackAsset(PlayableAsset clipAsset, TrackAsset trackAsset)
        {
            foreach(TimelineClip timelineClip in trackAsset.GetClips()) {
                if(timelineClip.asset == clipAsset) {
                    return timelineClip;
                }
            }
            return null;
        }

        public static TimelineClip[] RenameClips(string newName, TimelineClip[] targetClips)
        {
            Array.Sort(targetClips, new ClipTimeSequentialSort(GetAllTracks()));

            for (int i = 0; i < targetClips.Length; i++) {
                if (newName.Contains("{x}")) {
                    targetClips[i].displayName = newName.Replace("{x}", (i + 1).ToString());
                } else {
                    if (i == 0) {
                        targetClips[i].displayName = newName;
                    } else {
                        targetClips[i].displayName = string.Format("{0} ({1})", newName, i);
                    }
                }

            }

            return targetClips;
        }

        public static IEnumerable<IMarker> GetMarkers(TrackAsset sourceTrack)
        {
            IEnumerable<IMarker> markers = sourceTrack.GetMarkers();
            return markers.OrderBy(s => s.time);
        }

        public class ClipTimeSequentialSort : Comparer<TimelineClip>
        {
            List<TrackAsset> orderedTrackList = new List<TrackAsset>();

            public ClipTimeSequentialSort(List<TrackAsset> orderedTrackList)
            {
                this.orderedTrackList = orderedTrackList;
            }

            public override int Compare(TimelineClip x, TimelineClip y)
            {
                int xTrackPosition = 0;
                int yTrackPosition = 0;

                for (int i = 0; i < orderedTrackList.Count; i++) {
                    if (x.parentTrack == orderedTrackList[i]) {
                        xTrackPosition = i;
                    }
                    if (y.parentTrack == orderedTrackList[i]) {
                        yTrackPosition = i;
                    }
                }

                if (xTrackPosition.CompareTo(yTrackPosition) != 0) {
                    return xTrackPosition.CompareTo(yTrackPosition);
                } else {
                    return x.start.CompareTo(y.start);
                }
            }
        }

        public static bool FindDebugTrack(TimelineAsset sourceAsset)
        {
            IEnumerable<PlayableBinding> playableBindings = sourceAsset.outputs;
            bool debugTrackFound = false;

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset is DebugTimelineTrack) {
                    foreach (TimelineClip debugClip in trackAsset.GetClips()) {
                        debugTrackFound = true;
                        break;
                    }
                }
            }
            return debugTrackFound;
        }

        public static void CreateDebugTrack(TimelineAsset targetAsset, PlayableDirector targetDirector)
        {
            TrackAsset debugTrack = targetAsset.CreateTrack(typeof(DebugTimelineTrack), null, "Debug Track");

            PlayableDirector currentDirector = targetDirector;
            currentDirector.SetGenericBinding(debugTrack, currentTimeReference.Variable);
            debugTrack.CreateDefaultClip();

            RefreshTimelineContentsAddedOrRemoved();
            createDebugTrackCalled = true;
        }

        public static void RefreshTimelineWindow()
        {
            RefreshTimelineContentsAddedOrRemoved();
            RefreshTimelineContentsModified();
            RefreshTimelineRedrawWindow();
        }

        public static void RefreshTimelineContentsAddedOrRemoved()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }

        public static void RefreshTimelineContentsModified()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        public static void RefreshTimelineRedrawWindow()
        {
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }

        public static void RefreshTimelineSceneRedraw()
        {
            TimelineEditor.Refresh(RefreshReason.SceneNeedsUpdate);
        }
    }
}