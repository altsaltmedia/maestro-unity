using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Layout
{
    public class ContentExtensionStructures : ModuleWindow
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
        public bool selectOnCreation = true;
        
        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            LayoutConfig,
            TextFamily,
            TextCollectionBank,
            ContentExtensionController
        }

        private enum EnableCondition
        {
            DirectoryAndNamePopulated
        }

        private void UpdateDisplay()
        {
            if (string.IsNullOrEmpty(selectedObjectDirectory) == false && string.IsNullOrEmpty(objectName) == false) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.DirectoryAndNamePopulated, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {
                
                case nameof(ButtonNames.LayoutConfig):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(LayoutConfig), objectName, selectedObjectDirectory) };
                        } else {
                            Utils.CreateScriptableObjectAsset(typeof(LayoutConfig), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.TextFamily):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(TextFamily), objectName, selectedObjectDirectory) };
                        } else {
                            Utils.CreateScriptableObjectAsset(typeof(TextFamily), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.TextCollectionBank):
                    button.clickable.clicked += () => {
                        if (selectOnCreation == true) {
                            Selection.objects = new[] { Utils.CreateScriptableObjectAsset(typeof(TextCollectionBank), objectName, selectedObjectDirectory) };
                        } else {
                            Utils.CreateScriptableObjectAsset(typeof(TextCollectionBank), objectName, selectedObjectDirectory);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.DirectoryAndNamePopulated, button);
                    break;
                
                case nameof(ButtonNames.ContentExtensionController):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.contentExtensionControllerPrefab, objectName);
                    };
                    break;
            }

            return button;
        }
    }
}
