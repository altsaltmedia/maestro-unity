using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{

    public class TimelineUtilsWindow : ParentModuleWindow
    {
        PlayableDirector currentDirector;

        Undo.UndoRedoCallback undoCallback = new Undo.UndoRedoCallback(TimelineUtils.RefreshTimelineWindow);

//        public delegate void InspectorUpdateDelegate();
//        public static InspectorUpdateDelegate inspectorUpdateDelegate = () => {};
//
//        public delegate void SelectionChangedDelegate();
//        public static SelectionChangedDelegate selectionChangedDelegate = () => {};

        private Dictionary<Type, string> _childWindowData = new Dictionary<Type, string> {
            { typeof(TimelineAssetManipulation), "clip-selection-manipulation" },
            { typeof(TrackPlacement), "track-clip-creation" }
        };

        protected override Dictionary<Type, string> childWindowData
        {
            get => _childWindowData;
            set => _childWindowData = value;
        }
        
        enum ButtonNames
        {
            RefreshLayout
        }

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            return this;
        }
        

        void OnEnable()
        {
//            titleContent = new GUIContent("Timeline Utils");
            Undo.undoRedoPerformed += undoCallback;
//            Selection.selectionChanged += SelectionChangedCallback;
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

//        void OnInspectorUpdate()
//        {    
//            inspectorUpdateDelegate.Invoke();
//        }

        void SelectionChangedCallback()
        {
            //if(currentDirector != TimelineEditor.inspectedDirector) {
            //    currentDirector.paused -= OnPlayableDirectorPaused;
            //    currentDirector = TimelineEditor.inspectedDirector;
            //    currentDirector.paused += OnPlayableDirectorPaused;
            //}
            //selectionChangedDelegate.Invoke();
        }

        void SceneChangedCallback(Scene scene, Scene newScene)
        {
//            rootVisualElement.Clear();
            moduleWindowUXML = null;
        }

        void OnPlayableDirectorPaused(PlayableDirector playableDirector)
        {
            playableDirector.time = TimelineUtils.currentTime;
            playableDirector.Evaluate();
            TimelineUtils.RefreshTimelineWindow();
        }

//        void RenderLayout()
//        {
//            rootVisualElement.Clear();
//            AssetDatabase.Refresh();
//
//            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
//            rootVisualElement.styleSheets.Add(styleSheet);
//
//            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
//            var timelineUtilitiesTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/Maestro/Editor/TimelineUtils/TimelineUtilsWindow_UXML.uxml");
//            pageBuilderChildUXML = timelineUtilitiesTree.CloneTree();
//            rootVisualElement.Add(pageBuilderChildUXML);
//
//            foreach (KeyValuePair<Type, PageBuilderChildWindow> childWindow in childWindows) {
//                DestroyImmediate(childWindow.Value);
//            }
//            childWindows.Clear();
//            foreach (KeyValuePair<Type, string> childWindow in childWindowData) {
//                if(childWindows.ContainsKey(childWindow.Key)) {
//                    childWindows[childWindow.Key] = ModuleRootWindow.CreateModuleRootWindow(childWindow.Key, this, childWindow.Value);
//                } else {
//                    childWindows.Add(childWindow.Key, ModuleRootWindow.CreateModuleRootWindow(childWindow.Key, this, childWindow.Value));
//                }
//            }
//
//            var buttons = rootVisualElement.Query<Button>();
//            buttons.ForEach(SetupButton);
//        }
//
//
//        Button SetupButton(Button button)
//        {
//            switch (button.name) {
//
//                case nameof(ButtonNames.RefreshLayout):
//                    button.clickable.clicked += () => {
//                        RenderLayout();
//                    };
//                    break;
//            }
//
//            return button;
//        }

        public void OnGUI()
        {

            GUIStyle guiStyle = new GUIStyle("Label");
            guiStyle.fontStyle = FontStyle.Italic;
            guiStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Space(10);

            if (TimelineEditor.inspectedAsset == null) {
                GUILayout.Label("Please select a timeline asset in order to use clip tools.", guiStyle);
                if(moduleWindowUXML != null) {
                    moduleWindowUXML.visible = false;
                }
            } else {

                if (TimelineUtils.debugTrackCreated == false) {
                    if (moduleWindowUXML != null) {
                        moduleWindowUXML.visible = false;
                    }

                    GUILayout.Label("You must create a debug track in order to use timeline utilities.", guiStyle);
                    GUILayout.Space(10);

                    if (GUILayout.Button("Create Debug Track")) {
                        TimelineUtils.CreateDebugTrack(TimelineEditor.inspectedAsset);
                        controlPanel.RenderLayout();
                    }

                } else {
                    if(moduleWindowUXML != null) {
                        moduleWindowUXML.visible = true;
                    } else {
                        controlPanel.RenderLayout();
                    }
                }
            }

        }
    }
}