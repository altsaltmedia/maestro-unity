using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace AltSalt
{
    public class ResponsiveUtilsWindow : EditorWindow
    {
        public float breakpoint = 1.78f;

        static VisualElement responsiveElementListContainer;
        ListView responsiveElementListView;
        List<IResponsive> selectedElements = new List<IResponsive>();
        SimpleEventTrigger screenResized = new SimpleEventTrigger();

        bool listViewExpanded;

        [MenuItem("Tools/AltSalt/Responsive Utils")]
        public static void ShowWindow()
        {
            var window = GetWindow<ResponsiveUtilsWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(ResponsiveUtilsWindow)).Show();
        }

        void OnEnable()
        {
            titleContent = new GUIContent("Responsive Utils");
            RenderLayout();
            screenResized.SimpleEventTarget = Utils.GetSimpleEvent("ScreenResized");
            Selection.selectionChanged += UpdateResponsiveElementList;
            
        }

        void OnDisable()
        {
            Selection.selectionChanged -= UpdateResponsiveElementList;
        }

        void OnHierarchyChange()
        {
            UpdateResponsiveElementList();
        }

        enum ButtonNames
        {
            AddBreakpoint,
            SaveResponsiveValues,
            TriggerScreenResized,
            ToggleListView,
            RefreshLayout
        }

        void RenderLayout()
        {
            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.styleSheets.Add(styleSheet);

            var pageBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/Editor/ResponsiveUtils/ResponsiveUtilsWindow_UXML.uxml");
            VisualElement pageBuilderStructure = pageBuilderTree.CloneTree();
            rootVisualElement.Add(pageBuilderStructure);

            responsiveElementListContainer = rootVisualElement.Query("ResponsiveElementListContainer");

            SerializedObject serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(serializedObject);

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.AddBreakpoint):
                    button.clickable.clicked += () => {
                        if (selectedElements.Count > 0) {
                            AddBreakpointToSelectedElements(selectedElements.ToArray(), breakpoint);
                        } else {
                            AddBreakpointToSelectedElements(Selection.gameObjects, breakpoint);
                            AddBreakpointToSelectedElements(TimelineEditor.selectedClips, breakpoint);
                        }
                        UpdateResponsiveElementList();
                    };
                    break;

                case nameof(ButtonNames.SaveResponsiveValues):
                    button.clickable.clicked += () => {
                        SaveResponsiveValues(Selection.gameObjects);
                        SaveResponsiveValues(TimelineEditor.selectedClips);
                    };
                    break;

                case nameof(ButtonNames.TriggerScreenResized):
                    button.clickable.clicked += () => {
                        screenResized.RaiseEvent(this, "editor window", "screen resized button");
                    };
                    break;

                case nameof(ButtonNames.ToggleListView):
                    button.clickable.clicked += () => {
                        listViewExpanded = !listViewExpanded;
                        ToggleListView(responsiveElementListView, listViewExpanded);
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

        void UpdateResponsiveElementList()
        {
            responsiveElementListContainer.Clear();

            List<IResponsive> responsiveElements = new List<IResponsive>();

            for(int i=0; i<Selection.gameObjects.Length; i++) {
                responsiveElements.AddRange(Selection.gameObjects[i].GetComponents<IResponsive>());
            }

            for(int i=0; i<TimelineEditor.selectedClips.Length; i++) {
                var clipAsset = TimelineEditor.selectedClips[i].asset;
                if (clipAsset is ResponsiveLerpToTargetClip) {
                    responsiveElements.Add(ResponsiveUtilsCore.GetResponsiveElementFromClipAsset(clipAsset as ResponsiveLerpToTargetClip));
                }
            }

            if(responsiveElements.Count > 0) {

                Label breakpointLabel = CreateBreakpointLabel(responsiveElements);
                responsiveElementListContainer.Add(breakpointLabel);

                responsiveElementListView = CreateResponsiveElementListView(responsiveElements, listViewExpanded);
                responsiveElementListContainer.Add(responsiveElementListView);

                rootVisualElement.RegisterCallback<MouseCaptureOutEvent>((MouseCaptureOutEvent evt) => {
                    if (evt.target != responsiveElementListView) {
                        selectedElements.Clear();
                    }
                });

            } else {
                Label label = new Label("No responsive elements selected");
                responsiveElementListContainer.Add(label);
            }
        }

        static Label CreateBreakpointLabel(List<IResponsive> list)
        {
            List<float> breakpointValues = new List<float>();
            for (int q = 0; q < list.Count; q++) {
                for (int z = 0; z < list[q].AspectRatioBreakpoints.Count; z++) {
                    float elementBreakpoint = list[q].AspectRatioBreakpoints[z];
                    if (breakpointValues.Contains(elementBreakpoint) == false) {
                        breakpointValues.Add(elementBreakpoint);
                    }
                }
            }
            breakpointValues.Sort();

            string breakpointString;

            if (breakpointValues.Count > 0) {
                breakpointString = string.Join(", ", breakpointValues.ToArray());
            } else {
                breakpointString = "none";
            }

            Label label = new Label("Active breakpoints : " + breakpointString);
            return label;
        }

        ListView CreateResponsiveElementListView(List<IResponsive> list, bool expandListView)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = (e, i) => {
                string objectString;
                if (list[i].Name.Length > 10) {
                    objectString = list[i].Name.Substring(0, 10);
                } else {
                    objectString = list[i].Name;
                }
                string aspectRatioNames = string.Format("({0})", string.Join(", ", list[i].AspectRatioBreakpoints.ToArray()));

                (e as Label).text = objectString + " - " + list[i].GetType().Name + " " + aspectRatioNames;
            };

            const int itemHeight = 16;

            var listView = new ListView(list, itemHeight, makeItem, bindItem);
            listView.selectionType = SelectionType.Multiple;

            listView.onItemChosen += obj => {
                if (obj is ResponsiveLerpToTargetBehaviour) {
                    var clipAsset = ResponsiveUtilsCore.GetClipAssetFromResponsiveBehaviour(obj as ResponsiveLerpToTargetBehaviour);
                    var parentTrack = ResponsiveUtilsCore.GetParentTrackFromResponsiveBehaviour(obj as ResponsiveLerpToTargetBehaviour);
                    TimelineEditor.selectedClip = TimelineUtilsCore.GetTimelineClipFromTrackAsset(clipAsset, parentTrack);
                } else if(obj is UnityEngine.Object) {
                    Selection.activeObject = obj as UnityEngine.Object;
                } 
            };
            listView.onSelectionChanged += objects => {
                selectedElements = objects.ConvertAll(item => (IResponsive)item);
            };

            return ToggleListView(listView, expandListView);
        }

        public static IResponsiveSaveable[] SaveResponsiveValues(GameObject[] selectedObjects)
        {
            List<IResponsiveSaveable> saveableList = new List<IResponsiveSaveable>();

            for (int i = 0; i < selectedObjects.Length; i++) {

                IResponsiveSaveable[] objectComponents = selectedObjects[i].GetComponents<IResponsiveSaveable>();
                for (int q = 0; q < objectComponents.Length; q++) {
                    objectComponents[q].SaveValue();
                }
                saveableList.AddRange(objectComponents);
                
            }
            return saveableList.ToArray();
        }

        public static IResponsiveSaveable[] SaveResponsiveValues(TimelineClip[] selectedClips)
        {
            List<IResponsiveSaveable> saveableList = new List<IResponsiveSaveable>();

            for (int i = 0; i < selectedClips.Length; i++) {

                if (selectedClips[i].asset is IResponsiveSaveable) {
                    IResponsiveSaveable saveableElement = selectedClips[i] as IResponsiveSaveable;
                    saveableElement.SaveValue();
                    saveableList.Add(saveableElement);
                }
            }
            return saveableList.ToArray();
        }

        static ListView ToggleListView(ListView targetListView, bool expandListView)
        {
            if (targetListView != null) {
                if (expandListView == true) {
                    targetListView.RemoveFromClassList("expanded");
                } else {
                    targetListView.AddToClassList("expanded");
                }
            }
            return targetListView;
        }

        public static IResponsive[] AddBreakpointToSelectedElements(GameObject[] selectedObjects, float targetBreakpoint)
        {
            List<IResponsive> responsiveComponentList = new List<IResponsive>();

            for (int i = 0; i < selectedObjects.Length; i++) {
                IResponsive[] responsiveComponents = selectedObjects[i].GetComponents<IResponsive>();
                for (int q = 0; q < responsiveComponents.Length; q++) {
                    responsiveComponents[q].AddBreakpoint(targetBreakpoint);
                }
                responsiveComponentList.AddRange(responsiveComponents);                
            }
            return responsiveComponentList.ToArray();
        }

        public static IResponsive[] AddBreakpointToSelectedElements(TimelineClip[] selectedClips, float targetBreakpoint)
        {
            List<IResponsive> responsiveClipAssetList = new List<IResponsive>();

            for (int i = 0; i < selectedClips.Length; i++) {
                if (selectedClips[i].asset is ResponsiveLerpToTargetClip) {
                    IResponsive responsiveElement = ResponsiveUtilsCore.GetResponsiveElementFromClipAsset(selectedClips[i].asset as ResponsiveLerpToTargetClip);
                    responsiveElement.AddBreakpoint(targetBreakpoint);
                    responsiveClipAssetList.Add(responsiveElement);
                }
            }
            TimelineUtilsCore.RefreshTimelineContentsModified();
            return responsiveClipAssetList.ToArray();
        }

        public static IResponsive[] AddBreakpointToSelectedElements(IResponsive[] responsiveElements, float targetBreakpoint)
        {            
            for (int q = 0; q < responsiveElements.Length; q++) {
                if(responsiveElements[q] is ResponsiveElement) {
                    responsiveElements[q].AddBreakpoint(targetBreakpoint);
                } else if(responsiveElements[q] is ResponsiveLerpToTargetBehaviour) {
                    responsiveElements[q].AddBreakpoint(targetBreakpoint);
                }
            }
            return responsiveElements;
        }
    }
}
