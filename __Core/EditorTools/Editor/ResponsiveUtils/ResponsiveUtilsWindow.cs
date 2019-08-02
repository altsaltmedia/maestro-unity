using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt
{
    public class ResponsiveUtilsWindow : EditorWindow
    {
        public float breakpoint = 1.78f;

        static VisualElement responsiveElementListContainer;
        ListView responsiveElementListView;
        List<ResponsiveElement> selectedElements;
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
                        if(selectedElements.Count > 0) {
                            AddBreakpointToSelectedElements(selectedElements.ToArray(), breakpoint);
                        } else {
                            AddBreakpointToSelectedElements(Selection.gameObjects, breakpoint);
                        }
                        UpdateResponsiveElementList();
                    };
                    break;

                case nameof(ButtonNames.SaveResponsiveValues):
                    button.clickable.clicked += () => {
                        SaveResponsiveValues(Selection.gameObjects);
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

            List<ResponsiveElement> responsiveElements = new List<ResponsiveElement>();

            for(int i=0; i<Selection.gameObjects.Length; i++) {
                responsiveElements.AddRange(Selection.gameObjects[i].GetComponents<ResponsiveElement>());
            }

            if(responsiveElements.Count > 0) {

                Label breakpointLabel = CreateBreakpointLabel(responsiveElements);
                responsiveElementListContainer.Add(breakpointLabel);

                responsiveElementListView = CreateResponsiveElementListView(responsiveElements, listViewExpanded);
                responsiveElementListContainer.Add(responsiveElementListView);

            } else {
                Label label = new Label("No responsive elements selected");
                responsiveElementListContainer.Add(label);
            }
        }

        static Label CreateBreakpointLabel(List<ResponsiveElement> list)
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

        ListView CreateResponsiveElementListView(List<ResponsiveElement> list, bool expandListView)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = (e, i) => {
                string objectString;
                if (list[i].name.Length > 10) {
                    objectString = list[i].name.Substring(0, 10);
                } else {
                    objectString = list[i].name;
                }
                string aspectRatioNames = string.Format("({0})", string.Join(", ", list[i].AspectRatioBreakpoints.ToArray()));

                (e as Label).text = objectString + " - " + list[i].GetType().Name + " " + aspectRatioNames;
            };

            const int itemHeight = 16;

            var listView = new ListView(list, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Multiple;

            listView.onItemChosen += obj => Selection.activeObject = obj as UnityEngine.Object;
            listView.onSelectionChanged += objects => {
                selectedElements = objects.ConvertAll(item => (ResponsiveElement)item);
            };
            listView.RegisterCallback<MouseCaptureOutEvent>((MouseCaptureOutEvent evt) => {
                if (evt.target != responsiveElementListView) {
                    selectedElements.Clear();
                }
            });

            return ToggleListView(listView, expandListView);
        }

        public static IResponsiveSaveable[] SaveResponsiveValues(GameObject[] gameObjects)
        {
            List<IResponsiveSaveable> componentList = new List<IResponsiveSaveable>();

            for (int i = 0; i < gameObjects.Length; i++) {

                IResponsiveSaveable[] objectComponents = gameObjects[i].GetComponents<IResponsiveSaveable>();
                for (int q = 0; q < objectComponents.Length; q++) {
                    objectComponents[q].SaveValue();
                }

                componentList.AddRange(objectComponents);
            }
            return componentList.ToArray();
        }

        public static ResponsiveElement[] ExecuteResponsiveAction(GameObject[] gameObjects)
        {
            List<ResponsiveElement> componentList = new List<ResponsiveElement>();

            for (int i = 0; i < gameObjects.Length; i++) {
                ResponsiveElement[] objectComponents = gameObjects[i].GetComponents<ResponsiveElement>();
                for (int q = 0; q < objectComponents.Length; q++) {
                    objectComponents[q].ExecuteResponsiveAction();
                }
                componentList.AddRange(objectComponents);
            }
            return componentList.ToArray();
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

        public static ResponsiveElement[] AddBreakpointToSelectedElements(GameObject[] gameObjects, float targetBreakpoint)
        {
            List<ResponsiveElement> componentList = new List<ResponsiveElement>();

            for (int i = 0; i < gameObjects.Length; i++) {

                ResponsiveElement[] objectComponents = gameObjects[i].GetComponents<ResponsiveElement>();
                for (int q = 0; q < objectComponents.Length; q++) {
                    objectComponents[q].AddBreakpoint(targetBreakpoint);
                }

                componentList.AddRange(objectComponents);
            }
            return componentList.ToArray();
        }

        public static ResponsiveElement[] AddBreakpointToSelectedElements(ResponsiveElement[] responsiveElements, float targetBreakpoint)
        {            
            for (int q = 0; q < responsiveElements.Length; q++) {
                responsiveElements[q].AddBreakpoint(targetBreakpoint);
            }
            return responsiveElements;
        }
    }
}
