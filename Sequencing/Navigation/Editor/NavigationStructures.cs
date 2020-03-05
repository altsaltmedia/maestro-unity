using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    public class NavigationStructures : ModuleWindow
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
            ArrowIndicator,
            Scrubber
        }

        private enum EnableCondition
        {
            DirectoryAndNamePopulated
        }

        private enum UpdateWindowTriggers
        {
            SequenceName
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
                
                case nameof(ButtonNames.ArrowIndicator):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.arrowIndicatorPrefab, objectName);
                    };
                    break;
                
                case nameof(ButtonNames.Scrubber):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.scrubberPrefab, objectName);
                    };
                    break;
            }

            return button;
        }
    }
}
