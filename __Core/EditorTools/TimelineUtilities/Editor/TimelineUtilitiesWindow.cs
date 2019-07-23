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
using TMPro;

namespace AltSalt
{

    public class TimelineUtilitiesWindow : EditorWindow
    {
        Vector2 scrollPos;
        int toolbarInt = 0;
        string[] toolbarStrings = { "Clip Tools", "Create Tools" };

        FloatField playheadTime;
        VisualElement timelineUtilitiesStructure;
        VisualElement clipSelectionManipulation;
        VisualElement trackClipCreation;
        bool debugTrackCreated = false;

        Dictionary<EnableCondition, List<VisualElement>> toggleData = new Dictionary<EnableCondition, List<VisualElement>>();

        public float currentTime;
        public int selectionCount = 1;
        public bool callTransposeUnselectedClips = false;
        public float durationMultiplier = 1;
        public float targetDuration = 1;
        public float targetSpacing = 0;

        public List<TrackClipCreation.TrackData> copiedTracks = new List<TrackClipCreation.TrackData>();

        public bool allowBlankTracks = false;
        public float newClipDuration = .5f;
        public FloatVariable targetFloat;

        Undo.UndoRedoCallback undoCallback = new Undo.UndoRedoCallback(TimelineUtilitiesCore.RefreshTimelineWindow);

        SerializedObject editorWindowSerialized;
        SerializedObject floatVarSerialized;

        enum EnableCondition
        {
            TextSelected,
            RectTransformSelected,
            SpriteSelected,
            TrackSelected,
            CopiedTracksPopulated,
            FloatVarPopulated,
            ColorVarPopulated
        };

        enum ButtonNames
        {
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
            ExpandAndTransposeToPlayhead,
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
            RefreshLayout,
            CopyTracks,
            PasteTracks,
            TMProColorTrack,
            RectTransformPosTrack,
            SpriteColorTrack,
            FloatVarTrack,
            GroupTrack,
            NewClips
        };

        enum UpdateWindowTriggers
        {
            RootWindow,
            AllowBlankTracks,
            TargetFloatVariable
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
            UpdateElementStructure();
            Undo.undoRedoPerformed += undoCallback;
            Selection.selectionChanged += UpdateElementStructure;
        }

        void OnDestroy()
        {
            Selection.selectionChanged -= UpdateElementStructure;
        }

        void UpdateElementStructure()
        {
            if(allowBlankTracks == true) {

                ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);
                ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);

            } else {

                if (TargetComponentSelected(Selection.gameObjects, typeof(TMP_Text))) {
                    ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.TextSelected, false);
                }

