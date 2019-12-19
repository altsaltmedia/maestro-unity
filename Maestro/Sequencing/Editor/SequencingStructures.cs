using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.IO;
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
        
        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            RootConfig,
            MasterSequencePlusConfig,
            MasterSequence,
            SequenceConfig,
            SimpleDirector,
            Sequence,
            TimelineAsset
        }

        private enum EnableCondition
        {
            MasterSequencePlusDependenciesPopulated,
            MasterSequenceDependenciesPopulated,
            SequenceConfigDependenciesPopulated,
            SimpleDirectorDependenciesPopulated,
            DirectoryAndNamePopulated
        }

        private enum UpdateWindowTriggers
        {
            SequenceName
        }

        private void UpdateDisplay()
        {
            if(Utils.CullSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig)).Length > 0
               && string.IsNullOrEmpty(selectedObjectDirectory) == false
               && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, false);
            }
            
            if(Utils.CullSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig)).Length > 0
               && string.IsNullOrEmpty(sequenceName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, false);
            }

            if (Utils.CullSelection(Selection.gameObjects, typeof(MasterSequence)).Length > 0
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
                        GameObject rootConfig = Utils.CullSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig))[0] as GameObject;
                        GameObject masterSequence = CreateMasterSequence(rootConfig.GetComponent<RootConfig>(), sequenceName);
                        Selection.activeObject = CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(), selectedObjectDirectory, sequenceName);
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MasterSequencePlusDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.MasterSequence):
                    button.clickable.clicked += () =>
                    {
                        GameObject rootConfig = Utils.CullSelection(Utils.GetParentGameObjects(Selection.gameObjects, true), typeof(RootConfig))[0] as GameObject;
                        Selection.activeObject = CreateMasterSequence(rootConfig.GetComponent<RootConfig>(), sequenceName);
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.MasterSequenceDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.SequenceConfig):
                    button.clickable.clicked += () =>
                    {
                        GameObject masterSequence = Utils.CullSelection(Selection.gameObjects, typeof(MasterSequence))[0] as GameObject;
                        Selection.activeObject = CreateSequenceConfig(masterSequence.GetComponent<MasterSequence>(), selectedObjectDirectory, sequenceName);
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SequenceConfigDependenciesPopulated, button);
                    break;

                case nameof(ButtonNames.SimpleDirector):
                    button.clickable.clicked += () => {
                        CreateSimpleDirector(Selection.transforms, selectedObjectDirectory, sequenceName);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SimpleDirectorDependenciesPopulated, button);
                    break;
                
                case nameof(ButtonNames.Sequence):
                    button.clickable.clicked += () => {
                        CreateSequenceTimelinePair(selectedObjectDirectory, sequenceName);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.TimelineAsset):
                    button.clickable.clicked += () => {
                        CreateTimelineAsset(selectedObjectDirectory, sequenceName);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
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
            Tuple<Sequence, TimelineAsset> sequencePair = CreateSequenceTimelinePair(targetDirectory, name);
            if (sequencePair.Item1 != null) {
                GameObject sequenceConfigObject = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.sequenceConfig) as GameObject;
                Undo.RegisterCreatedObjectUndo(sequenceConfigObject, "Create sequence config");

                sequenceConfigObject.name = name;
                
                Sequence_Config sequenceConfig = sequenceConfigObject.GetComponent<Sequence_Config>();
                sequenceConfig.sequence = sequencePair.Item1;

                PlayableDirector playableDirector = sequenceConfigObject.GetComponent<PlayableDirector>();
                playableDirector.playableAsset = sequencePair.Item2;

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
    }
}
