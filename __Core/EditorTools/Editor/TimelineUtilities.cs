using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt {

    public class TimelineUtilities : EditorWindow
    {
        FloatReference timelineCurrentTime = new FloatReference();
        double durationMultiplier = 0;
        double targetDuration = 1;
        bool moveStartTime = true;
        [MenuItem("Tools/AltSalt/Timeline Utilities")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineUtilities>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(TimelineUtilities)).Show();
        }

        public void OnGUI()
        {
            string[] guids;
            string path;
            guids = AssetDatabase.FindAssets("TimelineCurrentTime");
            path = AssetDatabase.GUIDToAssetPath(guids[0]);
            timelineCurrentTime.Variable = (FloatVariable)AssetDatabase.LoadAssetAtPath(path, typeof(FloatVariable));

            GUILayout.Space(10);

            if (GUILayout.Button("Refresh Timeline Window")) {
                RefreshTimelineWindow();
            }

            GUILayout.Space(10);

            GUILayout.Label("Current time : " + timelineCurrentTime.Value.ToString("N"));
            if (GUILayout.Button("Set Start Time")) {
                SetStartTime();
            }

            if (GUILayout.Button("Shift to Start Time")) {
                ShiftToStartTime();
            }

            GUILayout.Space(10);

            durationMultiplier = EditorGUILayout.DoubleField("Multiply selected clips by :", durationMultiplier);
            if (GUILayout.Button("Multiply Clips")) {
                MultiplyClips();
            }

            GUILayout.Space(10);

            targetDuration = EditorGUILayout.DoubleField("Set selected clips' length to :", targetDuration);
            if (GUILayout.Button("Set Clip Duration")) {
                SetDuration();
            }

            if (GUILayout.Button("Collapse Groups")) {
                CollapseGroups();
            }
        }

        void RefreshTimelineWindow()
        {
            UnityEditor.Timeline.TimelineEditor.Refresh(UnityEditor.Timeline.RefreshReason.ContentsModified);
        }

        void SetStartTime()
        {
            Undo.RecordObjects(Selection.objects, "Set clip start time");
            for (int i = 0; i < Selection.objects.Length; i++) {
                var obj = Selection.objects[i];
                var fi = obj.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null) {
                    var clip = fi.GetValue(obj) as TimelineClip;
                    if (clip != null) {
                        clip.start = timelineCurrentTime.Value;
                    }
                }
            }
            UnityEditor.Timeline.TimelineEditor.Refresh(UnityEditor.Timeline.RefreshReason.ContentsModified);
        }

        void ShiftToStartTime()
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
                        if(firstClip == null) {
                            firstClip = clip;
                        } else {
                            if(clip.start < firstClip.start) {
                                firstClip = clip;
                            }
                        }
                    }
                }
            }
            double timeDifference = timelineCurrentTime.Value - firstClip.start;
            for (int q = 0; q < timelineClips.Count; q++) {
                timelineClips[q].start += timeDifference;
            }
            firstClip.start = timelineCurrentTime.Value;
            UnityEditor.Timeline.TimelineEditor.Refresh(UnityEditor.Timeline.RefreshReason.ContentsModified);
        }

        void MultiplyClips()
        {
            if(durationMultiplier.Equals(0)) {
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

        void SetDuration()
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

        void CollapseGroups()
        {
            PlayableAsset playableAsset = UnityEditor.Timeline.TimelineEditor.inspectedAsset;
            IEnumerable playableOutputs = playableAsset.outputs;
            foreach (PlayableBinding playbleOutput in playableOutputs) {
                TrackAsset trackAsset = playbleOutput.sourceObject as TrackAsset;
                Debug.Log(trackAsset.GetType());
                if(trackAsset.GetType() == typeof(GroupTrack)) {
                    Debug.Log(trackAsset.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                }
            }
        }
    }
    
}
