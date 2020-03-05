using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Audio
{
    public class AudioTracks : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            audioTracks = this;

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            TrackPlacement.allowBlankTracksChanged += UpdateDisplay;
            TrackPlacement.trackCreated += OnTrackCreated;
            
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

        private static AudioTracks _audioTracks;

        private static AudioTracks audioTracks
        {
            get => _audioTracks;
            set => _audioTracks = value;
        }

        private TrackPlacement trackPlacement => controlPanel.trackPlacement;
        
        private bool allowBlankTracks => controlPanel.trackPlacement.allowBlankTracks;
        
        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;
        
        enum EnableCondition
        {
            AudioMixerSelected,
            AudioSourceSelected,
        }
        
        enum ButtonNames
        {
            AudioFadePlayVolumeTrack,
            AudioForwardReverseTrack,
            AudioLerpSnapshotTrack,
            AudioLerpVolumeTrack
        }

        private void UpdateDisplay()
        {
            // The user can force these buttons to enable by toggling allowBlankTracks //
            if (allowBlankTracks == true) {

                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioMixerSelected, true);
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioSourceSelected, true);

            }
            else {

                if (Utils.TargetTypeSelected(Selection.objects, typeof(AudioMixer))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioMixerSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioMixerSelected, false);
                }

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(AudioSource))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioSourceSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.AudioSourceSelected, false);
                }
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                case nameof(ButtonNames.AudioFadePlayVolumeTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateAudioFadePlayVolumeTrack();
                        }
                        else {
                            CreateAudioFadePlayVolumeTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.AudioLerpSnapshotTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateAudioLerpSnapshotTrack();
                        }
                        else {
                            CreateAudioLerpSnapshotTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.AudioMixerSelected, button);
                    break;

                case nameof(ButtonNames.AudioLerpVolumeTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateAudioLerpVolumeTrack();
                        }
                        else {
                            CreateAudioLerpVolumeTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.AudioSourceSelected, button);
                    break;

                case nameof(ButtonNames.AudioForwardReverseTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateAudioForwardReverseTrack();
                        }
                        else {
                            CreateAudioForwardReverseTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.AudioSourceSelected, button);
                    break;
            }

            return button;
        }

        [MenuItem("Edit/AltSalt/Audio/Fade Play Volume Track", false, 0)]
        public static void HotkeyCreateAudioFadePlayVolumeTrack()
        {
            bool selectCreatedObject = audioTracks.selectCreatedObject;

            if(selectCreatedObject == true) {
                Selection.objects = CreateAudioFadePlayVolumeTrack();
            } else {
                CreateAudioFadePlayVolumeTrack();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        [MenuItem("Edit/AltSalt/Audio/Lerp Snapshot Track", false, 0)]
        public static void HotkeyCreateAudioLerpSnapshotTrack()
        {
            bool selectCreatedObject = audioTracks.selectCreatedObject;

            if(selectCreatedObject == true) {
                Selection.objects = CreateAudioLerpSnapshotTrack();
            } else {
                CreateAudioLerpSnapshotTrack();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        public static TrackAsset[] CreateAudioFadePlayVolumeTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(AudioFadePlayVolumeTrack), typeof(AudioSource), Selection.objects, TimelineEditor.selectedClips);
        }
        public static TrackAsset[] CreateAudioLerpSnapshotTrack()
        {
            Object[] audioMixers = Utils.FilterSelection(Selection.objects, typeof(AudioMixer));
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, audioMixers, typeof(AudioLerpSnapshotTrack), typeof(AudioMixer), Selection.objects, TimelineEditor.selectedClips);
        }
        
        public static TrackAsset[] CreateAudioLerpVolumeTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(AudioLerpVolumeTrack), typeof(AudioSource), Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateAudioForwardReverseTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(AudioForwardReverseTrack), typeof(AudioSource), Selection.objects, TimelineEditor.selectedClips);
        }
        
        private static TrackAsset OnTrackCreated(PlayableDirector targetDirector, TrackAsset targetTrack, UnityEngine.Object targetObject)
        {
            foreach (PlayableBinding playableBinding in targetTrack.outputs) {

                switch (targetTrack.GetType().Name) {

                    case nameof(AudioForwardReverseTrack):
                    case nameof(AudioLerpVolumeTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<AudioSource>());
                        break;
                    
                    case nameof(AudioLerpSnapshotTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, targetObject);
                        break;
                }
            }

            return targetTrack;
        }
        
    }
}