using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public class ObjectCreation : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private VisualElementToggleData toggleData = new VisualElementToggleData();

        public string objectName = "[INSERT NAME HERE]";
        public bool selectCreatedObject;

        UnityEngine.Object[] savedSelection;
        static List<GameObject> objectsToCopy = new List<GameObject>();
        
        enum ButtonNames
        {
            RenameElements,
            SaveSelection,
            LoadSelection,
            SelectAllChildren,
            AddAllChildrenToSelection,
            CopyGameObjects,
            PasteGameObjects
        }
        
        enum EnableCondition
        {
            GameObjectSelected
        }

        void UpdateDisplay()
        {
            if(Selection.gameObjects.Length > 0) {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, true);
            } else {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.RenameElements):
                    button.clickable.clicked += () => { Utils.RenameElements(objectName, Selection.gameObjects); };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.SaveSelection):
                    button.clickable.clicked += () => { savedSelection = Selection.objects; };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.LoadSelection):
                    button.clickable.clicked += () => { Selection.objects = savedSelection; };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.SelectAllChildren):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.AddAllChildrenToSelection):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(Selection.gameObjects);
                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.CopyGameObjects):
                    button.clickable.clicked += () =>
                    {
                        for (int z = 0; z < objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }

                        objectsToCopy.Clear();
                        Array.Sort(Selection.gameObjects, new Utils.GameObjectSort());
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            GameObject copy = Utils.DuplicateObject(Selection.gameObjects[i]);
                            copy.transform.localPosition = Selection.gameObjects[i].transform.localPosition;
                            Undo.RegisterCreatedObjectUndo(copy, "Copy game object");
                            objectsToCopy.Add(copy);
                        }
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.PasteGameObjects):
                    button.clickable.clicked += () =>
                    {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            for (int z = objectsToCopy.Count - 1; z >= 0; z--) {
                                GameObject copy = Utils.DuplicateObject(objectsToCopy[z]);
                                copy.hideFlags = HideFlags.None;
                                Undo.RegisterCreatedObjectUndo(copy, "Paste game object");
                                Undo.SetTransformParent(copy.transform, Selection.gameObjects[i].transform,
                                    "Set parent on pasted object");
                            }
                        }

                        for (int z = 0; z < objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }

                        objectsToCopy.Clear();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

            }

            return button;
        }
    }
}