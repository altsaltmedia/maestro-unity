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
        
        static double durationMultiplier = 0;
        static double targetDuration = 1;
        static double targetSpacing = 0;

        public static void ShowTimelineTools()
        {
            ShowClipTools();       
        }

        static void ShowClipTools()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Refresh Timeline Window")) {
                TimelineUtilitiesCore.RefreshTimelineWindow();
            }

            if (GUILayout.Button("Deselect All")) {
                DeselectAll();
            }

            if (GUILayout.Button("Deselect Clips")) {
                DeselectClips();
            }

            GUILayout.Space(10);

            GUILayout.Label("Current time : " + TimelineUtilitiesCore.CurrentTime.ToString("N"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("< Select Ending Before")) {
                SelectClipsEndingBeforePlayhead(Selection.objects);
            }

            if (GUILayout.Button("Select Starting After >")) {
                SelectClipsStartingAfterPlayhead(Selection.objects);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("< Select Starting Before")) {
                SelectClipsStartingBeforePlayhead(Selection.objects);
            }

            if (GUILayout.Button("Select Ending After >")) {
                SelectClipsEndingAfterPlayhead(Selection.objects);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("< Select Previous Clip")) {
                SelectPreviousClip();
            }

            if (GUILayout.Button("Select Next Clip >")) {
                SelectNextClip();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Set Start Time")) {
                SetStartTime();
            }

            if (GUILayout.Button("Shift to Start Time")) {
                ShiftToStartTime();
            }

            GUILayout.Space(10);

            durationMultiplier = EditorGUILayout.DoubleField("Multiply selected clips by :", durationMultiplier);
            if (GUILayout.Button("Multiply And Shift")) {
                MultiplyAndShift();
            }
            if (GUILayout.Button("Multiply Clips")) {
                MultiplyClips();
            }

            GUILayout.Space(10);

            targetDuration = EditorGUILayout.DoubleField("Set selected clips' length to :", targetDuration);
            if (GUILayout.Button("Set Clip Duration")) {
                SetDuration();
            }

            GUILayout.Space(10);

            targetSpacing = EditorGUILayout.DoubleField("Set space between clips to :", targetSpacing);
            if (GUILayout.Button("Set Sequential Order")) {
                SetSequentialOrder();
            }

            //if (GUILayout.Button("Collapse Groups")) {
            //    CollapseGroups();
            //}
        }

        static void DeselectAll()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            Selection.objects = new UnityEngine.Object[0];
            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        static void DeselectClips()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        static TimelineClip[] SelectClipsEndingBeforePlayhead(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            TimelineEditor.selectedClips = clipSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();

            return TimelineEditor.selectedClips;
        }

        static TimelineClip[] SelectClipsStartingAfterPlayhead(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            TimelineEditor.selectedClips = clipSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();

            return TimelineEditor.selectedClips;
        }

        static TimelineClip[] SelectClipsEndingAfterPlayhead(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            TimelineEditor.selectedClips = clipSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();

            return TimelineEditor.selectedClips;
        }

        static TimelineClip[] SelectClipsStartingBeforePlayhead(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            TimelineEditor.selectedClips = clipSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();

            return TimelineEditor.selectedClips;
        }

        static void SelectNextClip()
        {
            List<TimelineClip> allClips = GetTimelineClips();
            TimelineClip clipToSelect = null;
            List<TimelineClip> currentSelection = new List<TimelineClip>();
            currentSelection.AddRange(TimelineEditor.selectedClips);

            foreach (TimelineClip clip in allClips) {
                if (currentSelection.Contains(clip) || clip.start < TimelineUtilitiesCore.CurrentTime) {
                    continue;
                }

                if (clipToSelect == null) {
                    clipToSelect = clip;
                } else if (Mathf.Abs((float)clip.start - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)clipToSelect.start - TimelineUtilitiesCore.CurrentTime)) {
                    clipToSelect = clip;
                }

            }
            Debug.Log(clipToSelect);
            currentSelection.Add(clipToSelect);

            TimelineEditor.selectedClips = currentSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        static void SelectPreviousClip()
        {
            List<TimelineClip> allClips = GetTimelineClips();
            TimelineClip clipToSelect = null;
            List<TimelineClip> currentSelection = new List<TimelineClip>();
            currentSelection.AddRange(TimelineEditor.selectedClips);

            foreach (TimelineClip clip in GetTimelineClips()) {
                if (currentSelection.Contains(clip) || clip.end > TimelineUtilitiesCore.CurrentTime) {
                    continue;
                }

                if (clipToSelect == null) {
                    clipToSelect = clip;
                } else if (Mathf.Abs((float)clip.end - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)clipToSelect.end - TimelineUtilitiesCore.CurrentTime)) {
                    clipToSelect = clip;
                }
            }
            Debug.Log(clipToSelect);
            currentSelection.Add(clipToSelect);

            TimelineEditor.selectedClips = currentSelection.ToArray();
            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        static void MultiplyAndShift()
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
                        Undo.RecordObject(clip.asset, "Multiply and shift clips");
                        clip.start += difference;
                    }
                }
            }

            TimelineUtilitiesCore.RefreshTimelineWindow();
        }


        static List<TimelineClip> GetTimelineClips(Object[] selection)
        {
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            if(selection != null && selection.Length > 0) {
                for (int i=0; i<selection.Length; i++) {
                    if(selection[i] is TrackAsset) {
                        TrackAsset trackAsset = selection[i] as TrackAsset;
                        if(trackAsset.GetChildTracks() != null) {
                            selectedClips.AddRange(trackAsset.GetClips());
                        }
                    }
                }
                return selectedClips;
            } else {
                return GetTimelineClips();
            }
        }

        static List<TimelineClip> GetTimelineClips()
        {
            TimelineAsset currentAsset = TimelineEditor.inspectedAsset;
            IEnumerable<PlayableBinding> playableBindings = currentAsset.outputs;
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.GetChildTracks() == null) {
                    continue;
                }

                selectedClips.AddRange(trackAsset.GetClips());
            }
            return selectedClips;
        }

        static void SetStartTime()
        {
            Undo.RecordObjects(Selection.objects, "Set clip start time");
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
            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        static void ShiftToStartTime()
        {
            Undo.RecordObjects(Selection.objects, "Shift clips to start time");
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
            TimelineUtilitiesCore.RefreshTimelineWindow();
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

        static void SetSequentialOrder()
        {
            List<TimelineClip> selectedClips = new List<TimelineClip>();
            selectedClips.AddRange(TimelineEditor.selectedClips);
            selectedClips.Sort(new ClipTimeSort());

            List<TimelineClip> allClips = GetTimelineClips();
            allClips.Sort(new ClipTimeSort());

            foreach (TimelineClip selectedClip in selectedClips) {

                double newStartTime = selectedClip.end + targetSpacing;

                foreach (TimelineClip clip in allClips) {
                    if (clip == selectedClip) {
                        continue;
                    }

                    if (clip.start >= selectedClip.start) {
                        Undo.RecordObject(clip.asset, "Multiply and shift clips");
                        clip.start = newStartTime;
                    }
                }
            }

            TimelineUtilitiesCore.RefreshTimelineWindow();
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
    }
}