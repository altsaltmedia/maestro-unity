using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

namespace AltSalt
{
    public class CreateCoreElements : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;
            toggleData.Clear();

            var buttons = parentVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        static VisualElementToggleData toggleData = new VisualElementToggleData();

        public string objectName = "[INSERT NAME HERE]";
        public bool selectCreatedObject;
        static GameObject createdGameObject;

        UnityEngine.Object[] savedSelection;
        static List<GameObject> objectsToCopy = new List<GameObject>();

        enum ButtonNames
        {
            NewText,
            NewSprite,
            NewContainer,
            NewEmptyContainer,
            NewEmptyResponsiveContainer,
            NewResponsiveContainer,
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

                case nameof(ButtonNames.NewText):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.textPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewSprite):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.spritePrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.containerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewEmptyContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.containerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewEmptyResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.responsiveContainerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = PageBuilderCore.CreateNewElement(Selection.transforms, PageBuilderCore.PrefabReferences.responsiveContainerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.RenameElements):
                    button.clickable.clicked += () => {
                        Utils.RenameElements(objectName, Selection.gameObjects);
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.SaveSelection):
                    button.clickable.clicked += () => {
                        savedSelection = Selection.objects;
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.LoadSelection):
                    button.clickable.clicked += () => {
                        Selection.objects = savedSelection;
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.SelectAllChildren):
                    button.clickable.clicked += () => {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.AddAllChildrenToSelection):
                    button.clickable.clicked += () => {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(Selection.gameObjects);
                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.CopyGameObjects):
                    button.clickable.clicked += () => {
                        for(int z=0; z<objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }
                        objectsToCopy.Clear();
                        Array.Sort(Selection.gameObjects, new Utils.GameObjectSort());
                        for(int i=0; i< Selection.gameObjects.Length; i++) {
                            GameObject copy = Utils.DuplicateObject(Selection.gameObjects[i]);
                            copy.transform.localPosition = Selection.gameObjects[i].transform.localPosition;
                            Undo.RegisterCreatedObjectUndo(copy, "Copy game object");
                            objectsToCopy.Add(copy);
                        }
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;

                case nameof(ButtonNames.PasteGameObjects):
                    button.clickable.clicked += () => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            for(int z=objectsToCopy.Count - 1; z>=0; z--) {
                                GameObject copy = Utils.DuplicateObject(objectsToCopy[z]);
                                copy.hideFlags = HideFlags.None;
                                Undo.RegisterCreatedObjectUndo(copy, "Paste game object");
                                Undo.SetTransformParent(copy.transform, Selection.gameObjects[i].transform, "Set parent on pasted object");
                            }
                        }
                        for (int z = 0; z < objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }
                        objectsToCopy.Clear();
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected, button);
                    break;
            }

            return button;
        }

    }
}
