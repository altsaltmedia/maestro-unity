using System;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    public class TimelineAssetManipulation : ModuleWindow
    {
        private static FloatField currentTimeField;

        private static TimelineAssetManipulation _timelineAssetManipulation;

        private static TimelineAssetManipulation timelineAssetManipulation
        {
            get => _timelineAssetManipulation;
            set => _timelineAssetManipulation = value;
        }

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            timelineAssetManipulation = this;
            
            var propertyFields = moduleWindowUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            currentTimeField = moduleWindowUXML.Query<FloatField>("CurrentTime");

            UpdateDisplay();

            ControlPanel.selectionChangedDelegate += UpdateDisplay;
            ControlPanel.inspectorUpdateDelegate += UpdateTime;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
            ControlPanel.inspectorUpdateDelegate -= UpdateTime;
        }

        static VisualElementToggleData toggleData = new VisualElementToggleData();
        
        public int selectionCount = 1;
        public bool selectConfigMarkers = true;
        public bool transposeUnselectedAssets = false;
        public float durationMultiplier = 1;
        public float targetDuration = 1;
        public float targetSpacing = 0;
        
        public delegate TimelineClip[] TransposeClipsCallback(TimelineClip[] selectedClips, TimelineClip[] sourceClips, double offset, double timeReference);
        
        public delegate Marker[] TransposeMarkersCallback(Marker[] selectedMarkers, Marker[] sourceMarkers, double offset, double timeReference);

        enum PropertyFieldNames
        {
            DurationMultiplier,
            TargetDuration,
            TargetSpacing
        }
        
        enum ButtonNames
        {
            SelectEndingBefore,
            SelectStartingBefore,
            SelectEndingAfter,
            SelectStartingAfter,
            AddPrevClipToSelection,
            DecrementSelectionCount,
            IncrementSelectionCount,
            AddNextClipToSelection,
            SetToPlayhead,
            TransposeToPlayhead,
            ResizeToPlayhead,
            ResizeAndTransposeToPlayhead,
            MultiplyDuration,
            MultiplyDurationAndTranspose,
            SetDuration,
            SetDurationAndTranspose,
            SetSpacing,
            AddSubtractSpacing,
            SetSequentialOrder,
            SetSequentialOrderReverse,
            DeselectAll,
            RefreshTimelineWindow,
            SelectSourceObjects,
            SelectTargetTracks
        }

        enum EnableCondition
        {
            ClipsSelected,
            ClipsOrMarkersSelected,
            TrackOrClipSelected,
            ClipSelected,
            ObjectsSelected
        }

        private static void UpdateTime()
        {
            if (currentTimeField != null && TimelineUtils.debugTrackCreated == true) {
                currentTimeField.value = TimelineUtils.currentTime;
            }
        }

        private static void UpdateDisplay()
        {
            if (TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipsSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipsSelected, false);
            }
            
            if (TimelineEditor.selectedClips.Length > 0 || Utils.TargetTypeSelected(Selection.objects, typeof(Marker)) == true) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipsOrMarkersSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipsOrMarkersSelected, false);
            }

            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset)) == true || TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackOrClipSelected, false);
            }

            if (Utils.CullSelection(Selection.objects, typeof(TrackAsset)).Length > 0 || TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectsSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectsSelected, false);
            }

            if (TimelineEditor.selectedClips.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ClipSelected, false);
            }
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.DurationMultiplier):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (evt.newValue < .1f) {
                            durationMultiplier = .1f;
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.TargetDuration):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (evt.newValue < .1f) {
                            targetDuration = .1f;
                        }
                        TimelineEditor.selectedClips = SetDuration(GetOrderedClipSelection(), targetDuration, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;

                case nameof(PropertyFieldNames.TargetSpacing):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        TimelineEditor.selectedClips = SetSpacing(GetOrderedClipSelection(), targetSpacing, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    });
                    break;
                
            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.SelectEndingBefore):
                    button.clickable.clicked += () =>
                    {
                        List<Object> newSelection = new List<Object>();
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsEndingBefore(Selection.objects, TimelineUtils.currentTime);
                        
                        newSelection.AddRange(Selection.objects);
                        
                        if (selectConfigMarkers == true) {
                            newSelection.AddRange(
                                TimelineUtils.SelectMarkersEndingBefore(Selection.objects, TimelineUtils.currentTime));
                        }

                        Selection.objects = newSelection.ToArray();

                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingBefore):
                    button.clickable.clicked += () =>
                    {
                        List<Object> newSelection = new List<Object>();
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsStartingBefore(Selection.objects, TimelineUtils.currentTime);
                        
                        newSelection.AddRange(Selection.objects);
                        
                        if (selectConfigMarkers == true) {
                            newSelection.AddRange(
                                TimelineUtils.SelectMarkersEndingBefore(Selection.objects, TimelineUtils.currentTime));
                        }
                        
                        Selection.objects = newSelection.ToArray();
                        
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectEndingAfter):
                    button.clickable.clicked += () => {
                        List<Object> newSelection = new List<Object>();
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsEndingAfter(Selection.objects, TimelineUtils.currentTime);
                        
                        newSelection.AddRange(Selection.objects);
                        
                        if (selectConfigMarkers == true) {
                            newSelection.AddRange(
                                TimelineUtils.SelectMarkersStartingAfter(Selection.objects, TimelineUtils.currentTime));
                        }
                        
                        Selection.objects = newSelection.ToArray();

                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SelectStartingAfter):
                    button.clickable.clicked += () => {
                        List<Object> newSelection = new List<Object>();
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsStartingAfter(Selection.objects, TimelineUtils.currentTime);
                        
                        newSelection.AddRange(Selection.objects);
                        
                        if (selectConfigMarkers == true) {
                            newSelection.AddRange(
                                TimelineUtils.SelectMarkersStartingAfter(Selection.objects, TimelineUtils.currentTime));
                        }
                        
                        Selection.objects = newSelection.ToArray();
                        
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.AddPrevClipToSelection):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = AddPrevClipToSelection(TimelineEditor.selectedClips, TimelineUtils.currentTime, selectionCount);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.DecrementSelectionCount):
                    button.clickable.clicked += () => {
                        selectionCount--;
                        if (selectionCount < 1) {
                            selectionCount = 1;
                        }
                    };
                    break;

                case nameof(ButtonNames.IncrementSelectionCount):
                    button.clickable.clicked += () => {
                        selectionCount++;
                    };
                    break;

                case nameof(ButtonNames.AddNextClipToSelection):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = AddNextClipToSelection(TimelineEditor.selectedClips, TimelineUtils.currentTime, selectionCount);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    break;

                case nameof(ButtonNames.SetToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetToPlayhead(GetOrderedClipSelection(), TimelineUtils.currentTime, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.TransposeToPlayhead):
                    button.clickable.clicked += () =>
                    {
                        Marker[] markerSelection = GetOrderedMarkerSelection();
                        float timeDelta = GetDeltaBetweenPlayheadAndSelection(TimelineEditor.selectedClips,
                            markerSelection, TimelineUtils.currentTime, out object firstAsset);

                        List<Object> newSelection = new List<Object>();
                        
                        TimelineEditor.selectedClips = TransposeClipsToPlayhead(GetOrderedClipSelection(), 
                            timeDelta, TimelineUtils.currentTime, firstAsset is TimelineClip);

                        newSelection.AddRange(Selection.objects);
                        newSelection.AddRange(TransposeMarkersToPlayhead(markerSelection, 
                            timeDelta, TimelineUtils.currentTime, firstAsset is Marker));
                        
                        Selection.objects = newSelection.ToArray();

                        if (transposeUnselectedAssets == true) {
                            TransposeTargetClips(TimelineEditor.selectedClips, TimelineUtils.GetAllTimelineClips(), timeDelta,
                                TimelineUtils.currentTime - timeDelta);
                            TransposeTargetMarkers(markerSelection, TimelineUtils.GetAllMarkers(), timeDelta,
                                TimelineUtils.currentTime - timeDelta);
                        }

                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsOrMarkersSelected, button);
                    break;

                case nameof(ButtonNames.ResizeToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ResizeToPlayhead(GetOrderedClipSelection(), TimelineUtils.currentTime, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.ResizeAndTransposeToPlayhead):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = ResizeAndTransposeToPlayhead(GetOrderedClipSelection(), TimelineUtils.currentTime, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips());
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.MultiplyDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = MultiplyDuration(GetOrderedClipSelection(), durationMultiplier, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.MultiplyDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = MultiplyDurationAndTranspose(GetOrderedClipSelection(), durationMultiplier, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips());
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.SetDuration):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetDuration(GetOrderedClipSelection(), targetDuration, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.SetDurationAndTranspose):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetDurationAndTranspose(GetOrderedClipSelection(), targetDuration, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.SetSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetSpacing(GetOrderedClipSelection(), targetSpacing, transposeUnselectedAssets, TimelineUtils.GetAllTimelineClips(), TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.AddSubtractSpacing):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = AddSubtractSpacing(GetOrderedClipSelection(), TimelineUtils.GetAllTimelineClips(), targetSpacing);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.SetSequentialOrder):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetSequentialOrder(GetOrderedClipSelection(), TimelineUtils.GetAllTimelineClips(), transposeUnselectedAssets, TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.SetSequentialOrderReverse):
                    button.clickable.clicked += () => {
                        TimelineEditor.selectedClips = SetSequentialOrderReverse(GetOrderedClipSelection(), TimelineUtils.GetAllTimelineClips(), transposeUnselectedAssets, TransposeTargetClips);
                        TimelineUtils.RefreshTimelineContentsModified();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ClipsSelected, button);
                    break;

                case nameof(ButtonNames.RefreshTimelineWindow):
                    button.clickable.clicked += () => {
                        TimelineUtils.RefreshTimelineWindow();
                    };
                    break;

                case nameof(ButtonNames.DeselectAll):
                    button.clickable.clicked += () => {
                        ModuleUtils.DeselectAll();
                    };
                    break;

                case nameof(ButtonNames.SelectSourceObjects):
                    button.clickable.clicked += () => {
                        SelectSourceObjects();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackOrClipSelected, button);
                    break;

                case nameof(ButtonNames.SelectTargetTracks):
                    button.clickable.clicked += () => {
                        SelectTargetTracks();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ObjectsSelected, button);
                    break;
            }

            return button;
        }

        [MenuItem("Edit/Maestro/Timeline/Select Source Objects", false, 0)]
        public static void SelectSourceObjects()
        {
            Selection.objects = GetObjectsFromTimelineSelection(Selection.objects, TimelineEditor.selectedClips, TimelineEditor.inspectedDirector);
            TimelineUtils.RefreshTimelineContentsModified();
        }

        [MenuItem("Edit/Maestro/Timeline/Select Target Tracks", false, 0)]
        public static void SelectTargetTracks()
        {
            Selection.objects = SelectTargetTracks(Selection.objects, TimelineEditor.selectedClips, TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector);
            TimelineUtils.RefreshTimelineContentsModified();
        }

        public static TimelineClip[] AddPrevClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip previousClip = GetPreviousClip(newSelection, TimelineUtils.GetAllTimelineClips(), timeReference);
                newSelection.Add(previousClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetPreviousClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip previousClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if (currentSelectionList.Contains(clipToCompare) || clipToCompare.start > timeReference) {
                    continue;
                }

                if (previousClip == null) {
                    previousClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtils.currentTime) < Mathf.Abs((float)previousClip.start - TimelineUtils.currentTime)) {
                    previousClip = clipToCompare;
                }

            }

            return previousClip;
        }

        public static TimelineClip[] AddNextClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip nextClip = GetNextClip(newSelection, TimelineUtils.GetAllTimelineClips(), timeReference);
                newSelection.Add(nextClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetNextClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip nextClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if (currentSelectionList.Contains(clipToCompare) || clipToCompare.end < timeReference) {
                    continue;
                }

                if (nextClip == null) {
                    nextClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtils.currentTime) < Mathf.Abs((float)nextClip.start - TimelineUtils.currentTime)) {
                    nextClip = clipToCompare;
                }

            }

            return nextClip;
        }

        public static TimelineClip[] SetToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips);

            double startTime = timeReference;
            double difference = 0;

            if (selectedClips.Length > 0) {
                startTime = selectedClips[0].start;
                difference = timeReference - selectedClips[0].start;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "set clip(s) start time");
                selectedClips[i].start = timeReference;
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, startTime);
            }

            return selectedClips;
        }

        public static float GetDeltaBetweenPlayheadAndSelection(TimelineClip[] selectedClips, Marker[] selectedMarkers, float timeReference, out object firstAsset)
        {
            firstAsset = null;
            double firstAssetStartTime = 0;

            if (selectedClips.Length > 0) {
                firstAsset = selectedClips[0];
                firstAssetStartTime = selectedClips[0].start;
            } else if (selectedMarkers.Length > 0) {
                firstAsset = selectedMarkers[0];
                firstAssetStartTime = selectedMarkers[0].time;
            }
            else {
                return timeReference;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                if (selectedClips[i].start < firstAssetStartTime) {
                    firstAsset = selectedClips[i];
                    firstAssetStartTime = selectedClips[i].start;
                }
            }

            for (int i = 0; i < selectedMarkers.Length; i++) {
                if (selectedMarkers[i].time < firstAssetStartTime) {
                    firstAsset = selectedMarkers[i];
                    firstAssetStartTime = selectedMarkers[i].time;
                }
            }

            return timeReference - (float)firstAssetStartTime;
        }

        public static TimelineClip[] TransposeClipsToPlayhead(TimelineClip[] selectedClips, float timeDelta, float newStartTime, bool setFirstClipStartTime)
        {
            selectedClips = SortClips(selectedClips);

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "transpose clip(s) to start time");
                if (i == 0 && setFirstClipStartTime == true) {
                    selectedClips[i].start = newStartTime;
                }

                if (selectedClips[i].start == newStartTime) {
                    continue;
                }

                selectedClips[i].start += timeDelta;
            }

            // if (executeTranposeCallback == true) {
            //     transposeClipsCallback(selectedClips, sourceClips, timeDelta, newStartTime);
            // }

            return selectedClips;
        }
        
        public static Marker[] TransposeMarkersToPlayhead(Marker[] selectedMarkers, float timeDelta, float newStartTime,  bool setFirstMarkerStartTime)
        {
            selectedMarkers = SortMarkers(selectedMarkers);

            for (int i = 0; i < selectedMarkers.Length; i++) {
                Undo.RecordObject(selectedMarkers[i], "transpose marker(s) to start time");
                if (i == 0 && setFirstMarkerStartTime == true) {
                    selectedMarkers[i].time = newStartTime;
                }
                
                if (selectedMarkers[i].time == newStartTime) {
                    continue;
                }

                selectedMarkers[i].time += timeDelta;
            }

            // if (executeTranposeCallback == true) {
            //     transposeMarkersCallback(selectedMarkers, sourceMarkers, timeDelta, newStartTime);
            //     transposeTargetClipsCallback(TimelineEditor.selectedClips, TimelineUtils.GetAllTimelineClips(),
            //         timeDelta, newStartTime);
            // }

            return selectedMarkers;
        }


        public static TimelineClip[] ResizeToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            double smallestDifference = 0;
            double originalEnd = 0;

            if (selectedClips.Length > 0) {
                originalEnd = selectedClips[0].end;
            }

            for (int i = 0; i < selectedClips.Length; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "expand clip(s) to playhead");
                if (timeReference > selectedClips[i].start) {
                    double difference = timeReference - selectedClips[i].end;
                    selectedClips[i].duration += difference;

                    if (smallestDifference.Equals(0) || Mathf.Abs((float)difference) < Mathf.Abs((float)smallestDifference)) {
                        smallestDifference = difference;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, smallestDifference, originalEnd);
            }

            return selectedClips;
        }

        public static TimelineClip[] ResizeAndTransposeToPlayhead(TimelineClip[] selectedClips, float timeReference, bool executeTransposeUnselectedClips = false, TimelineClip[] sourceClips = null)
        {
            selectedClips = SortClips(selectedClips);
            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;
                double difference = timeReference - selectedClip.end;

                Undo.RecordObject(selectedClip.parentTrack, "expand and transpose clip(s) to playhead");
                if (timeReference > selectedClip.start) {
                    selectedClips[i].duration += difference;
                }

                if (executeTransposeUnselectedClips == true) {

                    double adjustedDifference = difference;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && Equals(selectedClip.start, selectedClips[i - 1].start)) {
                        adjustedDifference -= previousDifference;
                    }

                    for (int q = 0; q < sourceClips.Length; q++) {

                        TimelineClip clip = sourceClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "expand and transpose clip(s) to playhead");
                            clip.start += adjustedDifference;
                        }
                    }
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] MultiplyDuration(TimelineClip[] selectedClips, float multiplier, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply clip(s)");
                selectedClip.duration *= multiplier;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] MultiplyDurationAndTranspose(TimelineClip[] selectedClips, float multiplier, bool executeTranposeUnselectedClips = false, TimelineClip[] sourceClips = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {
                TimelineClip selectedClip = selectedClips[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply and transpose clip(s)");
                selectedClip.duration *= multiplier;
                double difference = selectedClip.duration - originalDuration;

                if (executeTranposeUnselectedClips == false) {

                    for (int q = 0; q < selectedClips.Length; q++) {

                        TimelineClip clip = selectedClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "multiply and transpose clip(s)");
                            clip.start += difference;
                        }
                    }
                } else {

                    double adjustedDifference = difference;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && Equals(selectedClip.start, selectedClips[i - 1].start)) {
                        adjustedDifference -= previousDifference;
                    }

                    for (int q = 0; q < sourceClips.Length; q++) {

                        TimelineClip clip = sourceClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "multiply and transpose clip(s)");
                            clip.start += adjustedDifference;
                        }
                    }
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetDuration(TimelineClip[] selectedClips, float duration, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            double previousDifference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set clip(s) duration");
                selectedClip.duration = duration;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetDurationAndTranspose(TimelineClip[] selectedClips, float duration, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            selectedClips = SortClips(selectedClips);

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set duration and transpose clip(s)");
                selectedClip.duration = duration;

                double difference = selectedClip.duration - originalDuration;

                if (executeTranposeCallback == false) {

                    for (int q = 0; q < selectedClips.Length; q++) {

                        TimelineClip clip = selectedClips[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "set duration and transpose clip(s)");
                            clip.start += difference;
                        }
                    }

                } else {
                    TimelineClip[] selectedClipArray = { selectedClip };
                    transposeClipsCallback(selectedClipArray, sourceClips, difference, selectedClip.start);
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSpacing(TimelineClip[] selectedClips, float spacing, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double endTimePlusSpacing = selectedClip.end + spacing;

                for (int q = 0; q < selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    double initialStartTime = clip.start;

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clip(s) spacing");
                        clip.start = endTimePlusSpacing;

                        if (executeTranposeCallback == true) {
                            double difference = clip.start - initialStartTime;
                            transposeClipsCallback(selectedClips, sourceClips, difference, initialStartTime);
                        }
                    }
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] AddSubtractSpacing(TimelineClip[] selectedClips, TimelineClip[] sourceClips, float spacing)
        {

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                for (int q = 0; q < sourceClips.Length; q++) {

                    TimelineClip clip = sourceClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "add / subtract clip(s) spacing");
                        clip.start += spacing;
                    }
                }
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSequentialOrder(TimelineClip[] selectedClips, TimelineClip[] sourceClips, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips, new TimelineUtils.ClipTimeSequentialSort(TimelineUtils.GetAllTracks()));

            double difference = 0;

            for (int i = 0; i < selectedClips.Length; i++) {

                TimelineClip selectedClip = selectedClips[i];

                double newStartTime = selectedClip.end;

                for (int q = 0; q < selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == 0 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, selectedClips[0].end);
            }

            return selectedClips;
        }

        public static TimelineClip[] SetSequentialOrderReverse(TimelineClip[] selectedClips, TimelineClip[] sourceClips, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            selectedClips = SortClips(selectedClips, new TimelineUtils.ClipTimeSequentialSort(TimelineUtils.GetAllTracks()));

            double difference = 0;

            for (int i = selectedClips.Length - 1; i >= 0; i--) {

                TimelineClip selectedClip = selectedClips[i];

                double newStartTime = selectedClip.end;

                for (int q = 0; q < selectedClips.Length; q++) {

                    TimelineClip clip = selectedClips[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == selectedClips.Length - 1 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially (reverse)");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, selectedClips[selectedClips.Length - 1].end);
            }

            return selectedClips;
        }

        public static TimelineClip[] TransposeTargetClips(TimelineClip[] omittedClips, TimelineClip[] targetClips, double offset, double timeReference)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            if (omittedClips != null) {
                selectedClipsList.AddRange(omittedClips);
            }

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();

            for (int i = 0; i < targetClips.Length; i++) {

                TimelineClip sourceClip = targetClips[i];

                if (selectedClipsList.Contains(sourceClip)) {
                    continue;
                }

                if (sourceClip.start > timeReference || sourceClip.start.Equals(timeReference)) {
                    Undo.RecordObject(sourceClip.parentTrack, "transpose target clip(s)");
                    sourceClip.start += offset;
                    sourceClipsList.Add(sourceClip);
                }
            }

            return sourceClipsList.ToArray();
        }
        
        public static Marker[] TransposeTargetMarkers(Marker[] omittedClips, Marker[] targetMarkers, double offset, double timeReference)
        {
            List<Marker> selectedMarkersList = new List<Marker>();
            if (omittedClips != null) {
                selectedMarkersList.AddRange(omittedClips);
            }

            List<Marker> sourceMarkersList = new List<Marker>();

            for (int i = 0; i < targetMarkers.Length; i++) {

                Marker sourceMarker = targetMarkers[i];

                if (selectedMarkersList.Contains(sourceMarker)) {
                    continue;
                }

                if (sourceMarker.time > timeReference || sourceMarker.time.Equals(timeReference)) {
                    Undo.RecordObject(sourceMarker, "transpose target clip(s)");
                    sourceMarker.time += offset;
                    sourceMarkersList.Add(sourceMarker);
                }
            }

            return sourceMarkersList.ToArray();
        }
        
        public static TimelineClip[] GetOrderedClipSelection()
        {
            return GetOrderedClipSelection(new Utils.ClipTimeSort());
        }

        public static TimelineClip[] GetOrderedClipSelection(Comparer<TimelineClip> comparer)
        {
            TimelineClip[] currentSelection = TimelineEditor.selectedClips;
            System.Array.Sort(currentSelection, comparer);
            return currentSelection;
        }
        
        public static Marker[] GetOrderedMarkerSelection()
        {
            return GetOrderedMarkerSelection(new Utils.MarkerTimeSort());
        }

        public static Marker[] GetOrderedMarkerSelection(Comparer<Marker> comparer)
        {
            Marker[] currentSelection = Array.ConvertAll(Utils.FilterSelection(Selection.objects, typeof(Marker)), x => (Marker)x);
            System.Array.Sort(currentSelection, comparer);
            return currentSelection;
        }

        public static TimelineClip[] SortClips(TimelineClip[] clips)
        {
            System.Array.Sort(clips, new Utils.ClipTimeSort());
            return clips;
        }

        public static TimelineClip[] SortClips(TimelineClip[] clips, Comparer<TimelineClip> comparer)
        {
            System.Array.Sort(clips, comparer);
            return clips;
        }
        
        public static Marker[] SortMarkers(Marker[] markers)
        {
            System.Array.Sort(markers, new Utils.MarkerTimeSort());
            return markers;
        }

        public static Marker[] SortMarkers(Marker[] markers, Comparer<Marker> comparer)
        {
            System.Array.Sort(markers, comparer);
            return markers;
        }

        public static UnityEngine.Object[] GetObjectsFromTimelineSelection(UnityEngine.Object[] objectSelection, TimelineClip[] clipSelection, PlayableDirector sourceDirector)
        {
            List<UnityEngine.Object> newObjectSelection = new List<UnityEngine.Object>();

            List<TrackAsset> trackAssets = new List<TrackAsset>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if (objectSelection[i] is TrackAsset) {
                    trackAssets.Add(objectSelection[i] as TrackAsset);
                }
            }

            for (int i = 0; i < clipSelection.Length; i++) {
                TrackAsset trackAsset = clipSelection[i].parentTrack;
                if (trackAssets.Contains(trackAsset) == false) {
                    trackAssets.Add(trackAsset);
                }
            }

            return GetObjectsFromTrackSelection(trackAssets, sourceDirector);
        }

        public static UnityEngine.Object GetObjectFromTrackSelection(TrackAsset trackAsset, PlayableDirector sourceDirector)
        {
            UnityEngine.Object sourceObject = null;

            foreach (PlayableBinding playableBinding in trackAsset.outputs) {
                UnityEngine.Object objectBinding = sourceDirector.GetGenericBinding(playableBinding.sourceObject);
                if (objectBinding is Component) {
                    sourceObject = (objectBinding as Component).gameObject;
                } else {
                    sourceObject = objectBinding;
                }
            }

            return sourceObject;
        }
        
        public static UnityEngine.Object[] GetObjectsFromTrackSelection(List<TrackAsset> trackAssets, PlayableDirector sourceDirector)
        {
            List<UnityEngine.Object> newObjectSelection = new List<UnityEngine.Object>();
            
            for (int i = 0; i < trackAssets.Count; i++) {

                foreach (PlayableBinding playableBinding in trackAssets[i].outputs) {
                    UnityEngine.Object objectBinding = sourceDirector.GetGenericBinding(playableBinding.sourceObject);
                    if (objectBinding is Component) {
                        newObjectSelection.Add((objectBinding as Component).gameObject);
                    } else {
                        newObjectSelection.Add(objectBinding);
                    }
                }
            }

            return newObjectSelection.ToArray();
        }

        public static UnityEngine.Object[] SelectTargetTracks(UnityEngine.Object[] objectSelection, TimelineClip[] clipSelection, TimelineAsset sourceTimelineAsset, PlayableDirector sourceDirector)
        {
            UnityEngine.Object[] originalSelection = objectSelection;
            objectSelection = TimelineUtils.GetAssociatedTracksFromSelection(objectSelection, clipSelection, sourceTimelineAsset, sourceDirector);

            if (objectSelection.Length > 0) {
                return objectSelection;
            } else {
                EditorUtility.DisplayDialog("No track(s) found", "There are no tracks associated with the object(s) selected", "Okay");
                return originalSelection;
            }
        }

        //public static UnityEngine.Object[] ShowTargetGroupTracks(TrackAsset[] trackAssets)
        //{

        //}

        //public static UnityEngine.Object[] HideTargetGroupTracks(TrackAsset[] trackAssets)
        //{
        //    List<GroupTrack> groupTracks = new List<GroupTrack>();
        //    for(int i=0; i<trackAssets.Length; i++) {
        //        if(trackAssets[i] is GroupTrack) {
        //            groupTracks.Add(trackAssets[i] as GroupTrack);
        //        }
        //    }

        //    for(int i=0; i<groupTracks.Count; i++) {
        //        Selection.activeObject = groupTracks[i];

        //    }
        //}

    }
}
