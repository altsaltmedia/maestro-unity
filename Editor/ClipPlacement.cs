using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;

namespace AltSalt.Maestro
{
    public class ClipPlacement : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            clipPlacement = this;

            triggerCreateClipDelegates.Clear();
            EnablePopulateClipModules();

            var propertyFields = moduleWindowUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();

            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }
        
        private static ClipPlacement _clipPlacement;

        private static ClipPlacement clipPlacement
        {
            get => _clipPlacement;
            set => _clipPlacement = value;
        }

        private delegate PlayableAsset TriggerCreateClipDelegate(PlayableDirector targetDirector, TrackAsset parentTrack, EasingFunction.Ease easeType, TimelineClip timelineClip);

        private static readonly List<TriggerCreateClipDelegate> triggerCreateClipDelegates = new List<TriggerCreateClipDelegate>();
        
        private enum PopulateClipModules
        {
            PopulateAnimationClips,
            PopulateAudioClips,
            PopulateEventClips
        }

        private static readonly Dictionary<ModuleNamespace, PopulateClipModules> _populateClipNamespaces =
            new Dictionary<ModuleNamespace, PopulateClipModules>
            {
                {ModuleNamespace.Animation, PopulateClipModules.PopulateAnimationClips},
                {ModuleNamespace.Audio, PopulateClipModules.PopulateAudioClips},
                {ModuleNamespace.Root, PopulateClipModules.PopulateEventClips}
            };

        private static Dictionary<ModuleNamespace, PopulateClipModules> populateClipNamespaces => _populateClipNamespaces;

        public float newClipDuration = .5f;
        public EasingFunction.Ease clipEaseType = EasingFunction.Ease.EaseInOutQuad;
        public string clipName = "";
        public bool selectCreatedClip = true;
        public bool advancePlayhead = true;
        
        private static VisualElementToggleData toggleData = new VisualElementToggleData();

        private enum PropertyFieldNames
        {
            NewClipDuration
        }

        private enum ButtonNames
        {
            NewClips,
            RenameClips
        }

        private enum EnableCondition
        {
            TrackOrClipSelected,
            ClipSelected
        }

        private static void EnablePopulateClipModules()
        {
            foreach (KeyValuePair<ModuleNamespace, PopulateClipModules> populateClipNamespace in populateClipNamespaces) {
                ModuleNamespaceStrings namespaceStrings = ProjectNamespaceData.namespaceData[populateClipNamespace.Key];
                var moduleClassType = Type.GetType($"{namespaceStrings.name}.{populateClipNamespace.Value.ToString()}");
                
                if (moduleClassType != null) {
                    MethodInfo methodInfo = moduleClassType.GetMethod(nameof(PopulateClips.PopulateClip));

                    if (methodInfo != null) {

                        var populateClipInstance = Activator.CreateInstance(moduleClassType);
                        triggerCreateClipDelegates.Add(Delegate.CreateDelegate(typeof(TriggerCreateClipDelegate), populateClipInstance, methodInfo, true) as TriggerCreateClipDelegate);
                    }
                }
            }
        }

        private static void UpdateDisplay()
        {
            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset)) == true || TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, false);
            }

            if (TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipSelected, false);
            }
        }

        private PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.NewClipDuration):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (evt.newValue < .1f) {
                            newClipDuration = .1f;
                        }
                    });
                    break;
            }
            return propertyField;
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.NewClips):
                    button.clickable.clicked += () =>
                    {
                        TriggerCreateClips(selectCreatedClip, advancePlayhead, newClipDuration, clipName,
                            clipEaseType);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackOrClipSelected,
                        button);
                    break;

                case nameof(ButtonNames.RenameClips):
                    button.clickable.clicked += () =>
                    {
                        if (clipName.Length > 0) {
                            TimelineUtils.RenameClips(clipName, TimelineEditor.selectedClips);
                            TimelineUtils.RefreshTimelineContentsModified();
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipSelected, button);
                    break;
            }

            return button;
        }
        
        [MenuItem("Edit/Maestro/Timeline/Create New Clip(s)", false, 0)]
        public static void HotkeyTriggerCreateClips()
        {
            bool selectCreatedClip = true;
            bool advancePlayhead = true;
            float newClipDuration = .5f;
            string clipName = "";
            EasingFunction.Ease clipEaseType = EasingFunction.Ease.EaseInOutQuad;
            
            if (clipPlacement != null) {
                selectCreatedClip = clipPlacement.selectCreatedClip;
                advancePlayhead = clipPlacement.advancePlayhead;
                newClipDuration = clipPlacement.newClipDuration;
                clipName = clipPlacement.clipName;
                clipEaseType = clipPlacement.clipEaseType;
            }

            TriggerCreateClips(selectCreatedClip, advancePlayhead, newClipDuration, clipName, clipEaseType);
        }
        
        public static void TriggerCreateClips(bool selectCreatedClip, bool advancePlayhead, float newClipDuration, string clipName, EasingFunction.Ease clipEaseType)
        {
            if (selectCreatedClip == true) {
                TimelineEditor.selectedClips = CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName, clipEaseType);
            } else {
                CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName, clipEaseType);
            }
            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();

            if (advancePlayhead == true) {
                TimelineUtils.currentTime += newClipDuration;
                TimelineUtils.RefreshTimelineRedrawWindow();
            }
        }
        
        public static TimelineClip[] CreateClips(PlayableDirector targetDirector, UnityEngine.Object[] objectSelection, TimelineClip[] clipSelection, float duration, string clipName, EasingFunction.Ease easeType)
        {
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            List<TrackAsset> targetTracks = new List<TrackAsset>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if (objectSelection[i] is TrackAsset && objectSelection[i] is GroupTrack == false) {
                    targetTracks.Add(objectSelection[i] as TrackAsset);
                }
            }

            for (int i = 0; i < clipSelection.Length; i++) {
                TrackAsset trackAsset = clipSelection[i].parentTrack;

                // It is possible to have multiple clips selected on the same track,
                // so this conditional prevents us from adding duplicates
                if (targetTracks.Contains(trackAsset) == false) {
                    targetTracks.Add(trackAsset);
                }
            }

            for (int i = 0; i < targetTracks.Count; i++) {
                TimelineClip newClip = targetTracks[i].CreateDefaultClip();
                newClip.start = TimelineUtils.currentTime;
                newClip.duration = duration;
                triggerCreateClipDelegates.ForEach(x => x.Invoke(targetDirector, targetTracks[i], easeType, newClip));;
                timelineClips.Add(newClip);
            }

            TimelineClip[] timelineClipsArray = timelineClips.ToArray();

            if (clipName.Length > 0) {
                TimelineUtils.RenameClips(clipName, timelineClipsArray);
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
            return timelineClipsArray;
        }
        
    }
}