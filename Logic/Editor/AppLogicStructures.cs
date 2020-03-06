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
