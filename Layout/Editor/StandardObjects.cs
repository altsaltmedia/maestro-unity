using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Layout
{
    public class StandardObjects : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            appSettings = Utils.GetAppSettings();
            
            return this;
        }

        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get => _appSettings;
            set => _appSettings = value;
        }
        
        private float sceneHeight => appSettings.GetCurrentSceneHeight(this);
        private float sceneWidth => appSettings.GetCurrentSceneWidth(this);

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
            Camera,
            ResponsiveCamera,
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
                        RectTransform rectTransform =  createdGameObject.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = new Vector2((float)Utils.GetResponsiveWidth(sceneHeight, sceneWidth), rectTransform.sizeDelta.y);
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
                
                case nameof(ButtonNames.Container):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.containerPrefab, objectName, true);
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
                
                case nameof(ButtonNames.Camera):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.cameraPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ResponsiveCamera):
                    button.clickable.clicked += () => {
                        createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.responsiveCameraPrefab, objectName);
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
