using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;
using TMPro;

namespace AltSalt
{
    public class TrackClipCreation : ChildUIElementsWindow
    {
        static TimelineUtilsWindow timelineUtilsWindow;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            timelineUtilsWindow = parentWindow as TimelineUtilsWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            var buttons = parentVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            var propertyFields = parentVisualElement.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var updateWindowTriggers = parentVisualElement.Query<VisualElement>(null, EditorToolsCore.UpdateWindowTrigger);
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);

            UpdateDisplay();
            TimelineUtilsWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            TimelineUtilsWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        public static List<TrackData> copiedTracks = new List<TrackData>();

        public bool selectCreatedTracks = true;
        public bool allowBlankTracks = false;
        public float newClipDuration = .5f;
        public string clipName = "";
        public bool selectCreatedClip = true;
        public bool advancePlayhead = true;
        public FloatVariable targetFloat;
        public ColorVariable targetColor;

        Dictionary<EnableCondition, List<VisualElement>> toggleData = new Dictionary<EnableCondition, List<VisualElement>>();

        enum EnableCondition
        {
            TrackSelectedForCopying,
            TextSelected,
            RectTransformSelected,
            SpriteSelected,
            TrackSelected,
            CopiedTracksPopulated,
            FloatVarPopulated,
            ColorVarPopulated,
            TrackOrClipSelected,
            ClipSelected
        };

        enum ButtonNames
        {
            RefreshLayout,
            CopyTracks,
            PasteTracks,
            GroupTrack,
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
            NewClips,
            RenameClips
        };

        enum PropertyFieldNames
        {
            NewClipDuration
        }

        enum UpdateWindowTriggers
        {
            RootWindow,
            AllowBlankTracks,
            TargetFloatVariable,
            TargetColorVariable
        };

