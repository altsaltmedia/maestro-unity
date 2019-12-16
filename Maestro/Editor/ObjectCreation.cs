using System;
using System.Collections.Generic;
using System.Linq;
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

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        private VisualElementToggleData toggleData = new VisualElementToggleData();

        public string objectName = "";
        public bool selectCreatedObject;
        
        enum ButtonNames
        {
            RenameElements,
        }
        
        enum EnableCondition
        {
            ObjectSelected
        }

        void UpdateDisplay()
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
                        if (Selection.objects.ToList().Find(x => x is GameObject == null) != null) {
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
    }
}