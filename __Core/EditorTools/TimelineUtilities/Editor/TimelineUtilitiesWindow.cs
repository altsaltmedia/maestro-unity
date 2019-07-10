using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using Sirenix.OdinInspector;

namespace AltSalt
{

    public class TimelineUtilitiesWindow : EditorWindow
    {
        Vector2 scrollPos;
        int toolbarInt = 0;
        string[] toolbarStrings = { "Clip Tools", "Create Tools" };

        bool showTimelineClipTools = false;
        bool showSequencingTools = false;
        bool showTrackClipTools = false;

        [MenuItem("Tools/AltSalt/Timeline Utilities")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineUtilitiesWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(TimelineUtilitiesWindow)).Show();
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUIStyle guiStyle = new GUIStyle("Label");
            guiStyle.fontStyle = FontStyle.Italic;
            guiStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Space(10);

            //toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
            //GUILayout.Space(10);


            if (TimelineEditor.inspectedAsset == null) {
                GUILayout.Label("Please select a timeline asset in order to use clip tools.", guiStyle);
                return;
            }

            if (FindDebugTrack() == false) {

                GUILayout.Label("You must create a debug track in order to use timeline utilities.", guiStyle);
                GUILayout.Space(10);

                if (GUILayout.Button("Create Debug Track")) {
                    CreateDebugTrack();
                }

                return;
            }

            showTimelineClipTools = EditorGUILayout.Foldout(showTimelineClipTools, "Clip Tools");
            if (showTimelineClipTools == true) {
                TimelineClipTools.ShowTimelineTools();
            }

            GUILayout.Space(10);

            showSequencingTools = EditorGUILayout.Foldout(showSequencingTools, "Create Sequences");
            if (showSequencingTools == true) {
                CreateSequenceTools.ShowSequenceTools();
            }

            GUILayout.Space(10);

            showTrackClipTools = EditorGUILayout.Foldout(showTrackClipTools, "Tracks and Clips");
            if (showTrackClipTools == true) {
                CreateTrackClipTools.ShowTrackClipTools();
            }

            EditorGUILayout.EndScrollView();
        }

        static bool FindDebugTrack()
        {
            TimelineAsset currentAsset = TimelineEditor.inspectedAsset;
            IEnumerable<PlayableBinding> playableBindings = currentAsset.outputs;
            bool debugTrackCreated = false;

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset is DebugTimelineTrack) {
                    foreach (TimelineClip debugClip in trackAsset.GetClips()) {
                        debugTrackCreated = true;
                        break;
                    }
                }
            }
            return debugTrackCreated;
        }

        static void CreateDebugTrack()
        {
            TimelineAsset currentAsset = TimelineEditor.inspectedAsset;
            TrackAsset debugTrack = currentAsset.CreateTrack(typeof(DebugTimelineTrack), null, "Debug Track");

            PlayableDirector currentDirector = TimelineEditor.inspectedDirector;
            currentDirector.SetGenericBinding(debugTrack, TimelineUtilitiesCore.GetCurrentTimeReference().Variable);
            debugTrack.CreateDefaultClip();

            TimelineUtilitiesCore.RefreshTimelineWindow();
        }

        void CollapseGroups()
        {
            PlayableAsset playableAsset = UnityEditor.Timeline.TimelineEditor.inspectedAsset;
            IEnumerable playableOutputs = playableAsset.outputs;
            foreach (PlayableBinding playbleOutput in playableOutputs) {
                TrackAsset trackAsset = playbleOutput.sourceObject as TrackAsset;
                Debug.Log(trackAsset.GetType());
                if (trackAsset.GetType() == typeof(GroupTrack)) {
                    Debug.Log(trackAsset.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                }
            }
        }
    }
}
