using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Animation
{
    public class AnimationStructures : ModuleWindow
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
            ManualVideoPlayer,
            ForwardReverseVideoPlayer,
            DOTweenUtils
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
                
                case nameof(ButtonNames.ManualVideoPlayer):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.manualVideoPlayerPrefab, objectName);
                    };
                    break;
                
                case nameof(ButtonNames.ForwardReverseVideoPlayer):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.forwardReverseVideoPlayerPrefab, objectName);
                    };
                    break;
                
                case nameof(ButtonNames.DOTweenUtils):
                    button.clickable.clicked += () => {
                        Selection.activeGameObject =
                            ModuleUtils.CreateElement(Selection.transforms, ModuleUtils.moduleReferences.DOTweenUtilsPrefab, objectName);
                    };
                    break;
            }

            return button;
        }
    }
}
