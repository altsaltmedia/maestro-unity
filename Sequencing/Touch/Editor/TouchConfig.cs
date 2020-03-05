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
            
            return this;
        }
        
        enum ButtonNames
        {
            AxisMarker,
            AxisJoinNext,
            AxisJoinPrevious,
            TouchFork
        }
        private static VisualElementToggleData toggleData = new VisualElementToggleData();
        
        private enum EnableCondition
        {
            DirectoryAndNamePopulated
        }

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private string objectName => controlPanel.objectCreation.objectName;

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
                    break;

                case nameof(ButtonNames.AxisJoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(AxisMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.AxisJoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(AxisMarker_JoinPrevious), 0);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.TouchFork):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = Utils.CreateScriptableObjectAsset(typeof(TouchFork), selectedObjectDirectory, objectName);
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