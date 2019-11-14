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
            AxisJoinPrevious
        }

        Button SetupButton(Button button)
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
            }

            return button;
        }

        public static ConfigMarker TriggerCreateMarker(ConfigTrack configTrack, Type markerType, double targetTime)
        {
            ConfigMarker configMarker = null;
            
            switch (markerType.Name) {

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

            return configMarker;
        }
    }
}