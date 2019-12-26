using System;
using System.Reflection;
using System.Collections.Generic;
using Sirenix.Utilities;
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
            GameObjectSelected,
            TrackSelected,
            TrackSelectedForGrouping,
            CopiedTracksPopulated,
            ObjectsAndTracksSelected
        };

        enum ButtonNames
        {
            CopyTracks,
            PasteTracks,
            DuplicateTracks,
            CloneObjectTrackGroups,
            DeepCloneObjectTrackGroups,
            MapSelectionToTracks,
            RemoveTrackBindings,
            GroupTrack
        };

        enum UpdateWindowTriggers
        {
            AllowBlankTracks,
        };

        private void UpdateDisplay()
        {
            // Disable window if the timeline window is inactive
            if (TimelineEditor.inspectedAsset == null) {
                moduleWindowUXML.SetEnabled(false);
            }
            else {
                moduleWindowUXML.SetEnabled(true);
            }

            // The user can force these buttons to enable by toggling allowBlankTracks //
            if (allowBlankTracks == true) {

                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForGrouping, true);

            } else {

                if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForGrouping, true);
                } else {
                    ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelectedForGrouping, false);
                }
            }

            // These elements cannot be overriden by allowBlankTracks //

            if (Selection.gameObjects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, true);
            }
            else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, false);
            }

            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset))) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.TrackSelected, false);
            }

            if (copiedTracks.Count > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.CopiedTracksPopulated, false);
            }
            
            if (Utils.TargetTypeSelected(Selection.objects, typeof(TrackAsset)) &&
                Utils.CullSelection(Selection.objects, typeof(TrackAsset)).Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectsAndTracksSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectsAndTracksSelected, false);
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;

                case nameof(ButtonNames.PasteTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = PasteTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects, TimelineEditor.selectedClips, copiedTracks);
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsStartingAfter(Selection.objects, 0f);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.CopiedTracksPopulated, button);
                    break;

                case nameof(ButtonNames.DuplicateTracks):
                    button.clickable.clicked += () => {
                        Selection.objects = DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects);
                        TimelineEditor.selectedClips = TimelineUtils.SelectClipsStartingAfter(Selection.objects, 0f);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelected, button);
                    break;
                
                case nameof(ButtonNames.CloneObjectTrackGroups):
                    button.clickable.clicked += () =>
                    {
                        Selection.objects = CloneObjectTrackGroups(Selection.gameObjects,
                            TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, out copiedTracks);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;
                
                case nameof(ButtonNames.DeepCloneObjectTrackGroups):
                    button.clickable.clicked += () =>
                    {
                        Selection.objects = DeepCloneObjectTrackGroups(Selection.gameObjects,
                        TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, out copiedTracks);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;
                
                case nameof(ButtonNames.MapSelectionToTracks):
                    button.clickable.clicked += () =>
                    {
                        Selection.objects = MapSelectionToTracks(Selection.objects, TimelineEditor.inspectedDirector);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ObjectsAndTracksSelected, button);
                    break;
                
                case nameof(ButtonNames.RemoveTrackBindings):
                    button.clickable.clicked += () =>
                    {
                        Selection.objects = RemoveTrackBindings(Selection.objects, TimelineEditor.inspectedDirector);
                        TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
                        TimelineUtils.FocusTimelineWindow();
                        UpdateDisplay(); // This will update the status of the PasteTracks button
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelected, button);
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
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.TrackSelectedForGrouping, button);
                    break;
            }

            return button;
        }

        private VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
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
            List<TrackAsset> pastedTracks = new List<TrackAsset>();

            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);
            if (parentTrack == null) {
                parentTrack = GetDestinationTrackFromSelection(clipSelection);
            }

            // Paste tracks
            for (int i = 0; i < sourceTrackData.Count; i++) {
                TrackAsset trackAsset = targetTimelineAsset.CreateTrack(sourceTrackData[i].trackType, parentTrack, sourceTrackData[i].trackName);
                
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
                
                sourceTrackData[i].newTrackAsset = trackAsset; 
                pastedTracks.Add(sourceTrackData[i].newTrackAsset);
            }

            // Set groups if applicable
            for (int i = 0; i < sourceTrackData.Count; i++) {
                if (sourceTrackData[i].groupTrack != null) {
                    for (int q = 0; q < sourceTrackData.Count; q++) {
                        if (sourceTrackData[i].groupTrackID == sourceTrackData[q].trackAsset.GetInstanceID()) {
                            sourceTrackData[i].newTrackAsset.SetGroup(sourceTrackData[q].newTrackAsset as GroupTrack);
                        }
                    }
                }
            }

            TimelineUtils.RefreshTimelineContentsAddedOrRemoved();

            return pastedTracks.ToArray();
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
        
        public GameObject[] DeepCloneObjectTrackGroups(GameObject[] gameObjectSelection, 
            TimelineAsset sourceTimelineAsset, PlayableDirector sourceDirector, out List<TrackData> trackData)
        {
            GameObject[] gameObjectHierarchy = Utils.GetChildGameObjects(Utils.SortGameObjectSelection(gameObjectSelection), true);
            GameObject[] clonedObjects = Utils.DuplicateHierarchy(gameObjectHierarchy);
            trackData = MapClonedObjectsToTrackData(gameObjectHierarchy, clonedObjects, sourceTimelineAsset,
                sourceDirector);
            return clonedObjects;
        }

        public GameObject[] CloneObjectTrackGroups(GameObject[] gameObjectSelection,
            TimelineAsset sourceTimelineAsset, PlayableDirector sourceDirector, out List<TrackData> trackData)
        {
            GameObject[] sortedGameObjects = Utils.SortGameObjectSelection(gameObjectSelection);
            List<GameObject> clonedObjects = new List<GameObject>();
            
            for (int i = 0; i < sortedGameObjects.Length; i++) {
                clonedObjects.Add(Utils.DuplicateGameObject(sortedGameObjects[i]));     
            }

            trackData = MapClonedObjectsToTrackData(sortedGameObjects,
                clonedObjects.ToArray(), sourceTimelineAsset, sourceDirector);

            return clonedObjects.ToArray();
        }

        public List<TrackData> MapClonedObjectsToTrackData(GameObject[] originalObjects, GameObject[] clonedObjects, 
            TimelineAsset sourceTimelineAsset, PlayableDirector sourceDirector)
        {
            List<TrackData> trackData = new List<TrackData>();
            
            UnityEngine.Object[] objectTracks = TimelineUtils.GetAssociatedTracksFromSelection(
                originalObjects, new TimelineClip[] {}, sourceTimelineAsset, sourceDirector);

            TrackAsset[] sortedTracks = TimelineUtils.SortTracks(Array.ConvertAll(objectTracks, x => (TrackAsset) x));

            for (int i = 0; i < sortedTracks.Length; i++) {
                
                TrackAsset currentTrack = sortedTracks[i] as TrackAsset;
                UnityEngine.Object sourceObject = new UnityEngine.Object();
                
                foreach (PlayableBinding playableBinding in currentTrack.outputs) {
                    sourceObject = sourceDirector.GetGenericBinding(playableBinding.sourceObject);
                }
                
                if (currentTrack is GroupTrack) {
                    trackData.Add(new TrackData(currentTrack, sourceObject, currentTrack.GetClips()));
                    continue;
                }
                
                for (int j = 0; j < originalObjects.Length; j++) {

                    if (sourceObject is Component component && originalObjects[j] == component.gameObject) {
                        Type componentType = component.GetType();
                        trackData.Add(new TrackData(currentTrack, clonedObjects[j].GetComponent(componentType), currentTrack.GetClips()));
                    } else if (sourceObject is GameObject gameObject && originalObjects[j] == gameObject) {
                        trackData.Add(new TrackData(currentTrack, clonedObjects[j], currentTrack.GetClips()));
                    }
                }
            }

            return trackData;
        }
        
        public static UnityEngine.Object[] MapSelectionToTracks(UnityEngine.Object[] selection,  PlayableDirector sourceDirector)
        {
            UnityEngine.Object[] objectsToMap = Utils.CullSelection(selection, typeof(TrackAsset));

            UnityEngine.Object[] trackSelectionRaw = Utils.FilterSelection(selection, typeof(TrackAsset));
            TrackAsset[] trackSelection = Array.ConvertAll(trackSelectionRaw, x => (TrackAsset) x);

            if (objectsToMap.Length != trackSelection.Length) {
                Debug.LogError("Number of objects selected must equal number of tracks selected");
                EditorUtility.DisplayDialog("Map unsuccessful",
                    "Number of objects selected must equal number of tracks selected", "Ok");
                return selection;
            }

            if (CanMapObjectsToTracks(objectsToMap, trackSelection, sourceDirector) == true) {
                MapObjectsToTracks(objectsToMap, trackSelection, sourceDirector);
            }

            return trackSelection;
        }

        private static bool CanMapObjectsToTracks(UnityEngine.Object[] selection, TrackAsset[] trackSelection, PlayableDirector sourceDirector)
        {
            UnityEngine.Object[] gameObjectSelection = Utils.FilterSelection(selection, typeof(GameObject));
            
            if (gameObjectSelection.Length > 0 &&
                Utils.CullSelection(selection, typeof(GameObject)).Length > 0) {
                Debug.LogError("Selection cannot contain both game objects and other objects; please map groups separately.");
                EditorUtility.DisplayDialog("Map unsuccessful",
                    "Selection cannot contain both game objects and other objects; please map groups separately.",
                    "Ok");
                return false;
            }
            
            if (gameObjectSelection.Length > 0) {
                selection = Utils.SortGameObjectSelection(Array.ConvertAll(gameObjectSelection, x => (GameObject) x));
            }

            TrackAsset[] sortedTracks = TimelineUtils.SortTracks(trackSelection);
            
            bool canMapTracks = true;

            for (int i = 0; i < sortedTracks.Length; i++) {

                TrackAsset currentTrack = sortedTracks[i];

                if (currentTrack is GroupTrack) {
                    Debug.LogError("Selected track is group track. Group tracks cannot be mapped.");
                    canMapTracks = false;
                    continue;
                }

                Type bindingType = null;

                foreach (PlayableBinding playableBinding in currentTrack.outputs) {
                    bindingType = playableBinding.outputTargetType;
                }

                if (bindingType.IsSubclassOf(typeof(Component))) {
                    
                    UnityEngine.Object currentObject = selection[i];
                    if (currentObject is GameObject == false || (currentObject as GameObject).TryGetComponent(bindingType, out Component component) == false) {
                        Debug.LogError(
                            $"Object {selection[i].name} does not match track {sortedTracks[i]}, aborting operation.",
                            selection[i]);
                        canMapTracks = false;
                    }
                }
                else {
                    if (selection[i].GetType() != bindingType) {
                        Debug.LogError(
                            $"Object {selection[i].name} does not match required object binding for {sortedTracks[i]}, aborting operation.");
                        canMapTracks = false;
                    }
                }
            }

            if (canMapTracks == false) {
                EditorUtility.DisplayDialog("Map unsuccessful", "Please check the console for details.", "Ok");
            }

            return canMapTracks;
        }

        private static TrackAsset[] MapObjectsToTracks(UnityEngine.Object[] selection,
            TrackAsset[] trackSelection, PlayableDirector sourceDirector)
        {
            UnityEngine.Object[] gameObjectSelection = Utils.FilterSelection(selection, typeof(GameObject));
            if (gameObjectSelection.Length > 0) {
                selection = Utils.SortGameObjectSelection(Array.ConvertAll(gameObjectSelection, x => (GameObject) x));
            }

            TrackAsset[] sortedTracks = TimelineUtils.SortTracks(trackSelection);
            
            Undo.RecordObject(sourceDirector, "bind object to track");
            
            for (int i = 0; i < sortedTracks.Length; i++) {

                TrackAsset currentTrack = sortedTracks[i];
                    
                Type bindingType = null;
                PlayableBinding trackBinding;

                foreach (PlayableBinding playableBinding in currentTrack.outputs) {
                    bindingType = playableBinding.outputTargetType;
                    trackBinding = playableBinding;
                }

                if (bindingType.IsSubclassOf(typeof(Component))) {
                    GameObject currentObject = selection[i] as GameObject;
                    sourceDirector.SetGenericBinding(trackBinding.sourceObject, currentObject.GetComponent(bindingType));
                }
                else {
                    sourceDirector.SetGenericBinding(trackBinding.sourceObject, selection[i]);
                }
            }

            return sortedTracks;
        }

        public static TrackAsset[] RemoveTrackBindings(UnityEngine.Object[] selection, PlayableDirector sourceDirector)
        {
            TrackAsset[] selectedTracks =
                Array.ConvertAll(Utils.FilterSelection(selection, typeof(TrackAsset)), x => (TrackAsset) x);

            Undo.RecordObject(sourceDirector, "remove track bindings");
            
            for (int i = 0; i < selectedTracks.Length; i++) {
            
                foreach (PlayableBinding playableBinding in selectedTracks[i].outputs) {
                    sourceDirector.SetGenericBinding(playableBinding.sourceObject, null);
                }
            }

            return selectedTracks;
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
            public int trackPosition;
            public GroupTrack groupTrack;
            public int groupTrackID;
            public Type trackType;
            public string trackName;
            public UnityEngine.Object trackBinding;
            public IEnumerable<TimelineClip> trackClips;

            public TrackAsset newTrackAsset;

            public TrackData(TrackAsset trackAsset, UnityEngine.Object binding, IEnumerable<TimelineClip> trackClips)
            {
                this.trackAsset = trackAsset;
                this.groupTrack = trackAsset.GetGroup();
                if (groupTrack != null) {
                    this.groupTrackID = this.groupTrack.GetInstanceID();
                }
                this.trackType = trackAsset.GetType();
                this.trackName = trackAsset.name;
                this.trackBinding = binding;
                this.trackClips = trackClips;
            }
        }
    }
}
