using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.IO;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt
{
    public class CreateSequenceElements : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;
            toggleData.Clear();

            var buttons = parentVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            var updateWindowTriggers = parentVisualElement.Query<VisualElement>(null, EditorToolsCore.UpdateWindowTrigger);
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);

            UpdateDisplay();
            pageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            pageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        static VisualElementToggleData toggleData = new VisualElementToggleData();

        public SequenceController sequenceControllerObject;
        public SequenceList targetSequenceList;
        public string newSequenceName = "";

        enum ButtonNames
        {
            NewSequenceTouchApplier,
            NewSequenceAutoplayer,
            NewSequenceList,
            NewSwipeDirector,
            RefreshLayout,
        }

        enum EnableCondition
        {
            SequenceControllerObjectPopulated,
            SequenceListNamePopulated
        }

        enum UpdateWindowTriggers
        {
            SequenceControllerObject,
            TargetSequenceList,
            NewSequenceName
        }

        void UpdateDisplay()
        {
            if (sequenceControllerObject != null) {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, true);
            } else {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, false);
            }

            if (targetSequenceList != null && newSequenceName.Length > 0) {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, true);
            } else {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.NewSequenceTouchApplier):
                    button.clickable.clicked += () => {
                        CreateSequenceElements.CreateSequenceTouchApplier(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSequenceAutoplayer):
                    button.clickable.clicked += () => {
                        CreateSequenceElements.CreateSequenceAutoplayer(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSequenceList):
                    button.clickable.clicked += () => {
                        CreateSequenceElements.CreateSequenceList(sequenceControllerObject);
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.SequenceControllerObjectPopulated, button);
                    break;

                case nameof(ButtonNames.NewSwipeDirector):
                    button.clickable.clicked += () => {
                        CreateSequenceElements.CreateSwipeDirector(Selection.activeTransform, targetSequenceList, newSequenceName);
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.SequenceListNamePopulated, button);
                    break;
            }

            return button;
        }

        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.SequenceControllerObject):
                case nameof(UpdateWindowTriggers.TargetSequenceList):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateDisplay();
                    });
                    break;

                case nameof(UpdateWindowTriggers.NewSequenceName):
                    visualElement.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) => {
                        UpdateDisplay();
                    });
                    break;
            }

            return visualElement;
        }

        public static GameObject CreateSequenceTouchApplier(Transform parentTransform)
        {
            GameObject sequenceTouchApplier = PrefabUtility.InstantiatePrefab(PageBuilderCore.PrefabReferences.sequenceTouchApplier) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceTouchApplier, "Create sequence touch applier");

            if (parentTransform != null) {
                sequenceTouchApplier.transform.SetParent(parentTransform);
            }
            return sequenceTouchApplier;
        }

        public static GameObject CreateSequenceAutoplayer(Transform parentTransform)
        {
            GameObject sequenceAutoplayer = PrefabUtility.InstantiatePrefab(PageBuilderCore.PrefabReferences.sequenceAutoplayer) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceAutoplayer, "Create sequence autoplayer");

            if (parentTransform != null) {
                sequenceAutoplayer.transform.SetParent(parentTransform);
            }
            return sequenceAutoplayer;
        }


        public static GameObject CreateSwipeDirector(Transform parentTransform, SequenceList parentSequenceList, string elementName)
        {
            GameObject swipeDirector = PrefabUtility.InstantiatePrefab(PageBuilderCore.PrefabReferences.swipeDirector) as GameObject;
            Undo.RegisterCreatedObjectUndo(swipeDirector, "Create swipe director");

            if (parentTransform != null) {
                swipeDirector.transform.SetParent(parentTransform);
            }

            if (elementName.Length > 0) {
                swipeDirector.name = elementName;
            }

            string parentDirectoryPath = Utils.GetAssetPathFromObject(parentSequenceList);
            string filePath = Path.GetDirectoryName(parentDirectoryPath) + "/" + elementName;

            swipeDirector = AddSequenceToSwipeDirector(swipeDirector, parentSequenceList, filePath);
            swipeDirector = AddTimelineAssetToSwipeDirector(swipeDirector, filePath);

            Selection.activeTransform = swipeDirector.transform;
            return swipeDirector;
        }

        static GameObject AddSequenceToSwipeDirector(GameObject swipeDirector, SequenceList targetSequenceList, string filePath)
        {
            Sequence newSequence = CreateSequence(filePath);
            if (newSequence != null) {
                swipeDirector.GetComponent<DirectorUpdater>().sequence = newSequence;
                targetSequenceList.sequences.Add(newSequence);
            }
            return swipeDirector;
        }

        static Sequence CreateSequence(string filePath)
        {
            Sequence newSequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;

            if (EditorUtility.DisplayDialog("Set sequence to active?", "Would you like to set sequence " + Path.GetFileName(filePath) + " to active?", "Yes", "No")) {
                newSequence.Active = true;
            } else {
                newSequence.Active = false;
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

        static GameObject AddTimelineAssetToSwipeDirector(GameObject swipeDirector, string filePath)
        {
            TimelineAsset newTimelineAsset = CreateTimelineAsset(filePath);
            if (newTimelineAsset != null) {
                swipeDirector.GetComponent<PlayableDirector>().playableAsset = newTimelineAsset;
            }
            return swipeDirector;
        }

        static TimelineAsset CreateTimelineAsset(string filePath)
        {
            TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
            newTimelineAsset.editorSettings.fps = 100f;
            string finalPath = filePath + ".playable";

            if (File.Exists(Path.GetFullPath(finalPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                    AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
                } else {
                    EditorUtility.DisplayDialog("No sequence populated", "Timeline asset creation cancelled.", "Ok");
                    return null;
                }
            } else {
                AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
            }

            return newTimelineAsset;
        }

        public static SequenceList CreateSequenceList(SequenceController sequenceController)
        {
            string filePath = EditorUtility.SaveFilePanelInProject("Create new sequence list", "", "asset", "Please enter a file name for the new sequence list");
            SequenceList newSequenceList = ScriptableObject.CreateInstance(typeof(SequenceList)) as SequenceList;
            AssetDatabase.CreateAsset(newSequenceList, filePath);

            sequenceController.sequenceLists.Add(newSequenceList);

            return newSequenceList;
        }

    }
}
