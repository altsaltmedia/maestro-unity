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
            NewSimpleDirector,
            NewSequenceTouchApplier,
            NewSequenceAutoplayer,
            NewSwipeDirector
        }

        private enum EnableCondition
        {
            MasterSequencePlusDependenciesPopulated,
            MasterSequenceDependenciesPopulated,
            SequenceConfigDependenciesPopulated,
            SequenceControllerObjectPopulated,
            SequenceListNamePopulated
        }

        private enum UpdateWindowTriggers
        {
            SequenceControllerObject,
            TargetSequenceList,
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

            if (sequenceControllerObject != null) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, false);
            }

            if (_targetSequenceCollection != null && sequenceName.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, false);
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

                case nameof(ButtonNames.NewSimpleDirector):
                    button.clickable.clicked += () => {
                        CreateStandardDirector();
                    };
                    break;

                case nameof(ButtonNames.NewSequenceTouchApplier):
                    button.clickable.clicked += () => {
                        SequencingStructures.CreateSequenceTouchApplier(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSequenceAutoplayer):
                    button.clickable.clicked += () => {
                        SequencingStructures.CreateSequenceAutoplayer(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSwipeDirector):
                    button.clickable.clicked += () => {
                        SequencingStructures.CreateSwipeDirector(Selection.activeTransform, _targetSequenceCollection, sequenceName);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SequenceListNamePopulated, button);
                    break;
            }

            return button;
        }

        private VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.SequenceControllerObject):
                case nameof(UpdateWindowTriggers.TargetSequenceList):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateDisplay();
                    });
                    break;

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
            Tuple<Sequence, TimelineAsset> sequencePair = CreateSequence(targetDirectory, name);
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

        public static Tuple<Sequence, TimelineAsset> CreateSequence(string targetDirectory, string name)
        {
            if (targetDirectory.Length > 0) {

                TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
                newTimelineAsset.editorSettings.fps = 100f;

                Sequence newSequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;
                newSequence.sourcePlayable = newTimelineAsset;
                
                string timelineAssetPath = targetDirectory + "/" + name + ".playable"; 
                string sequencePath = targetDirectory + "/" + name + ".asset";
                
                if (File.Exists(Path.GetFullPath(sequencePath)) || File.Exists(Path.GetFullPath(timelineAssetPath))) {
                    if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file(s) at " + targetDirectory + "/" + name, "Proceed", "Cancel")) {
                        PromptActivateSequence(newSequence);
                        AssetDatabase.CreateAsset(newSequence, sequencePath);
                        AssetDatabase.CreateAsset(newTimelineAsset, timelineAssetPath);
                        TimelineUtils.CreateDebugTrack(newTimelineAsset);
                        TimelineUtils.CreateConfigTrack(newTimelineAsset);
                        return new Tuple<Sequence, TimelineAsset>(newSequence, newTimelineAsset);
                    } else {
                        return null;
                    }
                } else {
                    PromptActivateSequence(newSequence);
                    AssetDatabase.CreateAsset(newSequence, sequencePath);
                    AssetDatabase.CreateAsset(newTimelineAsset, timelineAssetPath);
                    TimelineUtils.CreateDebugTrack(newTimelineAsset);
                    TimelineUtils.CreateConfigTrack(newTimelineAsset);
                    return new Tuple<Sequence, TimelineAsset>(newSequence, newTimelineAsset);
                }
                
            } else {
                Debug.Log("Creation cancelled");
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
        
        public static PlayableDirector CreateStandardDirector()
        {
            string filePath = EditorUtility.SaveFilePanelInProject("Create a new timeline asset", "", "", "Please enter a file name for the new timeline asset");

            if (filePath.Length > 0) {
                TimelineAsset timelineAsset = CreateTimelineAsset(filePath);
                PlayableDirector playableDirector = CreateElement(ModuleUtils.moduleReferences.standardDirector, Selection.activeTransform, "Create standard director").GetComponent<PlayableDirector>();
                playableDirector.playableAsset = timelineAsset;

                Selection.activeGameObject = playableDirector.gameObject;

                return playableDirector;
            } else {
                Debug.Log("Creation cancelled");
            }

            return null;
        }

        public static GameObject CreateElement(GameObject targetElement, Transform parentTransform, string createMessage = "Create new element")
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(targetElement) as GameObject;
            Undo.RegisterCreatedObjectUndo(newElement, createMessage);
            if (parentTransform != null) {
                newElement.transform.SetParent(parentTransform);
            }

            return newElement;
        }

        public static GameObject CreateSequenceTouchApplier(Transform parentTransform)
        {
            GameObject sequenceTouchApplier = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.sequenceTouchApplier) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceTouchApplier, "Create sequence touch applier");

            if (parentTransform != null) {
                sequenceTouchApplier.transform.SetParent(parentTransform);
            }
            return sequenceTouchApplier;
        }

        public static GameObject CreateSequenceAutoplayer(Transform parentTransform)
        {
            GameObject sequenceAutoplayer = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.sequenceAutoplayer) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceAutoplayer, "Create sequence autoplayer");

            if (parentTransform != null) {
                sequenceAutoplayer.transform.SetParent(parentTransform);
            }
            return sequenceAutoplayer;
        }


        public static GameObject CreateSwipeDirector(Transform parentTransform, MasterSequence parentSequenceCollection, string elementName)
        {
            GameObject swipeDirector = PrefabUtility.InstantiatePrefab(ModuleUtils.moduleReferences.swipeDirector) as GameObject;
            Undo.RegisterCreatedObjectUndo(swipeDirector, "Create swipe director");

            if (parentTransform != null) {
                swipeDirector.transform.SetParent(parentTransform);
            }

            if (elementName.Length > 0) {
                swipeDirector.name = elementName;
            }

            string parentDirectoryPath = Utils.GetAssetPathFromObject(parentSequenceCollection);
            string filePath = Path.GetDirectoryName(parentDirectoryPath) + "/" + elementName;

            swipeDirector = AddSequenceToSwipeDirector(swipeDirector, parentSequenceCollection, filePath);
            swipeDirector = AddTimelineAssetToSwipeDirector(swipeDirector, filePath);

            Selection.activeTransform = swipeDirector.transform;
            return swipeDirector;
        }

        private static GameObject AddSequenceToSwipeDirector(GameObject swipeDirector, MasterSequence targetSequenceCollection, string filePath)
        {
            Sequence newSequence = CreateSequence(filePath);
            if (newSequence != null) {
                swipeDirector.GetComponent<Sequence_SyncTimeline>().sequence = newSequence;
                //targetSequenceCollection.rootConfig.rootDataCollectors.Add(newSequence.sequenceConfig);
            }
            return swipeDirector;
        }

        private static Sequence CreateSequence(string filePath)
        {
            Sequence newSequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;

            if (EditorUtility.DisplayDialog("Set sequence to active?", "Would you like to set sequence " + Path.GetFileName(filePath) + " to active?", "Yes", "No")) {
                newSequence.active = true;
            } else {
                newSequence.active = false;
            }

            string finalPath = filePath + ".asset";

            if (File.Exists(Path.GetFullPath(finalPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                    AssetDatabase.CreateAsset(newSequence, finalPath);
                } else {
                    EditorUtility.DisplayDialog("No sequence populated", "Sequence creation cancelled.", "Ok");
                    return null;
                }
            } else {
                AssetDatabase.CreateAsset(newSequence, finalPath);
            }

            return newSequence;
        }

        private static GameObject AddTimelineAssetToSwipeDirector(GameObject swipeDirector, string filePath)
        {
            TimelineAsset newTimelineAsset = CreateTimelineAsset(filePath);
            if (newTimelineAsset != null) {
                swipeDirector.GetComponent<PlayableDirector>().playableAsset = newTimelineAsset;
            }
            return swipeDirector;
        }

        private static TimelineAsset CreateTimelineAsset(string filePath)
        {
            TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
            newTimelineAsset.editorSettings.fps = 100f;
            string finalPath = filePath + ".playable";

            if (File.Exists(Path.GetFullPath(finalPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                    AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
                } else {
                    EditorUtility.DisplayDialog("No assets created", "Timeline asset creation cancelled.", "Ok");
                    return null;
                }
            } else {
                AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
            }

            return newTimelineAsset;
        }

    }
}
