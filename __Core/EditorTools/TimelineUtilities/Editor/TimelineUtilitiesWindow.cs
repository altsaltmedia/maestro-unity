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
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt
{

    public class TimelineUtilitiesWindow : EditorWindow
    {
        Vector2 scrollPos;
        int toolbarInt = 0;
        string[] toolbarStrings = { "Clip Tools", "Create Tools" };

        FloatField playheadTime;

        public float currentTime;
        public int selectionCount = 1;
        public bool callTransposeUnselectedClips = false;
        public float durationMultiplier = 1;
        public float targetDuration = 1;
        public float targetSpacing = 0;

        bool showTimelineClipTools = false;
        bool showTrackClipTools = false;

        Undo.UndoRedoCallback undoCallback = new Undo.UndoRedoCallback(TimelineUtilitiesCore.RefreshTimelineWindow);

        SerializedObject serializedObject;

        public enum ButtonNames {
            SelectEndingBefore,
            SelectStartingBefore,
            SelectEndingAfter,
            SelectStartingAfter,
            AddPrevClipToSelection,
            DecrementSelectionCount,
            IncrementSelectionCount,
            AddNextClipToSelection,
            SetToPlayhead,
            TransposeToPlayhead,
            ExpandToPlayhead,
            ExpandAndTranposeToPlayhead,
            MultiplyDuration,
            MultiplyDurationAndTranspose,
            SetDuration,
            SetDurationAndTranspose,
            SetSpacing,
            AddSubtractSpacing,
            SetSequentialOrder,
            SetSequentialOrderReverse,
            DeselectAll,
            RefreshTimelineWindow,
            RefreshLayout
        }; 

        [MenuItem("Tools/AltSalt/Timeline Utilities")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineUtilitiesWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(TimelineUtilitiesWindow)).Show();
        }

        void OnEnable()
        {
            RenderLayout();
            Undo.undoRedoPerformed += undoCallback;
        }

        void RenderLayout()
        {
            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilities_Style.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var clipToolsTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilities_ClipTools.uxml");
            VisualElement clipToolsFromXML = clipToolsTree.CloneTree();
            rootVisualElement.Add(clipToolsFromXML);

            serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(serializedObject);

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);
        }

        Button SetupButton(Button button)
        {
            switch(button.name) {

                case nameof(ButtonNames.SelectEndingBefore) :
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SelectEndingBefore(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingBefore):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SelectStartingBefore(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectEndingAfter):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SelectEndingAfter(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingAfter):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SelectStartingAfter(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.AddPrevClipToSelection):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.AddPrevClipToSelection(TimelineEditor.selectedClips, currentTime, selectionCount);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.DecrementSelectionCount):
                    button.clickable.clicked += () => {
                        selectionCount--;
                        if (selectionCount < 1) {
                            selectionCount = 1;
                        }
                    };
                    break;

                case nameof(ButtonNames.IncrementSelectionCount):
                    button.clickable.clicked += () => {
                        selectionCount++;
                    };
                    break;

                case nameof(ButtonNames.AddNextClipToSelection):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.AddNextClipToSelection(TimelineEditor.selectedClips, currentTime, selectionCount);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetToPlayhead(ClipTools.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.TransposeToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.TransposeToPlayhead(ClipTools.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.MultiplyDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.MultiplyDuration(ClipTools.GetCurrentClipSelection(), durationMultiplier, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.MultiplyDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.MultiplyDurationAndTranspose(ClipTools.GetCurrentClipSelection(), durationMultiplier, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetDuration(ClipTools.GetCurrentClipSelection(), targetDuration, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetDurationAndTranspose(ClipTools.GetCurrentClipSelection(), targetDuration, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetSpacing(ClipTools.GetCurrentClipSelection(), targetSpacing, callTransposeUnselectedClips, ClipTools.GetAllTimelineClips(), ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.AddSubtractSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.AddSubtractSpacing(ClipTools.GetCurrentClipSelection(), ClipTools.GetAllTimelineClips(), targetSpacing);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSequentialOrder):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetSequentialOrder(ClipTools.GetCurrentClipSelection(), ClipTools.GetAllTimelineClips(), callTransposeUnselectedClips, ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSequentialOrderReverse):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipTools.SetSequentialOrderReverse(ClipTools.GetCurrentClipSelection(), ClipTools.GetAllTimelineClips(), callTransposeUnselectedClips, ClipTools.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.RefreshTimelineWindow):
                    button.clickable.clicked += () => {
                        TimelineUtilitiesCore.RefreshTimelineWindow();
                    };
                    break;

                case nameof(ButtonNames.DeselectAll):
                    button.clickable.clicked += () => {
                        ClipTools.DeselectAll();
                    };
                    break;

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        RenderLayout();
                    };
                    break;
            }

            return button;
        }

        void OnInspectorUpdate()
        {
            currentTime = TimelineUtilitiesCore.CurrentTime;
            if (TimelineEditor.inspectedDirector != null) {
                TimelineEditor.inspectedDirector.time = currentTime;
                TimelineEditor.inspectedDirector.DeferredEvaluate();
            }
        }

        public void OnGUI()
        {

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
            
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            //showTimelineClipTools = EditorGUILayout.Foldout(showTimelineClipTools, "Clip Tools");
            //if (showTimelineClipTools == true) {
                //ClipTools.ShowClipTools(position, rootVisualElement, this);
            //}

            GUILayout.Space(10);

            //showTrackClipTools = EditorGUILayout.Foldout(showTrackClipTools, "Tracks and Clips");
            //if (showTrackClipTools == true) {
            //    CreateTrackClipTools.ShowTrackClipTools();
            //}

            //EditorGUILayout.EndScrollView();
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

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
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
