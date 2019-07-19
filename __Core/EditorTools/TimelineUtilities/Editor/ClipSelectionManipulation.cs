using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt
{
    public static class ClipSelectionManipulation
    {
        public delegate TimelineClip[] TransposeClipsCallback(TimelineClip[] selectedClips, TimelineClip[] sourceClips, double offset, double timeReference);

        public static void DeselectAll()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            Selection.objects = new UnityEngine.Object[0];
            TimelineUtilitiesCore.RefreshTimelineContentsModified();
        }

        public static TimelineClip[] SelectEndingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClipsFromSelection(selection)) {
                if (clip.end < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] SelectStartingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClipsFromSelection(selection)) {
                if (clip.start > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] SelectEndingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClipsFromSelection(selection)) {
                if (clip.end > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] SelectStartingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClipsFromSelection(selection)) {
                if (clip.start < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        public static TimelineClip[] AddPrevClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip previousClip = GetPreviousClip(newSelection, GetAllTimelineClips(), timeReference);
                newSelection.Add(previousClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetPreviousClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip previousClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

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

        public static TimelineClip[] AddNextClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip nextClip = GetNextClip(newSelection, GetAllTimelineClips(), timeReference);
                newSelection.Add(nextClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetNextClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip nextClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if (currentSelectionList.Contains(clipToCompare) || clipToCompare.end < timeReference) {
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

        public static TimelineClip[] SetToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips);

            double startTime = timeReference;
            double difference = 0;

            if (selectedClips.Length > 0) {
                startTime = selectedClips[0].start;
                difference = timeReference - selectedClips[0].start;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "set clip(s) start time");
                selectedClips[i].start = timeReference;
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, startTime);
            }

            return selectedClips;
        }

        public static TimelineClip[] TransposeToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips);

            double startTime = timeReference;
            double difference = 0;

            if (selectedClips.Length > 0) {
                startTime = selectedClips[0].start;
                difference = timeReference - selectedClips[0].start;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "transpose clip(s) to start time");
                selectedClips[i].start += difference;

                if (i == selectedClips.Length - 1) {
                    selectedClips[0].start = timeReference;
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, startTime);
            }

            return selectedClips;
        }

        public static TimelineClip[] ExpandToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            double smallestDifference = 0;
            double originalEnd = 0;

            if(selectedClips.Length > 0) {
                originalEnd = selectedClips[0].end;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "expand clip(s) to playhead");
                if(timeReference > selectedClips[i].start) {
                    double difference = timeReference - selectedClips[i].end;
                    selectedClips[i].duration += difference;

                    if(smallestDifference.Equals(0) || Mathf.Abs((float)difference) < Mathf.Abs((float)smallestDifference)) {
                        smallestDifference = difference;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, smallestDifference, originalEnd);
            }

            return selectedClips;
        }

        public static TimelineClip[] ExpandAndTransposeToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTransposeUnselectedClips = false, TimelineClip[] sourceClips = null)
        {
            selectedClips = SortClips(selectedClips);
            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;
                double difference = timeReference - selectedClip.end;

                Undo.RecordObject(selectedClip.parentTrack, "expand and transpose clip(s) to playhead");
                if (timeReference > selectedClip.start) {
                    selectedClips[i].duration += difference;
                }

                if (executeTransposeUnselectedClips == true) {

                    double adjustedDifference = difference;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && Equals(selectedClip.start, selectedClips[i - 1].start)) {
                        adjustedDifference -= previousDifference;
                    }

                    for (int q = 0; q < sourceClips.Length; q++) {

                        TimelineClip clip = sourceClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "expand and transpose clip(s) to playhead");
                            clip.start += adjustedDifference;
                        }
                    }
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }



        public static TimelineClip[] MultiplyDuration(TimelineClip[] selectedClips, float multiplier, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply clip(s)");
                selectedClip.duration *= multiplier;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;       
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] MultiplyDurationAndTranspose(TimelineClip[] selectedClips, float multiplier, bool executeTranposeUnselectedClips = false, TimelineClip[] sourceClips = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i=0; i<selectedClips.Length; i++) {
                TimelineClip selectedClip = selectedClips[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply and transpose clip(s)");
                selectedClip.duration *= multiplier;
                double difference = selectedClip.duration - originalDuration;

                if (executeTranposeUnselectedClips == false) {
                    
                    for (int q = 0; q < selectedClips.Length; q++) {

                        TimelineClip clip = selectedClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "multiply and transpose clip(s)");
                            clip.start += difference;
                        }
                    }
                } else {

                    double adjustedDifference = difference;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && Equals(selectedClip.start, selectedClips[i - 1].start) ) {
                        adjustedDifference -= previousDifference;
                    }

                    for (int q = 0; q < sourceClips.Length; q++) {

                        TimelineClip clip = sourceClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "multiply and transpose clip(s)");
                            clip.start += adjustedDifference;
                        }
                    }
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetDuration(TimelineClip[] selectedClips, float duration, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set clip(s) duration");
                selectedClip.duration = duration;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetDurationAndTranspose(TimelineClip[] selectedClips, float duration, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set duration and transpose clip(s)");
                selectedClip.duration = duration;

                double difference = selectedClip.duration - originalDuration;

                if (executeTranposeCallback == false) {

                    for (int q = 0; q < selectedClips.Length; q++) {

                        TimelineClip clip = selectedClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "set duration and transpose clip(s)");
                            clip.start += difference;
                        }
                    }

                } else {
                    TimelineClip[] selectedClipArray = { selectedClip };
                    transposeClipsCallback(selectedClipArray, sourceClips, difference, selectedClip.start);
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSpacing(TimelineClip[] selectedClips, float spacing, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double endTimePlusSpacing = selectedClip.end + spacing;

                for (int q = 0; q < selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    double initialStartTime = clip.start;

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clip(s) spacing");
                        clip.start = endTimePlusSpacing;

                        if (executeTranposeCallback == true) {
                            double difference = clip.start - initialStartTime;
                            transposeClipsCallback(selectedClips, sourceClips, difference, initialStartTime);
                        }
                    }
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] AddSubtractSpacing(TimelineClip[] selectedClips, TimelineClip[] sourceClips, float spacing)
        {

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                for (int q = 0; q < sourceClips.Length; q++) {

                    TimelineClip clip = sourceClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "add / subtract clip(s) spacing");
                        clip.start += spacing;
                    }
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSequentialOrder(TimelineClip[] selectedClips, TimelineClip[] sourceClips, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips, new ClipTimeSequentialSort(GetAllTracks()));

            double difference = 0;

            for (int i=0; i<selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double newStartTime = selectedClip.end;

                for(int q=0; q<selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

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

            if(executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, selectedClips[0].end);
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSequentialOrderReverse(TimelineClip[] selectedClips, TimelineClip[] sourceClips, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips, new ClipTimeSequentialSort(GetAllTracks()));

            double difference = 0;

            for (int i = selectedClips.Length - 1; i >= 0; i--) {

                TimelineClip selectedClip = selectedClips[i];

                double newStartTime = selectedClip.end;

                for (int q = 0; q < selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == selectedClips.Length - 1 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially (reverse)");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, selectedClips[selectedClips.Length - 1].end);
            }

            return selectedClips;
        }

        public static TimelineClip[] TransposeTargetClips(TimelineClip[] omittedClips, TimelineClip[] targetClips, double offset, double timeReference)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(omittedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();

            for (int i = 0; i < targetClips.Length; i++) {

                TimelineClip sourceClip = targetClips[i];

                if (selectedClipsList.Contains(sourceClip)) {
                    continue;
                }

                if (sourceClip.start > timeReference || sourceClip.start.Equals(timeReference)) {
                    Undo.RecordObject(sourceClip.parentTrack, "transpose target clip(s)");
                    sourceClip.start += offset;
                    sourceClipsList.Add(sourceClip);
                }
            }

            return sourceClipsList.ToArray();
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

        public static TimelineClip[] GetAllTimelineClips()
        {
            return GetAllTimelineClips(new ClipTimeSort());
        }

        public static TimelineClip[] GetAllTimelineClips(Comparer<TimelineClip> comparer)
        {
            IEnumerable<PlayableBinding> playableBindings = TimelineEditor.inspectedAsset.outputs;

            List<TimelineClip> allClips = new List<TimelineClip>();

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.hasClips == false || trackAsset is DebugTimelineTrack) {
                    continue;
                }

                allClips.AddRange(trackAsset.GetClips());
            }

            if (comparer != null) {
                allClips.Sort(comparer);
            }

            return allClips.ToArray();
        }


        static TimelineClip[] GetTimelineClipsFromSelection(UnityEngine.Object[] selection)
        {
            List<TimelineClip> selectedTrackClips = new List<TimelineClip>();
            bool trackAssetSelected = false;

            if (selection != null && selection.Length > 0) {
                for (int i = 0; i < selection.Length; i++) {
                    if (selection[i] is TrackAsset) {
                        trackAssetSelected = true;
                        TrackAsset trackAsset = selection[i] as TrackAsset;
                        if (trackAsset.GetChildTracks() != null) {
                            selectedTrackClips.AddRange(trackAsset.GetClips());
                        }
                    }
                }
            }

            if (trackAssetSelected == true) {
                return selectedTrackClips.ToArray();
            } else {
                return GetAllTimelineClips();
            }
        }

        public static TimelineClip[] GetCurrentClipSelection()
        {
            return GetCurrentClipSelection(new ClipTimeSort());
        }

        public static TimelineClip[] GetCurrentClipSelection(Comparer<TimelineClip> comparer)
        {
            TimelineClip[] currentSelection = TimelineEditor.selectedClips;
            System.Array.Sort(currentSelection, comparer);
            return currentSelection;
        }

        public static TimelineClip[] SortClips(TimelineClip[] clips)
        {
            System.Array.Sort(clips, new ClipTimeSort());
            return clips;
        }

        public static TimelineClip[] SortClips(TimelineClip[] clips, Comparer<TimelineClip> comparer)
        {
            System.Array.Sort(clips, comparer);
            return clips;
        }

        public class ClipTimeSort : Comparer<TimelineClip>
        {
            public override int Compare(TimelineClip x, TimelineClip y)
            {
                return x.start.CompareTo(y.start);    
            }
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
        
    }
}
