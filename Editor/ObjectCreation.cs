using System;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public class ObjectCreation : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            directoryPathLabel = moduleWindowUXML.Query<Label>(nameof(LabelNames.DirectoryPath));

            RefreshDirectoryName();
            ControlPanel.inspectorUpdateDelegate += RefreshDirectoryName;
            ControlPanel.selectionChangedDelegate += RefreshDirectoryName;

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.inspectorUpdateDelegate -= RefreshDirectoryName;
            ControlPanel.selectionChangedDelegate -= RefreshDirectoryName;
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private VisualElementToggleData toggleData = new VisualElementToggleData();
        
        public string objectName = "";
        public bool selectCreatedObject = true;

        private Label directoryPathLabel;
        public string selectedObjectDirectory = "";

        private enum LabelNames
        {
            DirectoryPath,
        }
        
        private enum ButtonNames
        {
            RenameElements,
        }

        private enum EnableCondition
        {
            ObjectSelected
        }

        private void RefreshDirectoryName()
        {
            UnityEngine.Object[] selection =
                Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            
            if (selection.Length > 0) {
                selectedObjectDirectory = GetSelectedObjectDirectory(selection[0]);
                
                if (string.IsNullOrEmpty(selectedObjectDirectory) == false) {
                    directoryPathLabel.text = selectedObjectDirectory;
                    return;
                }
            }

            selectedObjectDirectory = string.Empty;
            directoryPathLabel.text = "No path selected";
        }

        private void UpdateDisplay()
        {
            if(Selection.objects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectSelected, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.RenameElements):
                    button.clickable.clicked += () =>
                    {
                        if (Selection.objects.ToList().Find(x => x is GameObject == null)) {
                            Utils.RenameElements(objectName, Selection.objects);
                        }
                        else {
                            Utils.RenameElements(objectName, Utils.SortGameObjectSelection(Selection.gameObjects));
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ObjectSelected,
                        button);
                    break;
            }

            return button;
        }
        
        private static string GetSelectedObjectDirectory(UnityEngine.Object obj)
        {
            string emptyPath = "";
            
            if (obj == null){
                return emptyPath;
            }
 
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (assetPath.Length > 0)
            {
                if (File.Exists(assetPath))
                {
                    return Path.GetDirectoryName(assetPath);
                }
            }

            string directoryPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (directoryPath.Length > 0)
            {
                if (Directory.Exists(directoryPath))
                {
                    return directoryPath;
                }
            }

            return emptyPath;
        }
    }
}