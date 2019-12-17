using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class AutorunConfig : ModuleWindow
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
            AutoplayStart,
            AutoplayPause,
            AutoplayEnd
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.AutoplayStart):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack,
                            typeof(AutorunMarker_Start), TimelineUtils.currentTime);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.AutoplayPause):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack,
                            typeof(AutorunMarker_Pause), TimelineUtils.currentTime);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.AutoplayEnd):
                    button.clickable.clicked += () =>
                    {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack,
                            typeof(AutorunMarker_End), TimelineUtils.currentTime);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
            }

            return button;
        }

        public static ConfigMarker TriggerCreateMarker(MarkerTrack markerTrack, Type markerType, double targetTime)
        {
            ConfigMarker configMarker = null;
            string markerTypeName = markerType.Name; 
            
            switch (markerTypeName) {

                case nameof(AutorunMarker_Start):
                {
                    configMarker = markerTrack.CreateMarker<AutorunMarker_Start>(targetTime);
                }
                    break;

                case nameof(AutorunMarker_Pause):
                {
                    configMarker = markerTrack.CreateMarker<AutorunMarker_Pause>(targetTime);
                }
                    break;

                case nameof(AutorunMarker_End):
                {
                    configMarker = markerTrack.CreateMarker<AutorunMarker_End>(targetTime);
                }
                    break;
            }

            configMarker.name = markerTypeName;

            return configMarker;
        }
    }
}