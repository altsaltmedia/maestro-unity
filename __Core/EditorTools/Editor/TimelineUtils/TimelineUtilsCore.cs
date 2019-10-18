using System;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using System.Linq;

namespace AltSalt
{
    public static class TimelineUtilsCore
    {
        static FloatReference currentTime = new FloatReference();

        public static float CurrentTime {

            get {
                PopulateTimeReference();
                return currentTime.Value;
            }

            set {
                PopulateTimeReference();
                currentTime.Variable.SetValue(value);
            }

        }

        public static FloatReference CurrentTimeReference {

            get {
                PopulateTimeReference();
                return currentTime;
            }
        }

        static TimelineAsset currentTimelineAsset;
        static bool debugTrackCreated;
        static bool createDebugTrackCalled;

        public static bool DebugTrackCreated {

            get {
                if(TimelineEditor.inspectedAsset == null) {
                    return false;
                }

                if(createDebugTrackCalled == true || currentTimelineAsset == null || currentTimelineAsset != TimelineEditor.inspectedAsset) {
                    currentTimelineAsset = TimelineEditor.inspectedAsset;
                    if (FindDebugTrack(currentTimelineAsset) == true) {
                        debugTrackCreated = true;
                    } else {
                        debugTrackCreated = false;
                    }
                    createDebugTrackCalled = false;
                }

                return debugTrackCreated;
            }
        }

        public static FloatReference PopulateTimeReference()
        {
            if(currentTime.Variable == null) {
                currentTime.Variable = Utils.GetFloatVariable("TimelineCurrentTime");
            }
            return currentTime;
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
            currentDirector.SetGenericBinding(debugTrack, CurrentTimeReference.Variable);
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