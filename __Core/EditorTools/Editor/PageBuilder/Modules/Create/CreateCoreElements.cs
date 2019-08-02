using System.Collections;
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

        enum ButtonNames
        {
            NewText,
            NewSprite,
            NewContainer,
            NewResponsiveContainer,
            RenameElements
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
            }

            return button;
        }

    }
}
