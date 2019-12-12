﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    public class ResponsiveUtils : ModuleWindow
    {
        public float breakpoint = 1.78f;
        public bool listViewExpanded = true;

        private VisualElement _listContainer;

        private VisualElement listContainer
        {
            get => _listContainer;
            set => _listContainer = value;
        }

        private ListView _listView;

        private ListView listView
        {
            get => _listView;
            set => _listView = value;
        }

        private List<IResponsive> _selectedElements = new List<IResponsive>();

        private List<IResponsive> selectedElements
        {
            get => _selectedElements;
            set => _selectedElements = value;
        }

        private SimpleEventTrigger _screenResized = new SimpleEventTrigger();

        private SimpleEventTrigger screenResized
        {
            get => _screenResized;
            set => _screenResized = value;
        }

        [MenuItem("Tools/Maestro/Responsive Utils")]
        public static void ShowWindow()
        {
            var moduleWindow = CreateInstance<ResponsiveUtils>();
            moduleWindow.Init();
            moduleWindow.Show();
        }

        private void Init()
        {
            titleContent = new GUIContent("Responsive Utils");
            ModuleWindow moduleWindow = Configure(null, ProjectNamespaceData.namespaceData[ModuleNamespace.Root].editorPath + nameof(ResponsiveUtils)+"_UXML.uxml");
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.AddToClassList("altsalt");
            rootVisualElement.styleSheets.Add(styleSheet);
            
            rootVisualElement.Add(moduleWindow.moduleWindowUXML);
        }

        private void OnEnable()
        {
            Init();
            Selection.selectionChanged += CallUpdateListView;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= CallUpdateListView;
        }
        
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            listContainer = moduleWindowUXML.Query("ResponsiveElementListContainer");
            screenResized.SimpleEventTarget = Utils.GetSimpleEvent(nameof(VarDependencies.ScreenResized));
            
            UpdateListView(this);
            
            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            return this;
        }

        private void OnHierarchyChange()
        {
            UpdateListView(this);
        }

        private enum ButtonNames
        {
            AddBreakpoint,
            SaveResponsiveValues,
            TriggerScreenResized,
            ToggleListView
        }

        private Button SetupButton(Button button)
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
                        UpdateListView(this);
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
                        ModuleUtils.ToggleListView(listView, listViewExpanded);
                    };
                    break;
            }

            return button;
        }

        private void CallUpdateListView()
        {
            UpdateListView(this);
        }

        private static VisualElement UpdateListView(ResponsiveUtils responsiveUtils)
        {
            if (responsiveUtils.listContainer == null) {
                return responsiveUtils.listContainer;
            }
            
            responsiveUtils.listContainer.Clear();

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
                responsiveUtils.listContainer.Add(breakpointLabel);

                responsiveUtils.listView = CreateListView(responsiveUtils, responsiveElements, responsiveUtils.listViewExpanded);
                responsiveUtils.listContainer.Add(responsiveUtils.listView);

                responsiveUtils.rootVisualElement.RegisterCallback<MouseCaptureOutEvent>((MouseCaptureOutEvent evt) => {
                    if (evt.target != responsiveUtils.listView) {
                        responsiveUtils.selectedElements.Clear();
                    }
                });

            } else {
                Label label = new Label("No responsive elements selected");
                responsiveUtils.listContainer.Add(label);
            }

            return responsiveUtils.listContainer;
        }

        private static Label CreateBreakpointLabel(List<IResponsive> list)
        {
            List<float> breakpointValues = new List<float>();
            for (int q = 0; q < list.Count; q++) {
                for (int z = 0; z < list[q].aspectRatioBreakpoints.Count; z++) {
                    float elementBreakpoint = list[q].aspectRatioBreakpoints[z];
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

        private static ListView CreateListView(ResponsiveUtils responsiveUtils, List<IResponsive> listElements, bool expandListView)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = (labelText, index) =>
            {
                string objectString = listElements[index].elementName.LimitLength(10);

                string aspectRatioNames = string.Format("({0})", string.Join(", ", listElements[index].aspectRatioBreakpoints.ToArray()));

                (labelText as Label).text = objectString + " - " + listElements[index].GetType().Name + " " + aspectRatioNames;
            };

            const int itemHeight = 16;

            var listView = new ListView(listElements, itemHeight, makeItem, bindItem)
            {
                selectionType = SelectionType.Multiple
            };

            listView.onItemChosen += item => {
                
                if (item is ResponsiveLerpToTargetBehaviour behaviour) {
                    
                    var clipAsset = ResponsiveUtilsCore.GetClipAssetFromResponsiveBehaviour(behaviour);
                    var parentTrack = ResponsiveUtilsCore.GetParentTrackFromResponsiveBehaviour(behaviour);
                    TimelineEditor.selectedClip = TimelineUtils.GetTimelineClipFromTrackAsset(clipAsset, parentTrack);
                    
                } else if(item is UnityEngine.Object unityObject) {
                    Selection.activeObject = unityObject;
                }
                
            };
            listView.onSelectionChanged += items => {
                responsiveUtils.selectedElements = items.ConvertAll(item => (IResponsive)item);
            };

            return ModuleUtils.ToggleListView(listView, expandListView);
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
            TimelineUtils.RefreshTimelineContentsModified();
            return responsiveClipAssetList.ToArray();
        }

        public static IResponsive[] AddBreakpointToSelectedElements(IResponsive[] responsiveElements, float targetBreakpoint)
        {            
            for (int q = 0; q < responsiveElements.Length; q++) {
                if(responsiveElements[q] is IResponsive) {
                    responsiveElements[q].AddBreakpoint(targetBreakpoint);
                } else if(responsiveElements[q] is ResponsiveLerpToTargetBehaviour) {
                    responsiveElements[q].AddBreakpoint(targetBreakpoint);
                }
            }
            return responsiveElements;
        }
    }
}