                if (TargetComponentSelected(Selection.gameObjects, typeof(RectTransform))) {
                    ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, false);
                }

                if (TargetComponentSelected(Selection.gameObjects, typeof(SpriteRenderer))) {
                    ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, false);
                }

                if(targetFloat != null) {
                    ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, false);
                }

                if (TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                    ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.TrackSelected, false);
                }

                if(copiedTracks.Count > 0) {
                    ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, false);
                }
            }
        }

        Dictionary<EnableCondition, List<VisualElement>> AddToToggleData(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, VisualElement elementToAdd)
        {
            if(targetCollection.ContainsKey(targetCondition)) {
                targetCollection[targetCondition].Add(elementToAdd);
            } else {
                List<VisualElement> buttonList = new List<VisualElement>();
                buttonList.Add(elementToAdd);
                targetCollection.Add(targetCondition, buttonList);
            }

            return targetCollection;
        }

        Dictionary<EnableCondition, List<VisualElement>> ToggleVisualElements(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, bool targetStatus = false)
        {
            if(targetCollection.ContainsKey(targetCondition)) {
                List<VisualElement> buttonList = targetCollection[targetCondition];
                for (int i=0; i<buttonList.Count; i++) {
                    buttonList[i].SetEnabled(targetStatus);        
                }
            }
            return targetCollection;
        }

        bool TargetComponentSelected(GameObject[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                if (currentSelection[i].GetComponent(targetType) != null) {
                    return true;
                }
            }
            return false;
        }

        bool TargetTypeSelected(UnityEngine.Object[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                Type currentType = currentSelection[i].GetType();
                if (currentType.IsSubclassOf(targetType) || currentType == targetType) {
                    return true;
                }
            }
            return false;
        }

        void RenderLayout()
        {
            rootVisualElement.Clear();
            toggleData.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilities_Style.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var timelineUtilitiesTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilitiesWindow.uxml");
            timelineUtilitiesStructure = timelineUtilitiesTree.CloneTree();
            rootVisualElement.Add(timelineUtilitiesStructure);

            clipSelectionManipulation = rootVisualElement.Query("clip-selection-manipulation");
            trackClipCreation = rootVisualElement.Query("track-clip-creation");

            editorWindowSerialized = new SerializedObject(this);
            rootVisualElement.Bind(editorWindowSerialized);

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            var updateWindowTriggers = rootVisualElement.Query<VisualElement>(null, "update-window-trigger");
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);
        }

        void RefreshStyles()
        {
            rootVisualElement.styleSheets.Clear();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilities_Style.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.SelectEndingBefore):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SelectEndingBefore(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingBefore):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SelectStartingBefore(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectEndingAfter):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SelectEndingAfter(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingAfter):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SelectStartingAfter(Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.AddPrevClipToSelection):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.AddPrevClipToSelection(TimelineEditor.selectedClips, currentTime, selectionCount);
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
                        TimelineEditor.selectedClips = ClipSelectionManipulation.AddNextClipToSelection(TimelineEditor.selectedClips, currentTime, selectionCount);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetToPlayhead(ClipSelectionManipulation.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.TransposeToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.TransposeToPlayhead(ClipSelectionManipulation.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.ExpandToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.ExpandToPlayhead(ClipSelectionManipulation.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.ExpandAndTransposeToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.ExpandAndTransposeToPlayhead(ClipSelectionManipulation.GetCurrentClipSelection(), currentTime, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips());
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.MultiplyDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.MultiplyDuration(ClipSelectionManipulation.GetCurrentClipSelection(), durationMultiplier, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.MultiplyDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.MultiplyDurationAndTranspose(ClipSelectionManipulation.GetCurrentClipSelection(), durationMultiplier, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips());
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetDuration(ClipSelectionManipulation.GetCurrentClipSelection(), targetDuration, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetDurationAndTranspose(ClipSelectionManipulation.GetCurrentClipSelection(), targetDuration, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetSpacing(ClipSelectionManipulation.GetCurrentClipSelection(), targetSpacing, callTransposeUnselectedClips, ClipSelectionManipulation.GetAllTimelineClips(), ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.AddSubtractSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.AddSubtractSpacing(ClipSelectionManipulation.GetCurrentClipSelection(), ClipSelectionManipulation.GetAllTimelineClips(), targetSpacing);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSequentialOrder):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetSequentialOrder(ClipSelectionManipulation.GetCurrentClipSelection(), ClipSelectionManipulation.GetAllTimelineClips(), callTransposeUnselectedClips, ClipSelectionManipulation.TransposeTargetClips);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetSequentialOrderReverse):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ClipSelectionManipulation.SetSequentialOrderReverse(ClipSelectionManipulation.GetCurrentClipSelection(), ClipSelectionManipulation.GetAllTimelineClips(), callTransposeUnselectedClips, ClipSelectionManipulation.TransposeTargetClips);
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
                        ClipSelectionManipulation.DeselectAll();
                    };
                    break;

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        RenderLayout();
                    };
                    break;

                case nameof(ButtonNames.CopyTracks):
                    button.clickable.clicked += () => {
                        copiedTracks = TrackClipCreation.CopyTracks(TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsModified();
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;

                case nameof(ButtonNames.PasteTracks):
                    button.clickable.clicked += () => {
                        TrackClipCreation.PasteTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects, copiedTracks);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.CopiedTracksPopulated, button);
                    break;

                case nameof(ButtonNames.TMProColorTrack):
                    button.clickable.clicked += () => {
                        Selection.objects = TrackClipCreation.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProColorTrack), Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformPosTrack):
                    button.clickable.clicked += () => {
                        Selection.objects = TrackClipCreation.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.SpriteColorTrack):
                    button.clickable.clicked += () => {
                        Selection.objects = TrackClipCreation.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SpriteColorTrack), Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;

                case nameof(ButtonNames.FloatVarTrack):
                    button.clickable.clicked += () => {
                        Selection.objects = TrackClipCreation.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetFloat }, typeof(LerpFloatVarTrack), Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.FloatVarPopulated, button);
                    break;

                case nameof(ButtonNames.GroupTrack):
                    button.clickable.clicked += () => {
                        Selection.activeObject = TrackClipCreation.TriggerCreateGroupTrack(TimelineEditor.inspectedAsset, Selection.objects);
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;

                case nameof(ButtonNames.NewClips):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = TrackClipCreation.CreateClips(TimelineEditor.inspectedDirector, Selection.objects, newClipDuration).ToArray();
                        TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;
            }

            return button;
        }

        IEnumerator CallRenderLayout(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            RenderLayout();
        }

        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.AllowBlankTracks):
                    visualElement.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) => {
                        UpdateElementStructure();
                    });
                    break;

                case nameof(UpdateWindowTriggers.TargetFloatVariable):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateElementStructure();
                    });
                    break;

            }
            
            return visualElement;
        }

            void OnInspectorUpdate()
        {
            if (debugTrackCreated == true) {
                currentTime = TimelineUtilitiesCore.CurrentTime;
                if (TimelineEditor.inspectedDirector != null) {
                    TimelineEditor.inspectedDirector.time = currentTime;
                    TimelineEditor.inspectedDirector.DeferredEvaluate();
                }
            }
        }

        public void OnGUI()
        {

            GUIStyle guiStyle = new GUIStyle("Label");
            guiStyle.fontStyle = FontStyle.Italic;
            guiStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Space(10);

            if (TimelineEditor.inspectedAsset == null) {
                GUILayout.Label("Please select a timeline asset in order to use clip tools.", guiStyle);
                timelineUtilitiesStructure.visible = false;
            } else {
                debugTrackCreated = FindDebugTrack();

                if (debugTrackCreated == false) {
                    timelineUtilitiesStructure.visible = false;

                    GUILayout.Label("You must create a debug track in order to use timeline utilities.", guiStyle);
                    GUILayout.Space(10);

                    if (GUILayout.Button("Create Debug Track")) {
                        CreateDebugTrack();
                    }

                } else {
                    timelineUtilitiesStructure.visible = true;
                }
            }

        }

        static bool FindDebugTrack()
        {
            TimelineAsset currentAsset = TimelineEditor.inspectedAsset;
            IEnumerable<PlayableBinding> playableBindings = currentAsset.outputs;
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