using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    public class EditSpriteComponent : ChildModuleWindow
    {
        protected override ChildModuleWindow Configure(ParentModuleWindow parentModuleWindow, string childRootUXMLName)
        {
            base.Configure(parentModuleWindow, childRootUXMLName);

            var propertyFields = moduleChildUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleChildUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }

        public Color spriteColor;
        public string targetLayer = "";
        public int targetOrder;

        static bool populateButtonPressed = false;

        enum ButtonNames
        {
            PopulateSpriteColor,
            SetSpriteColor,
            SetOpaque,
            SetTransparent,
            SetSortingLayer,
            SetSortingOrder
        }

        enum PropertyFieldNames
        {
            SpriteColor
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                if (Selection.gameObjects[i].GetComponent<SpriteRenderer>() != null) {
                    dependencySelected = true;
                    break;
                }
            }

            moduleChildUXML.SetEnabled(dependencySelected);
            moduleChildUXML.value = dependencySelected;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.SpriteColor):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if (populateButtonPressed == false) {
                            SetSpriteColor(Selection.gameObjects, spriteColor);
                        }
                        populateButtonPressed = false;
                    });
                    break;
            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.PopulateSpriteColor):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        spriteColor = GetColorFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetSpriteColor):
                    button.clickable.clicked += () => {
                        SetSpriteColor(Selection.gameObjects, spriteColor);
                    };
                    break;

                case nameof(ButtonNames.SetOpaque):
                    button.clickable.clicked += () => {
                        SetSpriteAlpha(Selection.gameObjects, 1);
                    };
                    break;

                case nameof(ButtonNames.SetTransparent):
                    button.clickable.clicked += () => {
                        SetSpriteAlpha(Selection.gameObjects, 0);
                    };
                    break;

                case nameof(ButtonNames.SetSortingLayer):
                    button.clickable.clicked += () => {
                        SetSortingLayer(Selection.gameObjects, targetLayer);
                    };
                    break;

                case nameof(ButtonNames.SetSortingOrder):
                    button.clickable.clicked += () => {
                        SetSortingOrder(Selection.gameObjects, targetOrder);
                    };
                    break;
            }

            return button;
        }

        public static Color GetColorFromSelection(GameObject[] objectSelection)
        {
            Color value = new Color(0, 0, 0, 0);
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                SpriteRenderer component = objectSelection[i].GetComponent<SpriteRenderer>();
                if (component != null) {
                    value = component.color;
                    break;
                }
            }

            return value;
        }

        public static SpriteRenderer[] SetSpriteColor(GameObject[] objectSelection, Color targetColor)
        {
            List<SpriteRenderer> componentList = new List<SpriteRenderer>();

            for (int i = 0; i < objectSelection.Length; i++) {
                SpriteRenderer component = objectSelection[i].GetComponent<SpriteRenderer>();
                if (component != null) {
                    Undo.RecordObject(component, "set sprite color");
                    componentList.Add(component);
                    component.color = targetColor;
                }
            }

            return componentList.ToArray();
        }

        public static SpriteRenderer[] SetSpriteAlpha(GameObject[] objectSelection, float targetAlpha)
        {
            List<SpriteRenderer> componentList = new List<SpriteRenderer>();

            for (int i = 0; i < objectSelection.Length; i++) {
                SpriteRenderer component = objectSelection[i].GetComponent<SpriteRenderer>();
                if (component != null) {
                    Undo.RecordObject(component, "set sprite alpha");
                    componentList.Add(component);
                    component.color = new Color(component.color.r, component.color.g, component.color.b, targetAlpha);
                }
            }

            return componentList.ToArray();
        }

        public static SpriteRenderer[] SetSortingLayer(GameObject[] objectSelection, string targetSortingLayer)
        {
            List<SpriteRenderer> componentList = new List<SpriteRenderer>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if (objectSelection[i].TryGetComponent(typeof(SpriteRenderer), out Component component)) {
                    componentList.Add(component as SpriteRenderer);

                    SpriteRenderer spriteRenderer = component as SpriteRenderer;

                    Undo.RecordObject(spriteRenderer, "set sorting layer(s)");
                    spriteRenderer.sortingLayerName = targetSortingLayer;
                }
            }

            return componentList.ToArray();
        }

        public static SpriteRenderer[] SetSortingOrder(GameObject[] objectSelection, int targetSortingOrder)
        {
            List<SpriteRenderer> componentList = new List<SpriteRenderer>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if (objectSelection[i].TryGetComponent(typeof(SpriteRenderer), out Component component)) {
                    componentList.Add(component as SpriteRenderer);

                    SpriteRenderer spriteRenderer = component as SpriteRenderer;
                    Undo.RecordObject(spriteRenderer, "set sorting order(s)");

                    spriteRenderer.sortingOrder = targetSortingOrder;
                }
            }

            return componentList.ToArray();
        }
    }
}