        void UpdateDisplay()
        {

            // The user can force these buttons to enable by toggling allowBlankTracks //

            if (allowBlankTracks == true) {

                ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);
                ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);

            } else {

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(TMP_Text))) {
                    ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.TextSelected, false);
                }

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(RectTransform))) {
                    ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, false);
                }

                if (Utils.TargetComponentSelected(Selection.gameObjects, typeof(SpriteRenderer))) {
                    ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.SpriteSelected, false);
                }

                if (targetFloat != null) {
                    ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.FloatVarPopulated, false);
                }

                if (targetColor != null) {
                    ToggleVisualElements(toggleData, EnableCondition.ColorVarPopulated, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.ColorVarPopulated, false);
                }

                if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                    ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);
                } else {
                    ToggleVisualElements(toggleData, EnableCondition.TrackSelected, false);
                }
            }

            // These elements cannot be overriden by allowBlankTracks //

            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForCopying, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForCopying, false);
            }

            if (copiedTracks.Count > 0) {
                ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, false);
            }

            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset)) || TimelineEditor.selectedClips.Length > 0) {
                ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, false);
            }

            if (TimelineEditor.selectedClips.Length > 0) {
                ToggleVisualElements(toggleData, EnableCondition.ClipSelected, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.ClipSelected, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.CopyTracks):
                    button.clickable.clicked += () => {
                        copiedTracks = CopyTracks(TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineUtilsCore.RefreshTimelineContentsModified();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelectedForCopying, button);
                    break;

                case nameof(ButtonNames.PasteTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = PasteTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects, copiedTracks);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.CopiedTracksPopulated, button);
                    break;

                case nameof(ButtonNames.GroupTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.activeObject = TriggerCreateGroupTrack(TimelineEditor.inspectedAsset, Selection.objects);
                        } else {
                            TriggerCreateGroupTrack(TimelineEditor.inspectedAsset, Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;

                case nameof(ButtonNames.TMProColorTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformPosTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.SpriteColorTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformScaleTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformScaleTrack), typeof(RectTransform), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformScaleTrack), typeof(RectTransform), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformRotationTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformRotationTrack), typeof(RectTransform), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformRotationTrack), typeof(RectTransform), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.FloatVarTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetFloat }, typeof(LerpFloatVarTrack), typeof(FloatVariable), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetFloat }, typeof(LerpFloatVarTrack), typeof(FloatVariable), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.FloatVarPopulated, button);
                    break;

                case nameof(ButtonNames.ColorVarTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetColor }, typeof(LerpColorVarTrack), typeof(ColorVariable), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetColor }, typeof(LerpColorVarTrack), typeof(ColorVariable), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.ColorVarPopulated, button);
                    break;

                case nameof(ButtonNames.TMProCharSpacingTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProCharSpacingTrack), typeof(TMP_Text), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProCharSpacingTrack), typeof(TMP_Text), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.ResponsiveRectTransformPosTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformPosTrack), typeof(RectTransform), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformPosTrack), typeof(RectTransform), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.ResponsiveRectTransformScaleTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformScaleTrack), typeof(RectTransform), Selection.objects);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformScaleTrack), typeof(RectTransform), Selection.objects);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.NewClips):
                    button.clickable.clicked += () => {
                        if (selectCreatedClip == true) {
                            TimelineEditor.selectedClips = CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName);
                        } else {
                            CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();

                        if (advancePlayhead == true) {
                            TimelineUtilsCore.CurrentTime += newClipDuration;
                            TimelineUtilsCore.RefreshTimelineRedrawWindow();
                        }
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackOrClipSelected, button);
                    break;

                case nameof(ButtonNames.RenameClips):
                    button.clickable.clicked += () => {
                        if (clipName.Length > 0) {
                            TimelineUtilsCore.RenameClips(clipName, TimelineEditor.selectedClips);
                            TimelineUtilsCore.RefreshTimelineContentsModified();
                        }
                    };
                    AddToToggleData(toggleData, EnableCondition.ClipSelected, button);
                    break;
            }

            return button;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
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

        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.AllowBlankTracks):
                    visualElement.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) => {
                        allowBlankTracks = evt.newValue;
                        UpdateDisplay();
                    });
                    break;

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

        Dictionary<EnableCondition, List<VisualElement>> AddToToggleData(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, VisualElement elementToAdd)
        {
            if (targetCollection.ContainsKey(targetCondition)) {
                targetCollection[targetCondition].Add(elementToAdd);
            } else {
                List<VisualElement> buttonList = new List<VisualElement>();
                buttonList.Add(elementToAdd);
                targetCollection.Add(targetCondition, buttonList);
            }

            return targetCollection;
        }

        Dictionary<EnableCondition, List<VisualElement>> ToggleVisualElements(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, bool targetStatus = false)
        {
            if (targetCollection.ContainsKey(targetCondition)) {
                List<VisualElement> buttonList = targetCollection[targetCondition];
                for (int i = 0; i < buttonList.Count; i++) {
                    buttonList[i].SetEnabled(targetStatus);
                }
            }
            return targetCollection;
        }

        public static List<TrackData> CopyTracks(PlayableDirector sourceDirector, UnityEngine.Object[] sourceObjects)
        {
            List<TrackData> tracksToCopy = new List<TrackData>();
            for (int i = 0; i < sourceObjects.Length; i++) {
                if (sourceObjects[i] is TrackAsset) {
                    List<TrackData> trackData = GetTrackData(sourceDirector, sourceObjects[i] as TrackAsset);
                    tracksToCopy.AddRange(trackData);
                }
            }
            return tracksToCopy;
        }

        static List<TrackData> GetTrackData(PlayableDirector sourceDirector, TrackAsset sourceTrack)
        {
            List<TrackData> trackData = new List<TrackData>();
            UnityEngine.Object sourceObject = new UnityEngine.Object();
            foreach (PlayableBinding playableBinding in sourceTrack.outputs) {
                sourceObject = sourceDirector.GetGenericBinding(playableBinding.sourceObject);
            }
            trackData.Add(new TrackData(sourceTrack, sourceObject, sourceTrack.GetClips()));
            foreach (TrackAsset childTrack in sourceTrack.GetChildTracks()) {
                trackData.AddRange(GetTrackData(sourceDirector, childTrack));
            }
            return trackData;
        }

        public static TrackAsset[] PasteTracks(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] destinationSelection, List<TrackData> sourceTrackData)
        {
            TrackAsset[] pastedTracks = new TrackAsset[sourceTrackData.Count];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            // Paste tracks
            for (int i = 0; i < sourceTrackData.Count; i++) {
                TrackAsset trackAsset = targetTimelineAsset.CreateTrack(sourceTrackData[i].trackType, parentTrack, sourceTrackData[i].trackName);
                pastedTracks[i] = trackAsset;

                foreach (PlayableBinding playableBinding in trackAsset.outputs) {
                    targetDirector.SetGenericBinding(playableBinding.sourceObject, sourceTrackData[i].trackBinding);
                }
                foreach (TimelineClip trackClip in sourceTrackData[i].trackClips) {
                    TimelineClip pastedClip = trackAsset.CreateDefaultClip();

                    pastedClip.duration = trackClip.duration;
                    pastedClip.start = trackClip.start;

                    Type assetType = pastedClip.asset.GetType();
                    FieldInfo[] assetFields = assetType.GetFields();

                    foreach (FieldInfo assetField in assetFields) {
                        assetField.SetValue(pastedClip.asset, assetField.GetValue(trackClip.asset));
                    }
                }
            }

            // Set groups if applicable
            for (int i = 0; i < sourceTrackData.Count; i++) {
                if (sourceTrackData[i].groupTrack != null) {
                    for (int q = 0; q < pastedTracks.Length; q++) {
                        if (pastedTracks[q].name == sourceTrackData[i].groupTrack.name) {
                            pastedTracks[i].SetGroup(pastedTracks[q] as GroupTrack);
                        }
                    }
                }
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();

            return pastedTracks;
        }

        static bool ObjectsSelected()
        {
            if (Selection.objects.Length > 0) {
                return true;
            } else {
                return false;
            }
        }

        static bool TracksSelected()
        {
            for (int i = 0; i < Selection.objects.Length; i++) {
                if (Selection.objects[i] is TrackAsset) {
                    return true;
                }
            }
            return false;
        }

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] sourceObjects, Type trackType, Type requiredObjectType, UnityEngine.Object[] destinationSelection)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceObjects.Length > 0) {
                for (int i = 0; i < sourceObjects.Length; i++) {
                    if (requiredObjectType != null && Utils.TargetTypeSelected(sourceObjects[i], requiredObjectType) == true) {
                        TrackAsset newTrack = CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
                        trackAssets.Add(newTrack);
                        PopulateTrackAsset(targetDirector, newTrack, sourceObjects[i]);
                    }
                }
            } else {
                trackAssets.Add(CreateNewTrack(targetTimelineAsset, parentTrack, trackType));
            }
            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
            return trackAssets.ToArray();
        }

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, GameObject[] sourceGameObjects, Type trackType, Type requiredComponentType, UnityEngine.Object[] destinationSelection)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceGameObjects.Length > 0) {
                Array.Sort(sourceGameObjects, new Utils.GameObjectSort());
                for (int i = 0; i < sourceGameObjects.Length; i++) {
                    if (requiredComponentType != null && Utils.TargetComponentSelected(sourceGameObjects[i], requiredComponentType) == true) {
                        TrackAsset newTrack = CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
                        trackAssets.Add(newTrack);
                        PopulateTrackAsset(targetDirector, newTrack, sourceGameObjects[i]);
                    }
                }
            } else {
                trackAssets.Add(CreateNewTrack(targetTimelineAsset, parentTrack, trackType));
            }
            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
            return trackAssets.ToArray();
        }

        static TrackAsset CreateNewTrack(TimelineAsset targetTimelineAsset, TrackAsset parentTrack, Type trackType)
        {
            TrackAsset trackAsset;
            trackAsset = targetTimelineAsset.CreateTrack(trackType, parentTrack, trackType.Name);
            return trackAsset;
        }

        static TrackAsset PopulateTrackAsset(PlayableDirector targetDirector, TrackAsset targetTrack, UnityEngine.Object targetObject)
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
                    case nameof(ResponsiveRectTransformPosTrack):
                    case nameof(ResponsiveRectTransformScaleTrack):
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

        public static TimelineClip[] CreateClips(PlayableDirector targetDirector, UnityEngine.Object[] objectSelection, TimelineClip[] clipSelection, float duration, string clipName)
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
                newClip.start = TimelineUtilsCore.CurrentTime;
                newClip.duration = duration;
                PopulateClip(targetDirector, targetTracks[i], newClip);
                timelineClips.Add(newClip);
            }

            TimelineClip[] timelineClipsArray = timelineClips.ToArray();

            if (clipName.Length > 0) {
                TimelineUtilsCore.RenameClips(clipName, timelineClipsArray);
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
            return timelineClipsArray;
        }

        static PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack, TimelineClip timelineClip)
        {
            UnityEngine.Object sourceObject = null;
            foreach (PlayableBinding playableBinding in parentTrack.outputs) {
                sourceObject = targetDirector.GetGenericBinding(playableBinding.sourceObject);
            }

            switch (timelineClip.asset.GetType().Name) {

                case nameof(TMProColorClip): {
                        TMProColorClip asset = timelineClip.asset as TMProColorClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialColor = component.color;
                            asset.template.targetColor = component.color;
                        }
                        return asset;
                    }

                case nameof(RectTransformPosClip): {
                        RectTransformPosClip asset = timelineClip.asset as RectTransformPosClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialPosition = component.anchoredPosition3D;
                            asset.template.targetPosition = component.anchoredPosition3D;
                        }
                        return asset;
                    }

                case nameof(SpriteColorClip): {
                        SpriteColorClip asset = timelineClip.asset as SpriteColorClip;
                        SpriteRenderer component = sourceObject as SpriteRenderer;
                        if (component != null) {
                            asset.template.initialColor = component.color;
                            asset.template.targetColor = component.color;
                        }
                        return asset;
                    }

                case nameof(RectTransformScaleClip): {
                        RectTransformScaleClip asset = timelineClip.asset as RectTransformScaleClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialScale = component.localScale;
                            asset.template.targetScale = component.localScale;
                        }
                        return asset;
                    }

                case nameof(RectTransformRotationClip): {
                        RectTransformRotationClip asset = timelineClip.asset as RectTransformRotationClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialRotation = component.localEulerAngles;
                            asset.template.targetRotation = component.localEulerAngles;
                        }
                        return asset;
                    }

                case nameof(LerpFloatVarClip): {
                        LerpFloatVarClip asset = timelineClip.asset as LerpFloatVarClip;
                        FloatVariable component = sourceObject as FloatVariable;
                        if (component != null) {
                            asset.template.initialValue = component.Value;
                            asset.template.targetValue = component.Value;
                        }
                        return asset;
                    }

                case nameof(LerpColorVarClip): {
                        LerpColorVarClip asset = timelineClip.asset as LerpColorVarClip;
                        ColorVariable component = sourceObject as ColorVariable;
                        if (component != null) {
                            asset.template.initialColor = component.Value;
                            asset.template.targetColor = component.Value;
                        }
                        return asset;
                    }

                case nameof(TMProCharSpacingClip): {
                        TMProCharSpacingClip asset = timelineClip.asset as TMProCharSpacingClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialValue = component.characterSpacing;
                            asset.template.targetValue = component.characterSpacing;
                        }
                        return asset;
                    }

                case nameof(ResponsiveVector3Clip): {
                        ResponsiveVector3Clip asset = timelineClip.asset as ResponsiveVector3Clip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {

                            if(parentTrack is ResponsiveRectTransformPosTrack) {
                                asset.template.breakpointInitialValue.Add(component.anchoredPosition3D);
                                asset.template.breakpointTargetValue.Add(component.anchoredPosition3D);
                            } else if (parentTrack is ResponsiveRectTransformScaleTrack) {
                                asset.template.breakpointInitialValue.Add(component.localScale);
                                asset.template.breakpointTargetValue.Add(component.localScale);
                            }
                        }
                        return asset;
                    }
            }

            return null;
        }

        static TrackAsset GetDestinationTrackFromSelection(UnityEngine.Object[] destinationSelection)
        {
            TrackAsset destinationTrack = null;
            for (int q = 0; q < destinationSelection.Length; q++) {
                if (destinationSelection[q] is GroupTrack) {
                    destinationTrack = destinationSelection[q] as TrackAsset;
                    return destinationTrack;
                }
            }
            for (int q = 0; q < destinationSelection.Length; q++) {
                TrackAsset trackAsset = destinationSelection[q] as TrackAsset;
                if (trackAsset != null && trackAsset.parent is GroupTrack) {
                    destinationTrack = trackAsset.parent as TrackAsset;
                    return destinationTrack;
                }
            }
            return destinationTrack;
        }

        public static GroupTrack TriggerCreateGroupTrack(TimelineAsset targetTimelineAsset, UnityEngine.Object[] childSelection)
        {
            GroupTrack groupTrack = targetTimelineAsset.CreateTrack(typeof(GroupTrack), null, typeof(GroupTrack).Name) as GroupTrack;
            GroupTrack childGroup = null;

            for (int i = 0; i < childSelection.Length; i++) {
                if (childSelection[i] is TrackAsset) {
                    TrackAsset childTrack = childSelection[i] as TrackAsset;
                    if (childGroup == null) {
                        childGroup = childTrack.GetGroup();
                    }
                    childTrack.SetGroup(groupTrack);
                }
            }

            if (childGroup != null) {
                groupTrack.SetGroup(childGroup);
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();

            return groupTrack;
        }

        public class TrackData
        {
            public TrackAsset trackAsset;
            public GroupTrack groupTrack;
            public Type trackType;
            public string trackName;
            public UnityEngine.Object trackBinding;
            public IEnumerable<TimelineClip> trackClips;

            public TrackData(TrackAsset trackAsset, UnityEngine.Object binding, IEnumerable<TimelineClip> trackClips)
            {
                this.trackAsset = trackAsset;
                this.groupTrack = trackAsset.GetGroup();
                this.trackType = trackAsset.GetType();
                this.trackName = trackAsset.name;
                this.trackBinding = binding;
                this.trackClips = trackClips;
            }
        }
    }
}
