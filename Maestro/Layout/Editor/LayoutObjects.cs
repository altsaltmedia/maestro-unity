using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Layout
{
    public class LayoutObjects : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
            return this;
        }
        
        static GameObject createdGameObject;

        private bool selectCreatedObject => controlPanel.objectCreation.selectCreatedObject;
        
        private string objectName => controlPanel.objectCreation.objectName;

        private enum ButtonNames
        {
            NewText,
            NewSprite,
            NewContainer,
            NewEmptyContainer,
            NewEmptyResponsiveContainer,
            NewResponsiveContainer
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.NewText):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.textPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewSprite):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.spritePrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.containerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewEmptyContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.containerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewEmptyResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.responsiveContainerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.responsiveContainerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
            }

            return button;
        }

    }
}
