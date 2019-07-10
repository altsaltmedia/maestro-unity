using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;

namespace AltSalt
{
    public static class TimelineClipTools
    {
        static int selectionCount = 0;
        static bool transposeClipsAfterSelection = false;
        static double durationMultiplier = 0;
        static double targetDuration = 1;
        static float targetSpacing = 0;
        static List<TrackAsset> orderedTrackList = new List<TrackAsset>();

        public static void ShowTimelineTools()
        {
            ShowClipTools();       
        }

        static void ShowClipTools()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Refresh Timeline Window")) {
                TimelineUtilitiesCore.RefreshTimelineRedrawWindow();
            }

            if (GUILayout.Button("Deselect All")) {
                DeselectAll();
            }

            GUILayout.Space(10);

            GUILayout.Label("Current time : " + TimelineUtilitiesCore.CurrentTime.ToString("N"));

            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("< Select Starting Before")) {
                    TimelineEditor.selectedClips = SelectStartingBefore(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

                if (GUILayout.Button("Select Starting After >")) {
                    TimelineEditor.selectedClips = SelectStartingAfter(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("< Select Ending Before")) {
                    TimelineEditor.selectedClips = SelectEndingBefore(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

                if (GUILayout.Button("Select Ending After >")) {
                    TimelineEditor.selectedClips = SelectEndingAfter(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            selectionCount = EditorGUILayout.IntField("Number of clips to select :", selectionCount);
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("< Select Previous Clip(s)")) {
                    TimelineEditor.selectedClips = AddPrevClipToSelection(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, selectionCount);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

                if (GUILayout.Button("Select Next Clip(s) >")) {
                    TimelineEditor.selectedClips = AddNextClipToSelection(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, selectionCount);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            transposeClipsAfterSelection = EditorGUILayout.Toggle("Transpose clips after selection :", transposeClipsAfterSelection);

            GUILayout.Space(10);

            if (GUILayout.Button("Set to Playhead")) {
                SetToPlayhead();
            }

            if (GUILayout.Button("Transpose to Playhead")) {
                TransposeToPlayhead();
            }

            GUILayout.Space(10);

            durationMultiplier = EditorGUILayout.DoubleField("Multiply selected clips by :", durationMultiplier);
            if (GUILayout.Button("Multiply")) {
                MultiplyClips();
            }
            if (GUILayout.Button("Multiply and Transpose")) {
                MultiplyAndTranspose();
            }

            GUILayout.Space(10);

            targetDuration = EditorGUILayout.DoubleField("Set selected clips' length to :", targetDuration);
            if (GUILayout.Button("Set Duration")) {
                SetDuration();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Set Sequential Order")) {
                SetSequentialOrder(TimelineEditor.selectedClips, new ClipTimeSequentialSort());
            }

            if (GUILayout.Button("Set Sequential Order (Reverse)")) {
                SetSequentialOrderReverse(TimelineEditor.selectedClips, new ClipTimeSequentialSort());
            }

            GUILayout.Space(10);

            targetSpacing = EditorGUILayout.FloatField("Set space between clips to :", targetSpacing);
            if (GUILayout.Button("Set Spacing")) {
                SetSpacing(TimelineEditor.selectedClips, new ClipTimeSort());
            }

            //if (GUILayout.Button("Collapse Groups")) {
            //    CollapseGroups();
            //}
        }

        static void DeselectAll()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            Selection.objects = new UnityEngine.Object[0];
            TimelineUtilitiesCore.RefreshTimelineContentsModified();
        }

        static TimelineClip[] SelectEndingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectStartingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectEndingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectStartingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] AddNextClipToSelection(TimelineClip[] currentSelection, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(currentSelection);

            for(int i=0; i<clipCount; i++) {
                TimelineClip nextClip = GetNextClip(newSelection, GetTimelineClips(), timeReference);
                newSelection.Add(nextClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetNextClip(IEnumerable<TimelineClip> currentSelection, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip nextClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(currentSelection);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if(currentSelectionList.Contains(clipToCompare) || clipToCompare.start < timeReference) {
                    continue;
                }

                if (nextClip == null) {
                    nextClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)nextClip.start - TimelineUtilitiesCore.CurrentTime)) {
                    nextClip = clipToCompare;
                }

            }

            return nextClip;
        }

        static TimelineClip[] AddPrevClipToSelection(TimelineClip[] currentSelection, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(currentSelection);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip previousClip = GetPreviousClip(newSelection, GetTimelineClips(), timeReference);
                newSelection.Add(previousClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetPreviousClip(IEnumerable<TimelineClip> currentSelection, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip previousClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(currentSelection);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if (currentSelectionList.Contains(clipToCompare) || clipToCompare.start > timeReference) {
                    continue;
                }

                if (previousClip == null) {
                    previousClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)previousClip.start - TimelineUtilitiesCore.CurrentTime)) {
                    previousClip = clipToCompare;
                }

            }

            return previousClip;
        }

        static void MultiplyAndTranspose()
        {
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            selectedClips.AddRange(TimelineEditor.selectedClips);
            selectedClips.Sort(new ClipTimeSort());

            List<TimelineClip> allClips = GetTimelineClips();
            allClips.Sort(new ClipTimeSort());

            foreach (TimelineClip selectedClip in selectedClips) {

                double originalDuration = selectedClip.duration;
                selectedClip.duration = selectedClip.duration * durationMultiplier;

                double difference = selectedClip.duration - originalDuration;

                foreach (TimelineClip clip in allClips) {
                    if (clip == selectedClip) {
                        continue;
                    }

                    if (clip.start > selectedClip.start) {
                        Undo.RecordObject(clip.parentTrack, "multiply and transpose clips");
                        clip.start += difference;
                    }
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
        }


        static List<TimelineClip> GetTimelineClips(Object[] selection)
        {
            List<TimelineClip> selectedTrackClips = new List<TimelineClip>();
            bool trackAssetSelected = false;

            if(selection != null && selection.Length > 0) {
                for (int i=0; i<selection.Length; i++) {
                    if(selection[i] is TrackAsset) {
                        trackAssetSelected = true;
                        TrackAsset trackAsset = selection[i] as TrackAsset;
                        if(trackAsset.GetChildTracks() != null) {
                            selectedTrackClips.AddRange(trackAsset.GetClips());
                        }
                    }
                }
            }

            if(trackAssetSelected == true) {
                return selectedTrackClips;
            } else {
                return GetTimelineClips();
            }
        }

        static List<TimelineClip> GetTimelineClips()
        {
            IEnumerable<PlayableBinding> playableBindings = TimelineEditor.inspectedAsset.outputs;

            List<TimelineClip> allClips = new List<TimelineClip>();

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.hasClips == false) {
                    continue;
                }

                allClips.AddRange(trackAsset.GetClips());
            }
            return allClips;
        }

        static void SetToPlayhead()
        {
            Undo.RecordObjects(Selection.objects, "set clip(s) start time");
            for (int i = 0; i < Selection.objects.Length; i++) {
                var obj = Selection.objects[i];
                var fi = obj.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) {
                    var clip = fi.GetValue(obj) as TimelineClip;
                    if (clip != null) {
                        clip.start = TimelineUtilitiesCore.CurrentTime;
                    }
                }
            }
            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
        }

        static void TransposeToPlayhead()
        {
            Undo.RecordObjects(Selection.objects, "transpose clip(s) to start time");
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            TimelineClip firstClip = null;
            for (int i = 0; i < Selection.objects.Length; i++) {
                var obj = Selection.objects[i];
                var fi = obj.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) {
                    var clip = fi.GetValue(obj) as TimelineClip;
                    if (clip != null) {
                        timelineClips.Add(clip);
                        if (firstClip == null) {
                            firstClip = clip;
                        } else {
                            if (clip.start < firstClip.start) {
                                firstClip = clip;
                            }
                        }
                    }
                }
            }
            double timeDifference = TimelineUtilitiesCore.CurrentTime - firstClip.start;
            for (int q = 0; q < timelineClips.Count; q++) {
                timelineClips[q].start += timeDifference;
            }
            firstClip.start = TimelineUtilitiesCore.CurrentTime;
            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
        }

        static void MultiplyClips()
        {
            if (durationMultiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return;
            }
            Undo.RecordObjects(Selection.objects, "Multiply clips");
            for (int i = 0; i < Selection.objects.Length; i++) {
                var obj = Selection.objects[i];
                var fi = obj.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) {
                    var clip = fi.GetValue(obj) as TimelineClip;
                    if (clip != null) {
                        double newDuration = clip.duration * durationMultiplier;
                        clip.duration = newDuration;
                    }
                }
            }

        }

        static void SetDuration()
        {
            if (targetDuration.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return;
            }
            Undo.RecordObjects(Selection.objects, "Set clip duration");
            for (int i = 0; i < Selection.objects.Length; i++) {
                var obj = Selection.objects[i];
                var fi = obj.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) {
                    var clip = fi.GetValue(obj) as TimelineClip;
                    if (clip != null) {
                        clip.duration = targetDuration;
                    }
                }
            }
        }

        static TimelineClip[] SetSequentialOrder(TimelineClip[] selectedClips, Comparer<TimelineClip> comparer)
        {
            orderedTrackList = GetAllTracks();

            List<TimelineClip> allClips = GetTimelineClips();
            allClips.Sort(comparer);

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            double difference = 0;

            for (int i=0; i<selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                double newStartTime = selectedClip.end;

                for(int q=0; q<selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if(i == 0 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;                    
                    }
                }
            }

            for (int i = 0; i < allClips.Count; i++) {

                TimelineClip clip = allClips[i];

                if (selectedClipsList.Contains(clip) == false && clip.start > selectedClipsList[0].end) {    
                    Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                    clip.start += difference;
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return selectedClips;
        }

        static TimelineClip[] SetSequentialOrderReverse(TimelineClip[] selectedClips, Comparer<TimelineClip> comparer)
        {
            orderedTrackList = GetAllTracks();

            List<TimelineClip> allClips = GetTimelineClips();
            allClips.Sort(comparer);

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            double difference = 0;

            for (int i = selectedClipsList.Count - 1; i >= 0; i--) {

                TimelineClip selectedClip = selectedClipsList[i];

                double newStartTime = selectedClip.end;

                for (int q = 0; q < selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == selectedClipsList.Count - 1 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            for (int i = 0; i < allClips.Count; i++) {

                TimelineClip clip = allClips[i];

                if (selectedClipsList.Contains(clip) == false && clip.start > selectedClipsList[0].end) {
                    Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                    clip.start += difference;
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return selectedClips;
        }


        static List<TrackAsset> GetAllTracks()
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            foreach (TrackAsset rootTrack in TimelineEditor.inspectedAsset.GetRootTracks()) {
                trackAssets.AddRange(GetChildTracks(rootTrack));
            }
            return trackAssets;
        }

        static List<TrackAsset> GetChildTracks(TrackAsset trackAsset)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            trackAssets.Add(trackAsset);
            foreach(TrackAsset childTrack in trackAsset.GetChildTracks()) {
                trackAssets.AddRange(GetChildTracks(childTrack));
            }
            return trackAssets;
        }

        static TimelineClip[] SetSpacing(TimelineClip[] selectedClips, Comparer<TimelineClip> comparer)
        {
            List<TimelineClip> allClips = GetTimelineClips();
            allClips.Sort(comparer);

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            double difference = 0;

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                double newStartTime = selectedClip.end + targetSpacing;

                for (int q = 0; q < selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == 0 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clip spacing");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            for (int i = 0; i < allClips.Count; i++) {

                TimelineClip clip = allClips[i];

                if (selectedClipsList.Contains(clip) == false && clip.start > selectedClipsList[0].end) {
                    Undo.RecordObject(clip.parentTrack, "set clip spacing");
                    clip.start += difference;
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return selectedClips;
        }

        

        public class ClipTimeSort : Comparer<TimelineClip>
        {
            // Compares by Length, Height, and Width.
            public override int Compare(TimelineClip x, TimelineClip y)
            {
                if (x.start.CompareTo(y.start) != 0) {
                    return x.start.CompareTo(y.start);
                } else {
                    return 0;
                }
            }
        }

        public class ClipTimeSequentialSort : Comparer<TimelineClip>
        {
            // Compares by Length, Height, and Width.
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

                //if (x.start.CompareTo(y.start) != 0) {
                //    return x.start.CompareTo(y.start);
                //} else {
                //    int xTrackPosition = 0;
                //    int yTrackPosition = 0;

                //    for(int i=0; i<orderedTrackList.Count; i++) {
                //        if(x.parentTrack == orderedTrackList[i]) {
                //            xTrackPosition = i;
                //        }
                //        if(y.parentTrack == orderedTrackList[i]) {
                //            yTrackPosition = i;
                //        }
                //    }

                //    return xTrackPosition.CompareTo(yTrackPosition);
                //}
            }
        }

        
    }
}