using System;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Maestro.Sequencing;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    public static class TimelineUtils
    {
        private static readonly FloatReference _currentTime = new FloatReference();

        private static AppSettings _appSettings;

        private static AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
        }

        public static float currentTime {

            get => appSettings.timelineDebugTime;

            set
            {
                appSettings.timelineDebugTime = value;
                TimelineEditor.inspectedDirector.time = value;
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
                    Debug.LogError("Please select a timeline instance");
                    return null;
                }
                
                ConfigTrack currentConfigTrack = Utils.GetTrackFromTimelineAsset(TimelineEditor.inspectedAsset, typeof(ConfigTrack)) as ConfigTrack;

                if (currentConfigTrack == null) {
                    Debug.LogError("Timeline asset must contain a config track");
                    return null;
                }

                return currentConfigTrack;
            }
        }

        public static MarkerTrack GetMarkerTrack()
        {
            if (TimelineEditor.inspectedAsset.markerTrack == null) {
                TimelineEditor.inspectedAsset.CreateMarkerTrack();
            }

            return TimelineEditor.inspectedAsset.markerTrack;
        }

        public static void FocusTimelineWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/Sequencing/Timeline");
        }

        public static List<TrackAsset> GetAllTracks()
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            foreach (TrackAsset rootTrack in TimelineEditor.inspectedAsset.GetRootTracks()) {
                trackAssets.AddRange(GetChildTracks(rootTrack));
            }
            return trackAssets;
        }
        
        public static List<TrackAsset> GetAllTracks(TimelineAsset sourceTimelineAsset)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            foreach (TrackAsset rootTrack in sourceTimelineAsset.GetRootTracks()) {
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
                        targetClips[i].displayName = String.Format("{0} ({1})", newName, i);
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
                if (trackAsset is TimelineUtilsTrack) {
                    foreach (TimelineClip debugClip in trackAsset.GetClips()) {
                        debugTrackFound = true;
                        break;
                    }
                }
            }
            return debugTrackFound;
        }

        public static void CreateDebugTrack(TimelineAsset targetAsset)
        {
            TrackAsset debugTrack = targetAsset.CreateTrack(typeof(TimelineUtilsTrack), null, "Debug Track");
            debugTrack.CreateDefaultClip();
            RefreshTimelineContentsAddedOrRemoved();
            createDebugTrackCalled = true;
        }
        
        public static void CreateConfigTrack(TimelineAsset targetAsset)
        {
            TrackAsset configTrack = targetAsset.CreateTrack(typeof(ConfigTrack), null, "Config Track");
            configTrack.CreateDefaultClip();
            RefreshTimelineContentsAddedOrRemoved();
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

        public static Object[] GetAssociatedTracksFromSelection(Object[] objectSelection, TimelineClip[] clipSelection, TimelineAsset sourceTimelineAsset, PlayableDirector sourceDirector)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();

            for (int i = 0; i < objectSelection.Length; i++) {

                foreach (PlayableBinding playableBinding in sourceTimelineAsset.outputs) {

                    Object objectBinding = sourceDirector.GetGenericBinding(playableBinding.sourceObject);

                    if (objectSelection[i] is GameObject && objectBinding is Component) {

                        if (objectSelection[i] == (objectBinding as Component).gameObject) {
                            trackAssets.Add(playableBinding.sourceObject as TrackAsset);
                        }

                    } else if (objectSelection[i] == objectBinding) {
                        trackAssets.Add(playableBinding.sourceObject as TrackAsset);
                    }
                }

            }

            for (int i = 0; i < clipSelection.Length; i++) {
                TrackAsset trackAsset = clipSelection[i].parentTrack;
                if (trackAssets.Contains(trackAsset) == false) {
                    trackAssets.Add(trackAsset);
                }
            }

            List<TrackAsset> allParentTracks = new List<TrackAsset>();
            for (int z=0; z<trackAssets.Count; z++) {
                List<TrackAsset> parentTracks = GetParentTracks(trackAssets[z]);
                
                // Avoid adding duplicate parents to our selection
                for (int m = 0; m < parentTracks.Count; m++) {
                    if(allParentTracks.Contains(parentTracks[m]) == false) allParentTracks.Add(parentTracks[m]);
                }
            }

            trackAssets.AddRange(allParentTracks);

            return trackAssets.ToArray();
        }

        public static List<TrackAsset> GetParentTracks(TrackAsset trackAsset)
        {
            List<TrackAsset> parentTracks = new List<TrackAsset>();
            if (trackAsset.parent != null && trackAsset.parent is GroupTrack) {
                TrackAsset parentTrack = trackAsset.parent as TrackAsset;
                parentTracks.Add(parentTrack);
                parentTracks.AddRange(GetParentTracks(parentTrack));
            }
            return parentTracks;
        }

        public static TrackAsset[] SortTracks(TrackAsset[] selectedTracks)
        {
            List<TrackAsset> allTracks = GetAllTracks();
            List<TrackAsset> sortedTracks = new List<TrackAsset>();
            
            for (int i = 0; i < allTracks.Count; i++) {
                TrackAsset currentTrack = allTracks[i];
                
                for (int j = 0; j < selectedTracks.Length; j++) {
                    if (currentTrack == selectedTracks[j]) {
                        sortedTracks.Add(selectedTracks[j]);
                    }
                }
            }

            return sortedTracks.ToArray();
        }

        public static TimelineClip[] SelectClipsEndingBefore(Object[] selection, float targetTime)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClipsFromSelection(selection)) {
                if (clip.end < targetTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static Marker[] SelectMarkersEndingBefore(Object[] selection, float targetTime)
        {
            List<Marker> markerSelection = new List<Marker>();
            foreach (Marker marker in GetMarkersFromSelection(selection)) {
                if (marker.time <= targetTime) {
                    markerSelection.Add(marker);
                }
            }

            return markerSelection.ToArray();
        }
        
        public static Marker[] SelectMarkersStartingAfter(Object[] selection, float targetTime)
        {
            List<Marker> markerSelection = new List<Marker>();
            foreach (Marker marker in GetMarkersFromSelection(selection)) {
                if (marker.time >= targetTime) {
                    markerSelection.Add(marker);
                }
            }

            return markerSelection.ToArray();
        }

        public static TimelineClip[] SelectClipsStartingAfter(Object[] selection, float targetTime)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in TimelineUtils.GetTimelineClipsFromSelection(selection)) {
                if (clip.start > targetTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] SelectClipsEndingAfter(Object[] selection, float targetTime)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in TimelineUtils.GetTimelineClipsFromSelection(selection)) {
                if (clip.end > targetTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] SelectClipsStartingBefore(Object[] selection, float targetTime)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in TimelineUtils.GetTimelineClipsFromSelection(selection)) {
                if (clip.start < targetTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }
        
        public static TimelineClip[] GetTimelineClipsFromSelection(Object[] selection)
        {
            List<TimelineClip> selectedTrackClips = new List<TimelineClip>();
            bool trackAssetSelected = false;

            List<TrackAsset> selectedTrackAssets = new List<TrackAsset>();

            if (selection != null && selection.Length > 0) {
                for (int i = 0; i < selection.Length; i++) {
                    if (selection[i] is TrackAsset) {
                        trackAssetSelected = true;
                        TrackAsset trackAsset = selection[i] as TrackAsset;

                        selectedTrackAssets.Add(trackAsset);
                        selectedTrackAssets.AddRange(TimelineUtils.GetChildTracks(trackAsset));
                    }
                }
            }

            if (trackAssetSelected == true) {
                for(int z=0; z<selectedTrackAssets.Count; z++) {
                    selectedTrackClips.AddRange(selectedTrackAssets[z].GetClips());
                }
                return selectedTrackClips.ToArray();

            } else {
                return GetAllTimelineClips();
            }
        }
        
        public static Marker[] GetMarkersFromSelection(Object[] selection)
        {
            List<IMarker> selectedMarkers = new List<IMarker>();
            bool trackAssetSelected = false;

            List<TrackAsset> selectedTrackAssets = new List<TrackAsset>();

            if (selection != null && selection.Length > 0) {
                for (int i = 0; i < selection.Length; i++) {
                    if (selection[i] is TrackAsset) {
                        trackAssetSelected = true;
                        TrackAsset trackAsset = selection[i] as TrackAsset;

                        selectedTrackAssets.Add(trackAsset);
                        selectedTrackAssets.AddRange(TimelineUtils.GetChildTracks(trackAsset));
                    }
                }
            }

            if (trackAssetSelected == true) {
                for(int z=0; z<selectedTrackAssets.Count; z++) {
                    foreach (var marker in selectedTrackAssets[z].GetMarkers()) {
                        if(marker is JoinMarker_IJoinSequence == false)
                        {
                            selectedMarkers.Add(marker);
                        }
                    }
                }
                
                TrackAsset markerTrack = TimelineEditor.inspectedAsset.markerTrack;
                foreach (var marker in markerTrack.GetMarkers()) {
                    if(marker is JoinMarker_IJoinSequence == false)
                    {
                        selectedMarkers.Add(marker);
                    }
                }

                List<Marker> convertedMarkers = selectedMarkers.ConvertAll(x => (Marker) x);
                return convertedMarkers.ToArray();

            } else {
                return GetAllMarkers();
            }
        }

        public static TimelineClip[] GetAllTimelineClips()
        {
            return GetAllTimelineClips(new Utils.ClipTimeSort());
        }

        public static TimelineClip[] GetAllTimelineClips(Comparer<TimelineClip> comparer)
        {
            IEnumerable<PlayableBinding> playableBindings = TimelineEditor.inspectedAsset.outputs;

            List<TimelineClip> allClips = new List<TimelineClip>();

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.hasClips == false || trackAsset is TimelineUtilsTrack) {
                    continue;
                }

                allClips.AddRange(trackAsset.GetClips());
            }

            if (comparer != null) {
                allClips.Sort(comparer);
            }

            return allClips.ToArray();
        }
        
        public static Marker[] GetAllMarkers()
        {
            return GetAllMarkers(new Utils.IMarkerTimeSort());
        }
        
        public static Marker[] GetAllMarkers(Comparer<IMarker> comparer)
        {
            IEnumerable<PlayableBinding> playableBindings = TimelineEditor.inspectedAsset.outputs;

            List<IMarker> allMarkers = new List<IMarker>();

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.hasClips == false || trackAsset is TimelineUtilsTrack) {
                    continue;
                }

                foreach (var marker in trackAsset.GetMarkers()) {
                    if(marker is JoinMarker_IJoinSequence == false)
                    {
                        allMarkers.Add(marker);
                    }
                }
            }

            TrackAsset markerTrack = TimelineEditor.inspectedAsset.markerTrack;
            foreach (var marker in markerTrack.GetMarkers()) {
                if(marker is JoinMarker_IJoinSequence == false)
                {
                    allMarkers.Add(marker);
                }
            }

            if (comparer != null) {
                allMarkers.Sort(comparer);
            }

            List<Marker> convertedMarkers = allMarkers.ConvertAll(x => (Marker) x);
            return convertedMarkers.ToArray();
        }
    }
}