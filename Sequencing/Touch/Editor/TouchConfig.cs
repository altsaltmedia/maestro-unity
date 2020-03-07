using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class TouchConfig : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
            UpdateDisplay();
            ControlPanel.inspectorUpdateDelegate += UpdateDisplay;
            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            
            return this;
        }
        
        private void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= UpdateDisplay;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private enum ButtonNames
        {
            AxisMarker,
            AxisJoinNext,
            AxisJoinPrevious,
            TouchFork
        }
        private static VisualElementToggleData toggleData = new VisualElementToggleData();
        
        private enum EnableCondition
        {
            TimelineEditorActive,
            DirectoryAndNamePopulated
        }

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private string objectName => controlPanel.objectCreation.objectName;

        private void UpdateDisplay()
        {
            if (TimelineEditor.inspectedAsset != null) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TimelineEditorActive, true);
            }
            else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TimelineEditorActive, false);
            }
            
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false && string.IsNullOrEmpty(objectName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, true);
            }
            else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.AxisMarker):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(AxisMarker), TimelineUtils.currentTime);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TimelineEditorActive, button);
                    break;

                case nameof(ButtonNames.AxisJoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(AxisMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TimelineEditorActive, button);
                    break;

                case nameof(ButtonNames.AxisJoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(AxisMarker_JoinPrevious), 0);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TimelineEditorActive, button);
                    break;
                
                case nameof(ButtonNames.TouchFork):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = Utils.CreateScriptableObjectAsset(typeof(TouchFork), objectName, selectedObjectDirectory);
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
            }

            return button;
        }

        public static ConfigMarker TriggerCreateMarker(ConfigTrack configTrack, Type markerType, double targetTime)
        {
            ConfigMarker configMarker = null;
            string markerTypeName = markerType.Name; 
            
            switch (markerTypeName) {

                case nameof(AxisMarker):
                {
                    configMarker = configTrack.CreateMarker<AxisMarker>(targetTime);
                }
                    break;

                case nameof(AxisMarker_JoinNext):
                {
                    configMarker = configTrack.CreateMarker<AxisMarker_JoinNext>(targetTime);
                }
                    break;

                case nameof(AxisMarker_JoinPrevious):
                {
                    configMarker = configTrack.CreateMarker<AxisMarker_JoinPrevious>(targetTime);
                }
                    break;
            }

            configMarker.name = markerTypeName;
            return configMarker;
        }
    }
}