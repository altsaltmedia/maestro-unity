using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.IO;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing
{
    public class SequencingStructures : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            var updateWindowTriggers = moduleWindowUXML.Query<VisualElement>(null, ModuleUtils.updateWindowTrigger);
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);

            UpdateDisplay();
            ControlPanel.inspectorUpdateDelegate += UpdateDisplay;
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        private void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= UpdateDisplay;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private static VisualElementToggleData toggleData = new VisualElementToggleData();

        public RootDataCollector sequenceControllerObject;
        public MasterSequence _targetSequenceCollection;
        public string sequenceName = "";
        public bool selectOnCreation = true;
        
        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            RootConfig,
            MasterSequencePlusConfig,
            MasterSequence,
            SequenceConfig,
            SimpleDirector,
            Sequence,
            TimelineAsset,
            CloneTimeline,
            SiblingSequenceConfig
        }

        private enum EnableCondition
        {
            MasterSequencePlusDependenciesPopulated,
            MasterSequenceDependenciesPopulated,
            SequenceConfigDependenciesPopulated,
            SimpleDirectorDependenciesPopulated,
            DirectoryAndNamePopulated,
            PlayableDirectorSelected,
            SiblingSequeenceConfigDependenciesPopulated
        }

        private enum UpdateWindowTriggers
        {
            SequenceName
        }

        private void UpdateDisplay()
        {
            if(Utils.FilterSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig)).Length > 0
               && string.IsNullOrEmpty(selectedObjectDirectory) == false
               && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, false);
            }
            
            if(Utils.FilterSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig)).Length > 0
               && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, false);
            }

            if (Utils.FilterSelection(Selection.gameObjects, typeof(MasterSequence)).Length > 0
                && string.IsNullOrEmpty(selectedObjectDirectory) == false
                && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceConfigDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceConfigDependenciesPopulated, false);
            }
            
            if (Selection.transforms.Length > 0 
                && string.IsNullOrEmpty(selectedObjectDirectory) == false
                && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SimpleDirectorDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SimpleDirectorDependenciesPopulated, false);
            }
            
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, false);
            }
            
            if (Utils.FilterSelection(Selection.gameObjects, typeof(PlayableDirector)).Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.PlayableDirectorSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.PlayableDirectorSelected, false);
            }
            
            if (Utils.FilterSelection(Selection.gameObjects, typeof(Sequence_Config)).Length > 0
                && string.IsNullOrEmpty(selectedObjectDirectory) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SiblingSequeenceConfigDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SiblingSequeenceConfigDependenciesPopulated, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.RootConfig):
                    button.clickable.clicked += () => {
                        Selection.activeObject = CreateRootConfig();
                        ModuleUtils.FocusHierarchyWindow();
                        
                    };
                    break;
                
                case nameof(ButtonNames.MasterSequencePlusConfig):
                    button.clickable.clicked += () => {
                        GameObject rootConfig = Utils.FilterSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig))[0] as GameObject;
                        GameObject masterSequence = CreateMasterSequence(rootConfig.GetComponent<RootConfig>(), sequenceName);
                        if (selectOnCreation == true) {
                            Selection.activeObject = CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(), selectedObjectDirectory, sequenceName);
                        }
                        else {
                            CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(), selectedObjectDirectory, sequenceName);
                        }
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.MasterSequence):
                    button.clickable.clicked += () =>
                    {
                        GameObject rootConfig = Utils.FilterSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig))[0] as GameObject;
                        if (selectOnCreation == true) {
                            Selection.activeObject =
                                CreateMasterSequence(rootConfig.GetComponent<RootConfig>(), sequenceName);
                        }
                        else {
                            CreateMasterSequence(rootConfig.GetComponent<RootConfig>(), sequenceName);
                        }
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.SequenceConfig):
                    button.clickable.clicked += () =>
                    {
                        GameObject masterSequence = Utils.FilterSelection(Selection.gameObjects, typeof(MasterSequence))[0] as GameObject;
                        if (selectOnCreation == true) {
                            Selection.activeObject = CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(),
                                selectedObjectDirectory, sequenceName);
                        }
                        else {
                            CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(),
                                selectedObjectDirectory, sequenceName);
                        }
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SequenceConfigDependenciesPopulated, button);
                    break;

                case nameof(ButtonNames.SimpleDirector):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.activeObject = CreateSimpleDirector(Selection.transforms, selectedObjectDirectory, sequenceName);
                        }
                        else {
                            CreateSimpleDirector(Selection.transforms, selectedObjectDirectory, sequenceName);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SimpleDirectorDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.Sequence):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            var ( sequence, timelineAsset) = CreateSequenceTimelinePair(selectedObjectDirectory, sequenceName);
                            Selection.objects = new UnityEngine.Object[] { sequence, timelineAsset };
                        } else {
                            CreateSequenceTimelinePair(selectedObjectDirectory, sequenceName);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.TimelineAsset):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.activeObject = CreateTimelineAsset(selectedObjectDirectory, sequenceName);
                        }
                        else {
                            CreateTimelineAsset(selectedObjectDirectory, sequenceName);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.CloneTimeline):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.objects = CloneTimeline(Selection.gameObjects);
                        }
                        else {
                            CloneTimeline(Selection.gameObjects);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.PlayableDirectorSelected, button);
                    break;
                
                case nameof(ButtonNames.SiblingSequenceConfig):
                    button.clickable.clicked += () =>
                    {
                        TrackAsset[] trackSelection = Array.ConvertAll(Utils.FilterSelection(Selection.objects, typeof(TrackAsset)), x => (TrackAsset) x);
                        if (selectOnCreation == true) {
                            Selection.objects = CreateSiblingSequenceConfig(Selection.gameObjects, trackSelection);
                        }
                        else {
                            CreateSiblingSequenceConfig(Selection.gameObjects, trackSelection);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SiblingSequeenceConfigDependenciesPopulated, button);
                    break;
            }

            return button;
        }

        private VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {
                
                case nameof(UpdateWindowTriggers.SequenceName):
                    visualElement.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) => {
                        UpdateDisplay();
                    });
                    break;
            }

            return visualElement;
        }

        public static GameObject CreateRootConfig()
        {
            GameObject rootConfig = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.rootConfig) as GameObject;
            Undo.RegisterCreatedObjectUndo(rootConfig, "Create root config");
            return rootConfig;
        }
        
        public static GameObject CreateMasterSequence(RootConfig rootConfig, string name)
        {
            GameObject masterSequenceObject = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.masterSequence) as GameObject;
            Undo.RegisterCreatedObjectUndo(masterSequenceObject, "Create master sequence");

            masterSequenceObject.name = name + "-Master";
            rootConfig.masterSequences.Add(masterSequenceObject.GetComponent<MasterSequence>());
            EditorUtility.SetDirty(rootConfig);
            
            Undo.SetTransformParent(masterSequenceObject.transform, rootConfig.masterSequenceContainer.transform, "set parent on new element");

            return masterSequenceObject;
        }
        
        public static GameObject CreateSequenceConfig(MasterSequence masterSequence, string targetDirectory, string name)
        {
            var ( newSequence, newTimelineAsset ) = CreateSequenceTimelinePair(targetDirectory, name);
            if (newSequence != null) {
                GameObject sequenceConfigObject = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.sequenceConfig) as GameObject;
                Undo.RegisterCreatedObjectUndo(sequenceConfigObject, "Create sequence config");

                sequenceConfigObject.name = name;
                
                Sequence_Config sequenceConfig = sequenceConfigObject.GetComponent<Sequence_Config>();
                sequenceConfig.sequence = newSequence;

                PlayableDirector playableDirector = sequenceConfigObject.GetComponent<PlayableDirector>();
                playableDirector.playableAsset = newTimelineAsset;

                masterSequence.sequenceConfigs.Add(sequenceConfigObject.GetComponent<Sequence_Config>());
                EditorUtility.SetDirty(masterSequence);

                Undo.SetTransformParent(sequenceConfigObject.transform, masterSequence.gameObject.transform, "set parent on new element");

                return sequenceConfigObject;
            }
            
            Debug.Log("Unable to create sequence and related timeline asset.");

            return masterSequence.gameObject;
        }

        public static Tuple<Sequence, TimelineAsset> CreateSequenceTimelinePair(string targetDirectory, string name)
        {
            if (targetDirectory.Length > 0) {

                string timelineAssetPath = targetDirectory + "/" + name + ".playable"; 
                string sequencePath = targetDirectory + "/" + name + ".asset";

                bool createAsset = true;
                
                if (File.Exists(Path.GetFullPath(sequencePath)) || File.Exists(Path.GetFullPath(timelineAssetPath))) {
                    if (EditorUtility.DisplayDialog("Overwrite existing file?", 
                        "This will overwrite the existing file(s) at " + targetDirectory + "/" + name, 
                        "Proceed", "Cancel") == false) {
                        createAsset = false;
                    }
                }

                if (createAsset == true) {
                    TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
                    newTimelineAsset.editorSettings.fps = 100f;
                    Sequence newSequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;
                    
                    PromptActivateSequence(newSequence);
                    AssetDatabase.CreateAsset(newSequence, sequencePath);
                    AssetDatabase.CreateAsset(newTimelineAsset, timelineAssetPath);
                    TimelineUtils.CreateDebugTrack(newTimelineAsset);
                    TimelineUtils.CreateConfigTrack(newTimelineAsset);
                    newSequence.sourcePlayable = newTimelineAsset;
                    EditorUtility.SetDirty(newSequence);
                    return new Tuple<Sequence, TimelineAsset>(newSequence, newTimelineAsset);
                }
                
            } else {
                Debug.Log("Directory not valid");
            }

            return new Tuple<Sequence, TimelineAsset>(null, null);
        }

        public static Sequence PromptActivateSequence(Sequence targetSequence)
        {
            if (EditorUtility.DisplayDialog("Set sequence to active?", "Would you like to set sequence " + targetSequence.name + " to active?", "Yes", "No")) {
                targetSequence.active = true;
            } else {
                targetSequence.active = false;
            }

            return targetSequence;
        }
        
        public static PlayableDirector CreateSimpleDirector(Transform[] selectedTransforms, string targetDirectory, string name)
        {
            TimelineAsset timelineAsset = CreateTimelineAsset(targetDirectory, name);
            
            if (timelineAsset != null) {
                PlayableDirector playableDirector = ModuleUtils.CreateElement(selectedTransforms, ModuleUtils.moduleReferences.standardDirector, name).GetComponent<PlayableDirector>();
                playableDirector.playableAsset = timelineAsset;
                EditorUtility.SetDirty(playableDirector);

                return playableDirector;
            } else {
                Debug.Log("Creation cancelled");
            }

            return null;
        }
        
        private static TimelineAsset CreateTimelineAsset(string targetDirectory, string name)
        {
            string timelineAssetPath = targetDirectory + "/" + name + ".playable";
            
            bool createAsset = true;
            
            if (File.Exists(Path.GetFullPath(timelineAssetPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", 
                    "This will overwrite the existing file at " + timelineAssetPath, 
                    "Proceed", "Cancel") == false) {
                    createAsset = false;
                } 
            }

            if (createAsset == true) {
                TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
                newTimelineAsset.editorSettings.fps = 100f;
                AssetDatabase.CreateAsset(newTimelineAsset, timelineAssetPath);
                TimelineUtils.CreateDebugTrack(newTimelineAsset);
                TimelineUtils.CreateConfigTrack(newTimelineAsset);
                return newTimelineAsset;
            }

            return null;
        }

        public static GameObject[] CloneTimeline(GameObject[] selection)
        {
            bool selectionValid = true;

            for (int i = 0; i < selection.Length; i++) {
                if (CanCloneTimeline(selection[i]) == false) {
                    selectionValid = false;
                }
            }

            if (selectionValid == false) {
                EditorUtility.DisplayDialog("Clone unsuccessful",
                    "Please make sure your selected objects contain playable directors and a clone doesn't already exist", "Ok");
                return selection;
            }

            List<GameObject> clonedObjects = new List<GameObject>(); 

            for (int i = 0; i < selection.Length; i++) {
                
                GameObject sourceObject = selection[i];
                GameObject clonedObject = Utils.DuplicateGameObject(sourceObject);
                MigrateTimelineConfig(sourceObject, clonedObject);
                
                clonedObject.name += " (Clone)";
                
                Undo.SetTransformParent(clonedObject.transform, sourceObject.transform.parent, "Set timeline clone parent");
                clonedObject.transform.SetSiblingIndex(sourceObject.transform.GetSiblingIndex() + 1);
                
                clonedObjects.Add(clonedObject);
            }

            return clonedObjects.ToArray();
        }

        private static GameObject MigrateTimelineConfig(GameObject sourceObject, GameObject targetObject)
        {
            var playableDirector = sourceObject.GetComponent<PlayableDirector>();
            var playableAsset = playableDirector.playableAsset;
            var sourceAssetPath = AssetDatabase.GetAssetPath(playableAsset);
                
            string clonePath = Utils.GetCloneAssetPath(sourceAssetPath);
            if (!AssetDatabase.CopyAsset(sourceAssetPath, clonePath)) {
                Debug.LogError("Unable to clone asset");
                return sourceObject;
            }
            var clonedPlayableAsset = AssetDatabase.LoadMainAssetAtPath(clonePath) as PlayableAsset;

            var clonedPlayableDirector = targetObject.GetComponent<PlayableDirector>();
            clonedPlayableDirector.playableAsset = clonedPlayableAsset;

            var originalBindings = playableAsset.outputs.GetEnumerator();
            var clonedBindings = clonedPlayableAsset.outputs.GetEnumerator();

            while (originalBindings.MoveNext()) {
                var originalSourceObject = originalBindings.Current.sourceObject;

                clonedBindings.MoveNext();

                var clonedSourceObject = clonedBindings.Current.sourceObject;

                clonedPlayableDirector.SetGenericBinding(
                    clonedSourceObject,
                    playableDirector.GetGenericBinding(originalSourceObject)
                );
            }

            return targetObject;
        }

        public static bool CanCloneTimeline(GameObject selection)
        {
            var playableDirector = selection.GetComponent<PlayableDirector>();
            if (playableDirector == null) {
                Debug.LogError("Playable director not found", selection);
                return false;
            }

            var playableAsset = playableDirector.playableAsset;
            if (playableAsset == null) {
                Debug.LogError("Playable asset not set on playable director", selection);
                return false;
            }

            var sourceAssetPath = AssetDatabase.GetAssetPath(playableAsset);
            if (string.IsNullOrEmpty(sourceAssetPath)) {
                Debug.LogError("Timeline asset path not valid", selection);
                return false;
            }

            string clonedAssetPath = Utils.GetCloneAssetPath(sourceAssetPath);
            if (File.Exists(clonedAssetPath)) {
                Debug.LogError("Duplicate asset detected; aborting clone attempt", selection);
                return false;
            }

            return true;
        }

        public static GameObject[] CreateSiblingSequenceConfig(GameObject[] gameObjectSelection, TrackAsset[] trackSelection)
        {
            bool selectionValid = true;

            for (int i = 0; i < gameObjectSelection.Length; i++) {
                if (CanCreateSiblingSequenceConfig(gameObjectSelection[i]) == false) {
                    selectionValid = false;
                }
            }

            if (selectionValid == false) {
                EditorUtility.DisplayDialog("Clone unsuccessful",
                    "Please make sure your selected objects contain playable directors and that siblings don't already exist", "Ok");
                return gameObjectSelection;
            }

            List<GameObject> clonedSequenceObjects = new List<GameObject>(); 
            
            for (int i = 0; i < gameObjectSelection.Length; i++) {
                
                // Components whose data we'll use for migration
                GameObject sourceSequenceObject = gameObjectSelection[i];
                PlayableDirector sourceDirector = sourceSequenceObject.GetComponent<PlayableDirector>();
                Sequence_Config sourceConfigComponent = sourceSequenceObject.GetComponent<Sequence_Config>();

                // Objects to receive the data
                GameObject siblingSequenceObject = Utils.DuplicateGameObject(sourceSequenceObject);
                siblingSequenceObject.name += " (Sibling)";
                PlayableDirector siblingDirector = siblingSequenceObject.GetComponent<PlayableDirector>();
                Sequence_Config siblingConfigComponent = siblingSequenceObject.GetComponent<Sequence_Config>();
                
                // Clone the sequence
                Sequence siblingSequence = CreateSiblingSequence(sourceConfigComponent);
                if (siblingSequence == null) {
                    return gameObjectSelection;
                }
                siblingConfigComponent.sequence = siblingSequence;

                TimelineAsset siblingTimelineAsset = CreateSiblingTimelineAsset(sourceConfigComponent);
                if (siblingTimelineAsset == null) {
                    return gameObjectSelection;
                }
                siblingSequence.sourcePlayable = siblingTimelineAsset;
                siblingDirector.playableAsset = siblingTimelineAsset;
                
                // Copy and paste tracks slated for migration
                TrackAsset[] siblingTracks = CreateSiblingTracks(sourceDirector, siblingDirector, trackSelection);
                
                // Remove unneeded clips and set connecting clips to start of timeline  
                SanitizeSiblingTracks(siblingTimelineAsset, siblingTracks);

                // Some housekeeping to make sure everything is placed correctly
                Undo.SetTransformParent(siblingConfigComponent.transform, sourceSequenceObject.transform.parent, "Set timeline clone parent");
                siblingConfigComponent.transform.SetSiblingIndex(sourceSequenceObject.transform.GetSiblingIndex() + 1);
                
                clonedSequenceObjects.Add(siblingSequenceObject);
            }

            return clonedSequenceObjects.ToArray();
        }

        private static Sequence CreateSiblingSequence(Sequence_Config sourceSequenceConfig)
        {
            string sourceAssetPath = AssetDatabase.GetAssetPath(sourceSequenceConfig.sequence);
            string clonePath = Utils.GetCloneAssetPath(sourceAssetPath, "(Sibling)");
            if (!AssetDatabase.CopyAsset(sourceAssetPath, clonePath)) {
                Debug.LogError("Unable to clone asset");
                return null;
            }
            return AssetDatabase.LoadMainAssetAtPath(clonePath) as Sequence;
        }

        private static TimelineAsset CreateSiblingTimelineAsset(Sequence_Config sourceSequenceConfig)
        {
            // Create new timeline asset
            string sourceAssetPath = AssetDatabase.GetAssetPath(sourceSequenceConfig.sequence.sourcePlayable);
            string clonePath = Utils.GetCloneAssetPath(sourceAssetPath, "(Sibling)");
            
            return CreateTimelineAsset(Path.GetDirectoryName(clonePath), Path.GetFileNameWithoutExtension(clonePath));
        }
        
        private static TrackAsset[] CreateSiblingTracks(PlayableDirector sourceDirector, PlayableDirector siblingDirector, TrackAsset[] trackSelection)
        {
            TimelineAsset sourceTimelineAsset = sourceDirector.playableAsset as TimelineAsset;
            TimelineAsset siblingTimelineAsset = siblingDirector.playableAsset as TimelineAsset;
            
            List<TrackAsset> tracksToCopy = new List<TrackAsset>();
            List<TrackAsset> parentTracks = new List<TrackAsset>();
            
            // Flag any manually selected tracks for copying
            tracksToCopy.AddRange(trackSelection);
            for (int i = 0; i < trackSelection.Length; i++) {
                parentTracks.AddRange(TimelineUtils.GetParentTracks(trackSelection[i]));
            }
            
            // Add any additional tracks that have been marked as required
            foreach (var trackAsset in sourceTimelineAsset.GetOutputTracks()) {
                if (trackAsset is LerpToTargetTrack lerpToTargetTrack &&
                    lerpToTargetTrack.requiredForSiblingSequence == true) {
                    if (tracksToCopy.Contains(trackAsset) == false) {
                        tracksToCopy.Add(trackAsset);
                    }
                    parentTracks.AddRange(TimelineUtils.GetParentTracks(trackAsset));
                }
            }
            
            // Add all parent tracks to preserve track hierarchy
            for (int j = 0; j < parentTracks.Count; j++) {
                if (tracksToCopy.Contains(parentTracks[j]) == false) {
                    tracksToCopy.Add(parentTracks[j]);
                }
            }
            
            TrackAsset[] sortedTracks = TimelineUtils.SortTracks(tracksToCopy.ToArray());
            List<TrackPlacement.TrackData> trackData = TrackPlacement.CopyTracks(sourceDirector, sortedTracks, false);
            
            return TrackPlacement.PasteTracks(siblingTimelineAsset, siblingDirector,
                null, null, trackData);
        }

        private static TrackAsset[] SanitizeSiblingTracks(TimelineAsset targetTimelineAsset, TrackAsset[] targetTracks)
        {
            for (int q = 0; q < targetTracks.Length; q++) {
                IEnumerable<TimelineClip> timelineClips = targetTracks[q].GetClips();
                var timelineClipsArray = timelineClips.ToArray();

                if (timelineClipsArray.Length > 0) {
                        
                    // Delete all of the clips on each track, except for the last clip 
                    for (int j = 0; j < timelineClipsArray.Length - 1; j++) {
                        targetTimelineAsset.DeleteClip(timelineClipsArray[j]);
                    }
                        
                    TimelineClip connectingClip = timelineClipsArray[timelineClipsArray.Length - 1];
                        
                    // Set each track's last clip to the start of the timeline
                    connectingClip.start = 0;
                    if (connectingClip.asset is LerpToTargetClip lerpToTargetClip) {
                        lerpToTargetClip.templateReference.SetInitialValueToTarget();
                    }

                }
            }

            return targetTracks;
        }
        
        public static bool CanCreateSiblingSequenceConfig(GameObject selection)
        {
            if (CanCloneTimeline(selection) == false) {
                return false;
            }

            var sequenceConfig = selection.GetComponent<Sequence_Config>();
            if (sequenceConfig == null) {
                Debug.LogError("Sequence config component not found", selection);
                return false;
            }

            var sequence = sequenceConfig.sequence;
            if (sequence == null) {
                Debug.LogError("No sequence populated on sequence config", selection);
                return false;
            }

            string sourceAssetPath = AssetDatabase.GetAssetPath(sequence);
            if (string.IsNullOrEmpty(sourceAssetPath)) {
                Debug.LogError("Sequence path not valid", selection);
                return false;
            }

            string clonedAssetPath = Utils.GetCloneAssetPath(sourceAssetPath);
            if (File.Exists(clonedAssetPath)) {
                Debug.LogError("Duplicate asset detected; aborting clone attempt", selection);
                return false;
            }
            
            return true;
        }
    }
}
