using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace AltSalt
{

    public class TimelineUtilsWindow : EditorWindow
    {
        VisualElement timelineUtilsStructure;

        Undo.UndoRedoCallback undoCallback = new Undo.UndoRedoCallback(TimelineUtilsCore.RefreshTimelineWindow);

        public delegate void InspectorUpdateDelegate();
        public InspectorUpdateDelegate inspectorUpdateDelegate;

        public delegate void SelectionChangedDelegate();
        public SelectionChangedDelegate selectionChangedDelegate;

        Dictionary<Type, string> childWindowData = new Dictionary<Type, string> {
            { typeof(ClipSelectionManipulation), "clip-selection-manipulation" },
            { typeof(TrackClipCreation), "track-clip-creation" }
        };

        static List<ChildUIElementsWindow> childWindows = new List<ChildUIElementsWindow>();

        enum ButtonNames
        {
            RefreshLayout
        }

        [MenuItem("Tools/AltSalt/Timeline Utilities")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimelineUtilsWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(TimelineUtilsWindow)).Show();
        }

        void OnEnable()
        {
            RenderLayout();
            Undo.undoRedoPerformed += undoCallback;
            Selection.selectionChanged += SelectionChangedCallback;
        }

        void OnDestroy()
        {
            Undo.undoRedoPerformed -= undoCallback;
            Selection.selectionChanged -= SelectionChangedCallback;
        }

        void OnInspectorUpdate()
        {
            inspectorUpdateDelegate.Invoke();
        }

        void SelectionChangedCallback()
        {
            selectionChangedDelegate.Invoke();
        }

        void RenderLayout()
        {
            foreach(ChildUIElementsWindow childWindow in childWindows) {
                DestroyImmediate(childWindow);
            }
            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var timelineUtilitiesTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/Editor/TimelineUtils/TimelineUtilsWindow_UXML.uxml");
            timelineUtilsStructure = timelineUtilitiesTree.CloneTree();
            rootVisualElement.Add(timelineUtilsStructure);

            foreach (KeyValuePair<Type, string> childWindow in childWindowData) {
                childWindows.Add(EditorToolsCore.CreateAndBindChildWindow(childWindow.Key, this, childWindow.Value));
            }

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);
        }


        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        RenderLayout();
                    };
                    break;
            }

            return button;
        }

        public void OnGUI()
        {

            GUIStyle guiStyle = new GUIStyle("Label");
            guiStyle.fontStyle = FontStyle.Italic;
            guiStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Space(10);

            if (TimelineEditor.inspectedAsset == null) {
                GUILayout.Label("Please select a timeline asset in order to use clip tools.", guiStyle);
                timelineUtilsStructure.visible = false;
            } else {

                if (TimelineUtilsCore.DebugTrackCreated == false) {
                    timelineUtilsStructure.visible = false;

                    GUILayout.Label("You must create a debug track in order to use timeline utilities.", guiStyle);
                    GUILayout.Space(10);

                    if (GUILayout.Button("Create Debug Track")) {
                        TimelineUtilsCore.CreateDebugTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector);
                        RenderLayout();
                    }

                } else {
                    timelineUtilsStructure.visible = true;
                }
            }

        }
    }
}