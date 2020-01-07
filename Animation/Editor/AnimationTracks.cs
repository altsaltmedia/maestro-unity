using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Animation
{
    public class AnimationTracks : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            animationTracks = this;

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            var updateWindowTriggers = moduleWindowUXML.Query<VisualElement>(null, ModuleUtils.updateWindowTrigger);
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            TrackPlacement.allowBlankTracksChangedDelegate += UpdateDisplay;
            TrackPlacement.triggerCreateTrackDelegate += PopulateTrackAsset;
            
            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
            TrackPlacement.allowBlankTracksChangedDelegate -= UpdateDisplay;
        }
        
        VisualElementToggleData toggleData = new VisualElementToggleData();
        public FloatVariable targetFloat;
        public ColorVariable targetColor;

        private static AnimationTracks _animationTracks;

        private static AnimationTracks animationTracks
        {
            get => _animationTracks;
            set => _animationTracks = value;
        }

        private TrackPlacement trackPlacement => controlPanel.trackPlacement;
        
        private bool allowBlankTracks => controlPanel.trackPlacement.allowBlankTracks;
        
        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;
        
        enum EnableCondition
        {
            TextSelected,
            RectTransformSelected,
            SpriteSelected,
            FloatVarPopulated,
            ColorVarPopulated
        };
        
        enum ButtonNames
        {
            TMProColorTrack,
            RectTransformPosTrack,
            SpriteColorTrack,
            RectTransformScaleTrack,
            RectTransformRotationTrack,
            FloatVarTrack,
            ColorVarTrack,
            TMProCharSpacingTrack,
            ResponsiveRectTransformPosTrack,
            ResponsiveRectTransformScaleTrack,
        };
        
        enum UpdateWindowTriggers
        {
            AllowBlankTracks,
            TargetFloatVariable,
            TargetColorVariable
        };

        void UpdateDisplay()
        {
            // The user can force these buttons to enable by toggling allowBlankTracks //
            if (allowBlankTracks == true) {

                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);

            }
            else {

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(TMP_Text))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TextSelected, false);
                }

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(RectTransform))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, false);
                }

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(SpriteRenderer))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, false);
                }

                if (targetFloat != null) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, false);
                }

                if (targetColor != null) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ColorVarPopulated, true);
                }
                else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ColorVarPopulated, false);
                }

            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {
                case nameof(ButtonNames.TMProColorTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateTMProColorTrack();
                        }
                        else {
                            CreateTMProColorTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformPosTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack),
                                typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform),
                                Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.SpriteColorTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = CreateSpriteColorTrack();
                        }
                        else {
                            CreateSpriteColorTrack();
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformScaleTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, Selection.gameObjects,
                                typeof(RectTransformScaleTrack), typeof(RectTransform), Selection.objects,
                                TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                Selection.gameObjects, typeof(RectTransformScaleTrack), typeof(RectTransform),
                                Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformRotationTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, Selection.gameObjects,
                                typeof(RectTransformRotationTrack), typeof(RectTransform), Selection.objects,
                                TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                Selection.gameObjects, typeof(RectTransformRotationTrack), typeof(RectTransform),
                                Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.FloatVarTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, new UnityEngine.Object[] {targetFloat},
                                typeof(LerpFloatVarTrack), typeof(FloatVariable), Selection.objects,
                                TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                new UnityEngine.Object[] {targetFloat}, typeof(LerpFloatVarTrack),
                                typeof(FloatVariable), Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.FloatVarPopulated, button);
                    break;

                case nameof(ButtonNames.ColorVarTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, new UnityEngine.Object[] {targetColor},
                                typeof(LerpColorVarTrack), typeof(ColorVariable), Selection.objects,
                                TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                new UnityEngine.Object[] {targetColor}, typeof(LerpColorVarTrack),
                                typeof(ColorVariable), Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ColorVarPopulated, button);
                    break;

                case nameof(ButtonNames.TMProCharSpacingTrack):
                    button.clickable.clicked += () =>
                    {
                        if (selectCreatedObject == true) {
                            Selection.objects = TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset,
                                TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProCharSpacingTrack),
                                typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips);
                        }
                        else {
                            TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector,
                                Selection.gameObjects, typeof(TMProCharSpacingTrack), typeof(TMP_Text),
                                Selection.objects, TimelineEditor.selectedClips);
                        }

                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;
            }

            return button;
        }
        
        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.TargetFloatVariable):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateDisplay();
                    });
                    break;

                case nameof(UpdateWindowTriggers.TargetColorVariable):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateDisplay();
                    });
                    break;

            }

            return visualElement;
        }
        
        [MenuItem("Edit/AltSalt/Create Color Track", false, 0)]
        public static void HotkeyCreateColorTrack()
        {
            bool selectCreatedObject = animationTracks.selectCreatedObject;

            List<TrackAsset> newTracks = new List<TrackAsset>();
            Type[] types = { typeof(TMP_Text), typeof(SpriteRenderer) };

            UnityEngine.Object[] culledSelection = Utils.FilterSelection(Selection.gameObjects, types);
            GameObject[] selectionAsGameObjects = Array.ConvertAll(culledSelection, item => (GameObject)item);
            GameObject[] sortedSelection = Utils.SortGameObjectSelection(selectionAsGameObjects);

            for (int i=0; i< sortedSelection.Length; i++) {
                if(Utils.TargetComponentSelected(sortedSelection[i], typeof(TMP_Text))) {
                    newTracks.AddRange(TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new GameObject[] { sortedSelection[i] }, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips));
                }

                if (Utils.TargetComponentSelected(sortedSelection[i], typeof(SpriteRenderer))) {
                    newTracks.AddRange(TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new GameObject[] { sortedSelection[i] }, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects, TimelineEditor.selectedClips));
                }
            }

            if(selectCreatedObject == true) {
                Selection.objects = newTracks.ToArray();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        [MenuItem("Edit/AltSalt/Create Position Track", false, 0)]
        public static void HotkeyCreatePositionTrack()
        {
            bool selectCreatedObject = animationTracks.selectCreatedObject;

            if(selectCreatedObject == true) {
                Selection.objects = CreateRectTransformPosTrack();
            } else {
                CreateRectTransformPosTrack();
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
        }

        public static TrackAsset[] CreateTMProColorTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateSpriteColorTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateRectTransformPosTrack()
        {
            return TrackPlacement.TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
        }
        
        private static TrackAsset PopulateTrackAsset(PlayableDirector targetDirector, TrackAsset targetTrack, UnityEngine.Object targetObject)
        {
            foreach (PlayableBinding playableBinding in targetTrack.outputs) {

                switch (targetTrack.GetType().Name) {

                    case nameof(TMProColorTrack):
                    case nameof(TMProCharSpacingTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<TMP_Text>());
                        break;

                    case nameof(RectTransformPosTrack):
                    case nameof(RectTransformScaleTrack):
                    case nameof(RectTransformRotationTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<RectTransform>());
                        break;

                    case nameof(SpriteColorTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<SpriteRenderer>());
                        break;

                    case nameof(LerpFloatVarTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, targetObject);
                        break;

                    case nameof(LerpColorVarTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, targetObject);
                        break;

                    default:
                        Debug.LogError("Track type not supported");
                        break;
                }
            }

            return targetTrack;
        }
        
    }
}