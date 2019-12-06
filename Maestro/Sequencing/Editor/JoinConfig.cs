using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinConfig : ModuleWindow
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
            JoinNext,
            JoinPrevious,
            ForkJoinNext,
            ForkJoinPrevious
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.JoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(JoinMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.JoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(JoinMarker_JoinPrevious), 0);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.ForkJoinNext):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(ForkMarker_JoinNext), TimelineEditor.inspectedAsset.duration);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.ForkJoinPrevious):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineUtils.configTrack,
                            typeof(ForkMarker_JoinPrevious), 0);
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

                case nameof(JoinMarker_JoinNext):
                {
                    configMarker = configTrack.CreateMarker<JoinMarker_JoinNext>(targetTime);
                }
                    break;

                case nameof(JoinMarker_JoinPrevious):
                {
                    configMarker = configTrack.CreateMarker<JoinMarker_JoinPrevious>(targetTime);
                }
                    break;

                case nameof(ForkMarker_JoinNext):
                {
                    configMarker = configTrack.CreateMarker<ForkMarker_JoinNext>(targetTime);
                }
                    break;
                
                case nameof(ForkMarker_JoinPrevious):
                {
                    configMarker = configTrack.CreateMarker<ForkMarker_JoinPrevious>(targetTime);
                }
                    break;
            }

            return configMarker;
        }
    }
}