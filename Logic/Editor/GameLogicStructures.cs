using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Logic
{
    public class GameLogicStructures : ModuleWindow
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
            BoolVariable,
            IntVariable,
            StringVariable,
            ColorVariable,
            FloatVariable,
            V2Variable,
            V3Variable,
            AxisVariable,
            SimpleEvent,
            ComplexEvent,
            ComplexPayload,
            SimpleSignalListener,
            ComplexEventListener,
            ActionTrigger,
            PrepareScene
        }

        private enum EnableCondition
        {
            DirectorySelected,
            GameObjectSelected
        }

        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectorySelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectorySelected, false);
            }
            
            if (Selection.gameObjects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.BoolVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(BoolVariable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(BoolVariable), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.IntVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(IntVariable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(IntVariable), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.StringVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(StringVariable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(StringVariable), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                
                case nameof(ButtonNames.ColorVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(ColorVariable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(ColorVariable), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.FloatVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(FloatVariable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(FloatVariable), objectName,
                                selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.V2Variable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(V2Variable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(V2Variable), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                
                case nameof(ButtonNames.V3Variable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(V3Variable), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(V3Variable), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                
                case nameof(ButtonNames.AxisVariable):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(Axis), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(Axis), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.SimpleEvent):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(SimpleEvent), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(SimpleEvent), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.ComplexEvent):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(ComplexEvent), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(ComplexEvent), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.ComplexPayload):
                    button.clickable.clicked += () =>  {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(ComplexPayload), objectName, selectedObjectDirectory) };
                            EditorGUIUtility.PingObject(Selection.activeObject);
                            EditorUtility.FocusProjectWindow();
                        }
                        else {
                            Utils.CreateScriptableObjectAsset(typeof(ComplexPayload), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectorySelected, button);
                    break;
                
                case nameof(ButtonNames.SimpleSignalListener):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.simpleSignalListenerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ComplexEventListener):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.complexEventListenerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.ActionTrigger):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.actionTriggerPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.PrepareScene):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.prepareScenePrefab, objectName);
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
