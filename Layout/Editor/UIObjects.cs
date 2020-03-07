using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Layout
{
    public class UIObjects : ModuleWindow
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
            Canvas,
            ChildCanvas,
            OverlayCanvas,
            Image,
            Background,
            UIText,
            Button,
            ButtonWithText,
            Slider,
            CallWebviewButton
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.Canvas):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.canvasPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.ChildCanvas):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.childCanvasPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.OverlayCanvas):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.overlayCanvasPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.Image):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.imagePrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.Background):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.backgroundPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.UIText):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.uiTextPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                
                case nameof(ButtonNames.Button):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.buttonPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ButtonWithText):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.buttonWithTextPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.Slider):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.sliderPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.CallWebviewButton):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.callWebviewButtonPrefab, objectName);
                        createdGameObject.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);
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
