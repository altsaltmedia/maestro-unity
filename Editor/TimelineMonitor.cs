using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public class TimelineMonitor : ModuleWindow
    {
        public bool enabled = true;

        private enum ListDisplayType
        {
            IntersectingClips,
            SelectedClips
        }

        private ListDisplayType _listDisplayType;

        private ListDisplayType listDisplayType
        {
            get => _listDisplayType;
            set => _listDisplayType = value;
        }

        private float _lastTimeValue;

        private float lastTimeValue
        {
            get => _lastTimeValue;
            set => _lastTimeValue = value;
        }

        private TimelineClip[] _previousClipSelection = new TimelineClip[0];

        private TimelineClip[] previousClipSelection
        {
            get => _previousClipSelection;
            set => _previousClipSelection = value;
        }

        private VisualElement _listContainer;

        private VisualElement listContainer
        {
            get => _listContainer;
            set => _listContainer = value;
        }

        private Label _displayTypeLabel;

        private Label displayTypeLabel
        {
            get => _displayTypeLabel;
            set => _displayTypeLabel = value;
        }

        private ListView _listView;

        private ListView listView
        {
            get => _listView;
            set => _listView = value;
        }

        private static TimelineClip[] _allTimelineClips = new TimelineClip[0];

        private static TimelineClip[] allTimelineClips
        {
            get => _allTimelineClips;
            set => _allTimelineClips = value;
        }

        private static List<TimelineClip> _listItemsSelected = new List<TimelineClip>();

        private static List<TimelineClip> listItemsSelected
        {
            get => _listItemsSelected;
            set => _listItemsSelected = value;
        }

        private static FloatVariable _timelineCurrentTime;

        private static FloatVariable timelineCurrentTime
        {
            get => _timelineCurrentTime;
            set => _timelineCurrentTime = value;
        }

        private List<TimelineClip> _clipsToDisplay = new List<TimelineClip>();

        private List<TimelineClip> clipsToDisplay => _clipsToDisplay;

        private enum ButtonNames
        {
            ToggleView,
            AddToSelection,
            RemoveFromSelection,
            DeselectAll
        }

        private enum ToggleNames
        {
            Enabled
        }

        [MenuItem("Tools/Maestro/Timeline Monitor")]
        public static void ShowWindow()
        {
            var moduleWindow = CreateInstance<TimelineMonitor>();
            moduleWindow.Init();
            moduleWindow.Show();
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            titleContent = new GUIContent("Timeline Monitor");
            ModuleWindow moduleWindow = Configure(null,
                ProjectNamespaceData.namespaceData[ModuleNamespace.Root].editorPath + nameof(TimelineMonitor) +
                "_UXML.uxml");

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.GetStylesheetPath());
            rootVisualElement.AddToClassList("altsalt");
            rootVisualElement.styleSheets.Add(styleSheet);

            rootVisualElement.Add(moduleWindow.moduleWindowUXML);
        }

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            listContainer = moduleWindowUXML.Query("ClipListContainer");
            timelineCurrentTime = Utils.GetFloatVariable(nameof(VarDependencies.TimelineCurrentTime));

            if (TimelineEditor.inspectedAsset != null) {
                allTimelineClips = TimelineUtils.GetAllTimelineClips();
            }

            if (this.controlPanel != null) {
                ControlPanel.inspectorUpdateDelegate += CallUpdateListView;
            }

            displayTypeLabel = moduleWindowUXML.Query<Label>("DisplayTypeLabel");
            displayTypeLabel.text = "Display Type : " + listDisplayType;
            ;

            Selection.selectionChanged += CallUpdateListView;

            UpdateListView(this);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            var toggles = moduleWindowUXML.Query<Toggle>();
            toggles.ForEach(SetupToggle);

            return this;
        }

        private void OnDisable()
        {
            if (this.controlPanel != null) {
                ControlPanel.inspectorUpdateDelegate -= CallUpdateListView;
            }

            Selection.selectionChanged -= CallUpdateListView;
        }

        private void OnInspectorUpdate()
        {
            if (this.controlPanel == null) {
                CallUpdateListView();
            }
        }

        private void CallUpdateListView()
        {
            if (enabled == false) return;

            if (TimelineEditor.inspectedAsset == null) return;

            if (Mathf.Approximately(lastTimeValue, timelineCurrentTime.value) && TimelineEditor.selectedClips.SequenceEqual(previousClipSelection)) return;

            lastTimeValue = timelineCurrentTime.value;
            previousClipSelection = TimelineEditor.selectedClips;

            allTimelineClips = TimelineUtils.GetAllTimelineClips();
            UpdateListView(this);
        }

        private VisualElement UpdateListView(TimelineMonitor timelineMonitor)
        {
            if (timelineMonitor.listContainer == null) {
                return timelineMonitor.listView;
            }

            timelineMonitor.listContainer.Clear();
            timelineMonitor.clipsToDisplay.Clear();

            if (timelineMonitor.listDisplayType == ListDisplayType.IntersectingClips) {
                return CreateIntersectingClipsListView(timelineMonitor);
            }

            return CreateSelectedClipsListView(timelineMonitor);
        }

        private static VisualElement CreateIntersectingClipsListView(TimelineMonitor timelineMonitor)
        {
            for (int i = 0; i < allTimelineClips.Length; i++) {
                if (timelineCurrentTime.value >= allTimelineClips[i].start &&
                    timelineCurrentTime.value <= allTimelineClips[i].end) {
                    timelineMonitor.clipsToDisplay.Add(allTimelineClips[i]);
                }
            }

            if (timelineMonitor.clipsToDisplay.Count > 0) {

                timelineMonitor.listContainer.Add(
                    new Label($"{timelineMonitor.clipsToDisplay.Count} clips intersecting"));

                timelineMonitor.listView = CreateListView(timelineMonitor.clipsToDisplay, true);
                timelineMonitor.listContainer.Add(timelineMonitor.listView);

            }
            else {
                Label label = new Label("No intersecting clips found");
                timelineMonitor.listContainer.Add(label);
            }

            return timelineMonitor.listContainer;
        }

        private static VisualElement CreateSelectedClipsListView(TimelineMonitor timelineMonitor)
        {
            timelineMonitor.clipsToDisplay.AddRange(TimelineEditor.selectedClips);

            if (timelineMonitor.clipsToDisplay.Count > 0) {

                timelineMonitor.listContainer.Add(new Label($"{timelineMonitor.clipsToDisplay.Count} clips selected"));

                timelineMonitor.listView = CreateListView(timelineMonitor.clipsToDisplay, true);
                timelineMonitor.listContainer.Add(timelineMonitor.listView);

            }
            else {
                Label label = new Label("No selected clips found");
                timelineMonitor.listContainer.Add(label);
            }

            return timelineMonitor.listContainer;
        }

        private static ListView CreateListView(List<TimelineClip> listElements, bool expandListView)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = (label, index) =>
            {
                Label labelUXML = label as Label;

                string labelText = listElements[index].displayName.LimitLength(11);

                UnityEngine.Object sourceObject = TimelineAssetManipulation.GetObjectFromTrackSelection(
                    listElements[index].parentTrack,
                    TimelineEditor.inspectedDirector);

                if (sourceObject != null) {
                    labelText += $" - {sourceObject.name}";
                }
                
                labelText = labelText.LimitLength(40);

                if (TimelineEditor.selectedClips.Contains(listElements[index])) {
                    labelText += " - SELECTED";
                }

                labelUXML.text = labelText;
            };

            const int itemHeight = 16;

            var listView = new ListView(listElements, itemHeight, makeItem, bindItem);
            listView.selectionType = SelectionType.Multiple;

            listView.onItemChosen += item => { TimelineEditor.selectedClip = item as TimelineClip; };
            listView.onSelectionChanged += items =>
            {
                TimelineMonitor.listItemsSelected = items.ConvertAll(item => (TimelineClip) item);
            };

            return ModuleUtils.ToggleListView(listView, expandListView);
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.ToggleView):
                    button.clickable.clicked += () =>
                    {
                        if (listDisplayType == ListDisplayType.IntersectingClips) {
                            listDisplayType = ListDisplayType.SelectedClips;
                        }
                        else {
                            listDisplayType = ListDisplayType.IntersectingClips;
                        }

                        displayTypeLabel.text = "Display Type : " + listDisplayType;
                        UpdateListView(this);
                    };
                    break;

                case nameof(ButtonNames.AddToSelection):
                    button.clickable.clicked += () =>
                    {
                        List<TimelineClip> newSelection = TimelineEditor.selectedClips.ToList();
                        newSelection.AddRange(listItemsSelected);
                        TimelineEditor.selectedClips = newSelection.ToArray();
                        TimelineUtils.RefreshTimelineContentsModified();
                        UpdateListView(this);
                    };
                    break;

                case nameof(ButtonNames.RemoveFromSelection):
                    button.clickable.clicked += () =>
                    {
                        List<TimelineClip> newSelection = TimelineEditor.selectedClips.ToList();
                        listItemsSelected.ForEach(x =>
                        {
                            if (newSelection.Contains(x)) {
                                newSelection.Remove(x);
                            }
                        });
                        TimelineEditor.selectedClips = newSelection.ToArray();
                        TimelineUtils.RefreshTimelineContentsModified();
                        UpdateListView(this);
                    };
                    break;

                case nameof(ButtonNames.DeselectAll):
                    button.clickable.clicked += () =>
                    {
                        TimelineEditor.selectedClips = new TimelineClip[0];
                        TimelineUtils.RefreshTimelineContentsModified();
                        UpdateListView(this);
                    };
                    break;
            }

            return button;
        }

        private Toggle SetupToggle(Toggle toggle)
        {
            switch (toggle.name) {

                case nameof(ToggleNames.Enabled):
                {
                    toggle.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) => {
                        if (evt.newValue == true) {
                            UpdateListView(this);
                        }
                    });
                    break;
                }
            }

            return toggle;
        }
    }
}