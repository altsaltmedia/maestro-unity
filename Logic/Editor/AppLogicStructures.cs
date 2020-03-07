using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Logic
{
    public class AppLogicStructures : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
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

        private string objectName => controlPanel.objectCreation.objectName;
        
        public bool selectOnCreation => controlPanel.objectCreation.selectCreatedObject;

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            AppSettings,
            SystemSettings,
            DebugPreferences,
            InputData,
            UserData,
            Initializer,
            AppUtils,
            SystemDependencies,
            FaderProgressBarCanvas,
            FaderPanel,
            ProgressBarPanel,
            RequestAppUtils,
            CurrentAspectRatioCamera,
            SceneController,
            UserDataController,
            FrameRateController,
            AnalyticsTrackers
        }

        private enum EnableCondition
        {
            DirectorySelected
        }

        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectorySelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectorySelected, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.AppSettings):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(AppSettings), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(AppSettings), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.SystemSettings):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(SystemSettings), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(SystemSettings), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.DebugPreferences):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(DebugPreferences), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(DebugPreferences), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.InputData):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(InputData), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(InputData), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.UserData):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(UserData), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(UserData), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;

                case nameof(ButtonNames.Initializer):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.initializerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.AppUtils):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.appUtilsPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.SystemDependencies):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.systemDependenciesPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.FaderProgressBarCanvas):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.faderProgressBarCanvasPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.FaderPanel):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.faderPanelPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ProgressBarPanel):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.progressBarPanelPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.RequestAppUtils):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.requestAppUtilsPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.CurrentAspectRatioCamera):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.currentAspectRatioCameraPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.SceneController):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.sceneControllerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.UserDataController):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.userDataControllerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.FrameRateController):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.frameRateControllerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.AnalyticsTrackers):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.analyticsTrackersPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
            }

            return button;
        }
    }
}
