using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{

    public class TimelineUtilsWindow : EditorWindow
    {
        VisualElement timelineUtilsStructure;
        PlayableDirector currentDirector;

        Undo.UndoRedoCallback undoCallback = new Undo.UndoRedoCallback(TimelineUtilsCore.RefreshTimelineWindow);

        public delegate void InspectorUpdateDelegate();
        public static InspectorUpdateDelegate inspectorUpdateDelegate = () => {};

        public delegate void SelectionChangedDelegate();
        public static SelectionChangedDelegate selectionChangedDelegate = () => {};

        Dictionary<Type, string> childWindowData = new Dictionary<Type, string> {
            { typeof(ClipSelectionManipulation), "clip-selection-manipulation" },
            { typeof(TrackClipCreation), "track-clip-creation" }
        };

        static Dictionary<Type, ChildUIElementsWindow> childWindows = new Dictionary<Type, ChildUIElementsWindow>();

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
            titleContent = new GUIContent("Timeline Utils");
            Undo.undoRedoPerformed += undoCallback;
            Selection.selectionChanged += SelectionChangedCallback;
            EditorSceneManager.activeSceneChangedInEditMode += SceneChangedCallback;
            
            currentDirector = TimelineEditor.inspectedDirector;
            //currentDirector.paused += OnPlayableDirectorPaused;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= undoCallback;
            Selection.selectionChanged -= SelectionChangedCallback;
            EditorSceneManager.activeSceneChangedInEditMode -= SceneChangedCallback;

            //currentDirector.paused -= OnPlayableDirectorPaused;
        }

        void OnInspectorUpdate()
        {    
            inspectorUpdateDelegate.Invoke();
        }

        void SelectionChangedCallback()
        {
            //if(currentDirector != TimelineEditor.inspectedDirector) {
            //    currentDirector.paused -= OnPlayableDirectorPaused;
            //    currentDirector = TimelineEditor.inspectedDirector;
            //    currentDirector.paused += OnPlayableDirectorPaused;
            //}
            selectionChangedDelegate.Invoke();
        }

        void SceneChangedCallback(Scene scene, Scene newScene)
        {
            rootVisualElement.Clear();
            timelineUtilsStructure = null;
        }

        void OnPlayableDirectorPaused(PlayableDirector playableDirector)
        {
            playableDirector.time = TimelineUtilsCore.CurrentTime;
            playableDirector.Evaluate();
            TimelineUtilsCore.RefreshTimelineWindow();
        }

        void RenderLayout()
        {
            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var timelineUtilitiesTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/Maestro/Editor/TimelineUtils/TimelineUtilsWindow_UXML.uxml");
            timelineUtilsStructure = timelineUtilitiesTree.CloneTree();
            rootVisualElement.Add(timelineUtilsStructure);

            foreach (KeyValuePair<Type, ChildUIElementsWindow> childWindow in childWindows) {
                DestroyImmediate(childWindow.Value);
            }
            childWindows.Clear();
            foreach (KeyValuePair<Type, string> childWindow in childWindowData) {
                if(childWindows.ContainsKey(childWindow.Key)) {
                    childWindows[childWindow.Key] = EditorToolsCore.CreateAndBindChildWindow(childWindow.Key, this, childWindow.Value);
                } else {
                    childWindows.Add(childWindow.Key, EditorToolsCore.CreateAndBindChildWindow(childWindow.Key, this, childWindow.Value));
                }
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
                if(timelineUtilsStructure != null) {
                    timelineUtilsStructure.visible = false;
                }
            } else {

                if (TimelineUtilsCore.DebugTrackCreated == false) {
                    if (timelineUtilsStructure != null) {
                        timelineUtilsStructure.visible = false;
                    }

                    GUILayout.Label("You must create a debug track in order to use timeline utilities.", guiStyle);
                    GUILayout.Space(10);

                    if (GUILayout.Button("Create Debug Track")) {
                        TimelineUtilsCore.CreateDebugTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector);
                        RenderLayout();
                    }

                } else {
                    if(timelineUtilsStructure != null) {
                        timelineUtilsStructure.visible = true;
                    } else {
                        RenderLayout();
                    }
                }
            }

        }
    }
}