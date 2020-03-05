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
            Text,
            Sprite,
            Container,
            ResponsiveContainer,
            EmptyContainer,
            EmptyResponsiveContainer,
            SimpleVideoPlayer,
            ScrollSnapController,
            DynamicAppLayoutController,
            DynamicStoryLayoutController
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.Text):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.textPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.Sprite):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.spritePrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.Container):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.containerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.EmptyContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.containerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.EmptyResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.responsiveContainerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.ResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.responsiveContainerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.SimpleVideoPlayer):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.simpleVideoPlayerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ScrollSnapController):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.scrollSnapControllerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.DynamicAppLayoutController):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.dynamicAppLayoutControllerPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.DynamicStoryLayoutController):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.dynamicStoryLayoutControllerPrefab, objectName);
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
