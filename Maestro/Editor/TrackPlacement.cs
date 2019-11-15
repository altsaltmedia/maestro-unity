using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;

namespace AltSalt.Maestro
{
    public class TrackPlacement : ModuleWindow
    {
        private static TrackPlacement trackPlacementWindow;
        
        public delegate void AllowBlankTracksChangedDelegate();
        public static AllowBlankTracksChangedDelegate allowBlankTracksChangedDelegate = () => { };
        
        public delegate TrackAsset TriggerCreateTrackDelegate(PlayableDirector targetDirector, TrackAsset targetTrack, UnityEngine.Object targetObject);
        public static TriggerCreateTrackDelegate triggerCreateTrackDelegate =
            (targetDirector, targetTrack, targetObject) => targetTrack;

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            trackPlacementWindow = this;

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            var updateWindowTriggers = moduleWindowUXML.Query<VisualElement>(null, ModuleUtils.updateWindowTrigger);
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        public static List<TrackData> copiedTracks = new List<TrackData>();
        
        public bool allowBlankTracks = false;

        VisualElementToggleData toggleData = new VisualElementToggleData();

        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;

        enum EnableCondition
        {
            TrackSelectedForCopying,
            TrackSelected,
            CopiedTracksPopulated
        };

        enum ButtonNames
        {
            CopyTracks,
            PasteTracks,
            DuplicateTracks,
            GroupTrack
        };

        enum UpdateWindowTriggers
        {
            AllowBlankTracks,
        };

        void UpdateDisplay()
        {
            // The user can force these buttons to enable by toggling allowBlankTracks //
            if (allowBlankTracks == true) {

                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);

            } else {

                if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);
                } else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelected, false);
                }
            }

            // These elements cannot be overriden by allowBlankTracks //

            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForCopying, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForCopying, false);
            }

            if (copiedTracks.Count > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.CopyTracks):
                    button.clickable.clicked += () => {
                        copiedTracks = CopyTracks(TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineUtils.RefreshTimelineContentsModified();
                        TimelineUtils.FocusTimelineWindow();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelectedForCopying, button);
                    break;

                case nameof(ButtonNames.PasteTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = PasteTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, copiedTracks);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.CopiedTracksPopulated, button);
                    break;

                case nameof(ButtonNames.DuplicateTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelectedForCopying, button);
                    break;

                case nameof(ButtonNames.GroupTrack):
                    button.clickable.clicked += () => {
                        if (selectCreatedObject == true) {
                            Selection.activeObject = TriggerCreateGroupTrack(TimelineEditor.inspectedAsset, Selection.objects);
                        } else {
                            TriggerCreateGroupTrack(TimelineEditor.inspectedAsset, Selection.objects);
                        }
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;
            }

            return button;
        }

        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.AllowBlankTracks):
                    visualElement.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) => {
                        allowBlankTracks = evt.newValue;
                        UpdateDisplay();
                        allowBlankTracksChangedDelegate.Invoke();
                    });
                    break;
            }

            return visualElement;
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

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();

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
                        triggerCreateTrackDelegate.Invoke(targetDirector, newTrack, sourceObjects[i]);
                    }
                }
            } else {
                trackAssets.Add(CreateNewTrack(targetTimelineAsset, parentTrack, trackType));
            }
            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
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
                        triggerCreateTrackDelegate.Invoke(targetDirector, newTrack, sortedGameObjects[i]);
                    }
                }
            } else {
                trackAssets.Add(CreateNewTrack(targetTimelineAsset, parentTrack, trackType));
            }
            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
            return trackAssets.ToArray();
        }

        static TrackAsset CreateNewTrack(TimelineAsset targetTimelineAsset, TrackAsset parentTrack, Type trackType)
        {
            TrackAsset trackAsset;
            trackAsset = targetTimelineAsset.CreateTrack(trackType, parentTrack, trackType.Name);
            return trackAsset;
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

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();

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
