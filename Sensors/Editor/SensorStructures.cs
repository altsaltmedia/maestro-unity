using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sensors
{
    public class SensorStructures : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);
            
            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);
            
            return this;
        }
        
        private static VisualElementToggleData toggleData = new VisualElementToggleData();

        private string objectName => controlPanel.objectCreation.objectName;
        public bool selectOnCreation => controlPanel.objectCreation.selectCreatedObject;

        private string selectedObjectDirectory => controlPanel.objectCreation.selectedObjectDirectory;

        private enum ButtonNames
        {
            TouchMonitor,
            HandheldUtils
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.TouchMonitor):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.touchMonitorPrefab, objectName);
                        if (selectOnCreation == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;
                
                case nameof(ButtonNames.HandheldUtils):
                    button.clickable.clicked += () =>
                    {
                        GameObject createdGameObject = ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.handheldUtilsPrefab, objectName);
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
