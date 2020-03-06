using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    public class EventTracks : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            eventTracks = this;

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            TrackPlacement.allowBlankTracksChanged += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
            TrackPlacement.allowBlankTracksChanged -= UpdateDisplay;
        }
        
        VisualElementToggleData toggleData = new VisualElementToggleData();
        public FloatVariable targetFloat;
        public ColorVariable targetColor;

        private static EventTracks _eventTracks;

        private static EventTracks eventTracks
        {
            get => _eventTracks;
            set => _eventTracks = value;
        }

        private TrackPlacement trackPlacement => controlPanel.trackPlacement;
        
        private bool allowBlankTracks => controlPanel.trackPlacement.allowBlankTracks;
        
        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;
        
        enum EnableCondition
        {
            SimpleEventSelected,
            ComplexEventSelected
        }
        
        enum ButtonNames
        {
            SimpleEventTriggerTrack,
            ComplexEventTriggerTrack
        }

        private void UpdateDisplay()
        {
            // The user can force these buttons to enable by toggling allowBlankTracks //
            if (allowBlankTracks == true) {

                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SimpleEventSelected, true);
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ComplexEventSelected, true);

            }
            else {

                if (Utils.TargetTypeSelected(Selection.objects, typeof(SimpleEvent))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SimpleEventSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SimpleEventSelected, false);
                }

                if (Utils.TargetTypeSelected(Selection.objects, typeof(ComplexEvent))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ComplexEventSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ComplexEventSelected, false);
                }
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                case nameof(ButtonNames.SimpleEventTriggerTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateSimpleEventTriggerTrack();
                        }
                        else {
                            CreateSimpleEventTriggerTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.ComplexEventTriggerTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateComplexEventTriggerTrack();
                        }
                        else {
                            CreateComplexEventTriggerTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

            }

            return button;
        }

        [MenuItem("Edit/Maestro/Logic/Simple Event Trigger Track", false, 0)]
        public static void HotkeyCreateAudioFadePlayVolumeTrack()
        {
            bool selectCreatedObject = eventTracks.selectCreatedObject;

            if(selectCreatedObject == true) {
                Selection.objects = CreateSimpleEventTriggerTrack();
            } else {
                CreateSimpleEventTriggerTrack();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        [MenuItem("Edit/Maestro/Logic/Complex Event Trigger Track", false, 0)]
        public static void HotkeyCreateAudioLerpSnapshotTrack()
        {
            bool selectCreatedObject = eventTracks.selectCreatedObject;

            if(selectCreatedObject == true) {
                Selection.objects = CreateComplexEventTriggerTrack();
            } else {
                CreateComplexEventTriggerTrack();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        public static TrackAsset[] CreateSimpleEventTriggerTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SimpleEventTimelineTriggerTrack), null, Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateComplexEventTriggerTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ComplexEventTimelineTriggerTrack), null, Selection.objects, TimelineEditor.selectedClips);
        }

    }
}