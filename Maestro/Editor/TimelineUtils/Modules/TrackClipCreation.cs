using System;
using System.Reflection;
using System.Collections.Generic;
using AltSalt.Maestro.Sequencing;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;
using TMPro;
using AltSalt.Maestro.Sequencing.Autorun;

namespace AltSalt.Maestro
{
    public class TrackClipCreation : ChildUIElementsWindow
    {
        static TimelineUtilsWindow timelineUtilsWindow;
        static TrackClipCreation trackClipWindow;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            timelineUtilsWindow = parentWindow as TimelineUtilsWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            trackClipWindow = this;

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
        public EasingFunction.Ease clipEaseType = EasingFunction.Ease.EaseInOutQuad;
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
            DuplicateTracks,
            GroupTrack,
            AutoplayStart,
            AutoplayPause,
            AutoplayEnd,
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
                        Selection.objects = PasteTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, copiedTracks);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.CopiedTracksPopulated, button);
                    break;

                case nameof(ButtonNames.DuplicateTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TrackSelectedForCopying, button);
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
                
                case nameof(ButtonNames.AutoplayStart):
                    button.clickable.clicked += () => {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack, typeof(AutorunMarker_Start), TimelineUtilsCore.CurrentTime);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.AutoplayPause):
                    button.clickable.clicked += () => {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack, typeof(AutorunMarker_Pause), TimelineUtilsCore.CurrentTime);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;
                
                case nameof(ButtonNames.AutoplayEnd):
                    button.clickable.clicked += () => {
                        Selection.activeObject = TriggerCreateMarker(TimelineEditor.inspectedAsset.markerTrack, typeof(AutorunMarker_End), TimelineUtilsCore.CurrentTime);
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    break;

                case nameof(ButtonNames.TMProColorTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = CreateTMProColorTrack();
                        } else {
                            CreateTMProColorTrack();
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformPosTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformPosTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.SpriteColorTrack):
                    button.clickable.clicked += () => {
                        if(selectCreatedTracks == true) {
                            Selection.objects = CreateSpriteColorTrack();
                        } else {
                            CreateSpriteColorTrack();
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.SpriteSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformScaleTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformScaleTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformScaleTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.RectTransformRotationTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformRotationTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(RectTransformRotationTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.FloatVarTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetFloat }, typeof(LerpFloatVarTrack), typeof(FloatVariable), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetFloat }, typeof(LerpFloatVarTrack), typeof(FloatVariable), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.FloatVarPopulated, button);
                    break;

                case nameof(ButtonNames.ColorVarTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetColor }, typeof(LerpColorVarTrack), typeof(ColorVariable), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new UnityEngine.Object[] { targetColor }, typeof(LerpColorVarTrack), typeof(ColorVariable), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.ColorVarPopulated, button);
                    break;

                case nameof(ButtonNames.TMProCharSpacingTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProCharSpacingTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProCharSpacingTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.TextSelected, button);
                    break;

                case nameof(ButtonNames.ResponsiveRectTransformPosTrack):
                    button.clickable.clicked += () => {
                        if(selectCreatedTracks == true) {
                            Selection.objects = CreateResponsiveRectTransformPosTrack();
                        } else {
                            CreateResponsiveRectTransformPosTrack();
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.ResponsiveRectTransformScaleTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedTracks == true) {
                            Selection.objects = TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformScaleTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        } else {
                            TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformScaleTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
                        }
                        TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
                    };
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, button);
                    break;

                case nameof(ButtonNames.NewClips):
                    button.clickable.clicked += () => {
                        TriggerCreateClips(selectCreatedClip, advancePlayhead, newClipDuration, clipName, clipEaseType);
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

        public static TrackAsset[] PasteTracks(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] destinationSelection, TimelineClip[] clipSelection, List<TrackData> sourceTrackData)
        {
            TrackAsset[] pastedTracks = new TrackAsset[sourceTrackData.Count];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);
            if (parentTrack == null) {
                parentTrack = GetDestinationTrackFromSelection(clipSelection);
            }

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

                    // Need to create a copy of the asset, otherwise the new clip
                    // will use references to the old clip's asset values
                    var trackClipAssetCopy = Instantiate(trackClip.asset);

                    foreach (FieldInfo assetField in assetFields) {

                        assetField.SetValue(pastedClip.asset, assetField.GetValue(trackClipAssetCopy));                        
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

        public static TrackAsset[] DuplicateTracks(TimelineAsset targetTimelineAsset, PlayableDirector sourceDirector, UnityEngine.Object[] sourceObjects)
        {
            List<TrackData> sourceTrackData = new List<TrackData>();
            for (int i = 0; i < sourceObjects.Length; i++) {
                if (sourceObjects[i] is TrackAsset) {
                    List<TrackData> trackData = GetTrackData(sourceDirector, sourceObjects[i] as TrackAsset);
                    sourceTrackData.AddRange(trackData);
                }
            }

            TrackAsset[] pastedTracks = new TrackAsset[sourceTrackData.Count];

            // Create tracks from selection
            for (int i = 0; i < sourceTrackData.Count; i++) {

                TrackAsset trackAsset = targetTimelineAsset.CreateTrack(sourceTrackData[i].trackType, null, sourceTrackData[i].trackName);
                pastedTracks[i] = trackAsset;

                foreach (PlayableBinding playableBinding in trackAsset.outputs) {
                    sourceDirector.SetGenericBinding(playableBinding.sourceObject, sourceTrackData[i].trackBinding);
                }
                foreach (TimelineClip trackClip in sourceTrackData[i].trackClips) {
                    TimelineClip pastedClip = trackAsset.CreateDefaultClip();

                    pastedClip.duration = trackClip.duration;
                    pastedClip.start = trackClip.start;

                    Type assetType = pastedClip.asset.GetType();
                    FieldInfo[] assetFields = assetType.GetFields();

                    // Need to create a copy of the asset, otherwise the new clip
                    // will use references to the old clip's asset values
                    var trackClipAssetCopy = Instantiate(trackClip.asset);

                    foreach (FieldInfo assetField in assetFields) {

                        assetField.SetValue(pastedClip.asset, assetField.GetValue(trackClipAssetCopy));
                    }
                }
            }

            // Set groups if applicable, inserting the duplicated tracks into the group beside the source
            for (int i = 0; i < sourceTrackData.Count; i++) {
                if (sourceTrackData[i].groupTrack != null) {

                    List<TrackAsset> revisedTrackList = new List<TrackAsset>();
                    revisedTrackList.AddRange(sourceTrackData[i].groupTrack.GetChildTracks());

                    for (int q = 0; q < revisedTrackList.Count; q++) {
                        if (revisedTrackList[q] == sourceTrackData[i].trackAsset) {
                            revisedTrackList.Insert(q + 1, pastedTracks[i]);
                        }
                    }

                    for (int z = 0; z < revisedTrackList.Count; z++) {
                        revisedTrackList[z].SetGroup(null);
                        revisedTrackList[z].SetGroup(sourceTrackData[i].groupTrack);
                    }
                }
            }

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

        [MenuItem("Edit/AltSalt/Create Color Track", false, 0)]
        public static void HotkeyCreateColorTrack()
        {
            bool selectCreatedTracks = trackClipWindow.selectCreatedTracks;

            List<TrackAsset> newTracks = new List<TrackAsset>();
            Type[] types = { typeof(TMP_Text), typeof(SpriteRenderer) };

            UnityEngine.Object[] culledSelection = Utils.CullSelection(Selection.gameObjects, types);
            GameObject[] selectionAsGameObjects = Array.ConvertAll(culledSelection, item => (GameObject)item);
            GameObject[] sortedSelection = Utils.SortGameObjectSelection(selectionAsGameObjects);

            for (int i=0; i< sortedSelection.Length; i++) {
                if(Utils.TargetComponentSelected(sortedSelection[i], typeof(TMP_Text))) {
                    newTracks.AddRange(TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new GameObject[] { sortedSelection[i] }, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips));
                }

                if (Utils.TargetComponentSelected(sortedSelection[i], typeof(SpriteRenderer))) {
                    newTracks.AddRange(TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, new GameObject[] { sortedSelection[i] }, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects, TimelineEditor.selectedClips));
                }
            }

            if(selectCreatedTracks == true) {
                Selection.objects = newTracks.ToArray();
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
        }

        [MenuItem("Edit/AltSalt/Create Position Track", false, 0)]
        public static void HotkeyCreatePositionTrack()
        {
            bool selectCreatedTracks = trackClipWindow.selectCreatedTracks;

            if(selectCreatedTracks == true) {
                Selection.objects = CreateResponsiveRectTransformPosTrack();
            } else {
                CreateResponsiveRectTransformPosTrack();
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
        }

        [MenuItem("Edit/AltSalt/Create New Clip(s)", false, 0)]
        public static void HotkeyTriggerCreateClips()
        {
            bool selectCreatedClip = trackClipWindow.selectCreatedClip;
            bool advancePlayhead = trackClipWindow.advancePlayhead;
            float newClipDuration = trackClipWindow.newClipDuration;
            string clipName = trackClipWindow.clipName;
            EasingFunction.Ease clipEaseType = trackClipWindow.clipEaseType;

            TriggerCreateClips(selectCreatedClip, advancePlayhead, newClipDuration, clipName, clipEaseType);
        }

        public static TrackAsset[] CreateTMProColorTrack()
        {
            return TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(TMProColorTrack), typeof(TMP_Text), Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateSpriteColorTrack()
        {
            return TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(SpriteColorTrack), typeof(SpriteRenderer), Selection.objects, TimelineEditor.selectedClips);
        }

        public static TrackAsset[] CreateResponsiveRectTransformPosTrack()
        {
            return TriggerCreateTrack(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.gameObjects, typeof(ResponsiveRectTransformPosTrack), typeof(RectTransform), Selection.objects, TimelineEditor.selectedClips);
        }

        public static void TriggerCreateClips(bool selectCreatedClip, bool advancePlayhead, float newClipDuration, string clipName, EasingFunction.Ease clipEaseType)
        {
            if (selectCreatedClip == true) {
                TimelineEditor.selectedClips = CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName, clipEaseType);
            } else {
                CreateClips(TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, newClipDuration, clipName, clipEaseType);
            }
            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();

            if (advancePlayhead == true) {
                TimelineUtilsCore.CurrentTime += newClipDuration;
                TimelineUtilsCore.RefreshTimelineRedrawWindow();
            }
        }

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] sourceObjects, Type trackType, Type requiredObjectType, UnityEngine.Object[] destinationSelection, TimelineClip[] clipSelection)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);
            if (parentTrack == null) {
                parentTrack = GetDestinationTrackFromSelection(clipSelection);
            }

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

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, GameObject[] sourceGameObjects, Type trackType, Type requiredComponentType, UnityEngine.Object[] destinationSelection, TimelineClip[] clipSelection)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);
            if(parentTrack == null) {
                parentTrack = GetDestinationTrackFromSelection(clipSelection);
            }

            if (sourceGameObjects.Length > 0) {
                GameObject[] sortedGameObjects = Utils.SortGameObjectSelection(sourceGameObjects);
                for (int i = 0; i < sortedGameObjects.Length; i++) {
                    if (requiredComponentType != null && Utils.TargetComponentSelected(sortedGameObjects[i], requiredComponentType) == true) {
                        TrackAsset newTrack = CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
                        trackAssets.Add(newTrack);
                        PopulateTrackAsset(targetDirector, newTrack, sortedGameObjects[i]);
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
                newClip.start = TimelineUtilsCore.CurrentTime;
                newClip.duration = duration;
                PopulateClip(targetDirector, targetTracks[i], easeType, newClip);
                timelineClips.Add(newClip);
            }

            TimelineClip[] timelineClipsArray = timelineClips.ToArray();

            if (clipName.Length > 0) {
                TimelineUtilsCore.RenameClips(clipName, timelineClipsArray);
            }

            TimelineUtilsCore.RefreshTimelineContentsAddedOrRemoved();
            return timelineClipsArray;
        }

        static PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack, EasingFunction.Ease easeType, TimelineClip timelineClip)
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
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformPosClip): {
                        RectTransformPosClip asset = timelineClip.asset as RectTransformPosClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialPosition = component.anchoredPosition3D;
                            asset.template.targetPosition = component.anchoredPosition3D;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(SpriteColorClip): {
                        SpriteColorClip asset = timelineClip.asset as SpriteColorClip;
                        SpriteRenderer component = sourceObject as SpriteRenderer;
                        if (component != null) {
                            asset.template.initialColor = component.color;
                            asset.template.targetColor = component.color;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformScaleClip): {
                        RectTransformScaleClip asset = timelineClip.asset as RectTransformScaleClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialScale = component.localScale;
                            asset.template.targetScale = component.localScale;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(RectTransformRotationClip): {
                        RectTransformRotationClip asset = timelineClip.asset as RectTransformRotationClip;
                        RectTransform component = sourceObject as RectTransform;
                        if (component != null) {
                            asset.template.initialRotation = component.localEulerAngles;
                            asset.template.targetRotation = component.localEulerAngles;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(LerpFloatVarClip): {
                        LerpFloatVarClip asset = timelineClip.asset as LerpFloatVarClip;
                        FloatVariable component = sourceObject as FloatVariable;
                        if (component != null) {
                            asset.template.initialValue = component.value;
                            asset.template.targetValue = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(LerpColorVarClip): {
                        LerpColorVarClip asset = timelineClip.asset as LerpColorVarClip;
                        ColorVariable component = sourceObject as ColorVariable;
                        if (component != null) {
                            asset.template.initialColor = component.value;
                            asset.template.targetColor = component.value;
                        }
                        asset.template.ease = easeType;
                        return asset;
                    }

                case nameof(TMProCharSpacingClip): {
                        TMProCharSpacingClip asset = timelineClip.asset as TMProCharSpacingClip;
                        TMP_Text component = sourceObject as TMP_Text;
                        if (component != null) {
                            asset.template.initialValue = component.characterSpacing;
                            asset.template.targetValue = component.characterSpacing;
                        }
                        asset.template.ease = easeType;
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
                        asset.template.ease = easeType;
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

        static TrackAsset GetDestinationTrackFromSelection(TimelineClip[] destinationSelection)
        {
            TrackAsset destinationTrack = null;
            for (int q = 0; q < destinationSelection.Length; q++) {
                TimelineClip clip = destinationSelection[q];
                if (clip.parentTrack.parent is TrackAsset) {
                    destinationTrack = clip.parentTrack.parent as TrackAsset;
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

        public static ConfigMarker TriggerCreateMarker(MarkerTrack markerTrack, Type markerType, double targetTime)
        {
            ConfigMarker configMarker = null;
            
            switch (markerType.Name) {

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

            return configMarker;
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
