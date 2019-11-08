﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

namespace AltSalt.Maestro
{
    public class EditScrollSnapController : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout editWindowUXML;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            editWindowUXML = parentVisualElement.Query<Foldout>("EditScrollSnapController", EditorToolsCore.ToggleableGroup);

            var propertyFields = editWindowUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = editWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        static bool populateButtonPressed = false;

        public bool addIcon = true;
        public float iconInterval = .3f;

        enum ButtonNames
        {
            AddPanel,
            SetIconInterval
        }

        enum PropertyFieldNames
        {

        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                if (Selection.gameObjects[i].GetComponent<ScrollSnapController>() != null) {
                    dependencySelected = true;
                    break;
                }
            }

            editWindowUXML.SetEnabled(dependencySelected);
            editWindowUXML.value = dependencySelected;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {


            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.AddPanel):
                    button.clickable.clicked += () => {
                        AddPanel(Selection.gameObjects, PageBuilderCore.PrefabReferences.responsiveContainerLeftAnchorPrefab, PageBuilderCore.PrefabReferences.scrollSnapIcon, addIcon, iconInterval);
                    };
                    break;

                case nameof(ButtonNames.SetIconInterval):
                    button.clickable.clicked += () => {
                        CallSetIconInterval(Selection.gameObjects, iconInterval);
                    };
                    break;
            }

            return button;
        }

        static ScrollSnapController[] AddPanel(GameObject[] objectSelection, GameObject panelObject, GameObject iconObject, bool addIcon, float iconInterval)
        {
            List<ScrollSnapController> componentList = new List<ScrollSnapController>();

            for (int i = 0; i < objectSelection.Length; i++) {
                ScrollSnapController component = objectSelection[i].GetComponent<ScrollSnapController>();
                componentList.Add(component);

                if (component != null) {

                    // Create new element and set transform parent
                    GameObject dependencyGameObject = PrefabUtility.InstantiatePrefab(panelObject) as GameObject;
                    Undo.RegisterCreatedObjectUndo(dependencyGameObject, "create panel");
                    Undo.SetTransformParent(dependencyGameObject.transform, component.Content, "set new panel parent");

                    // Reset the new element position
                    dependencyGameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                    RelativePage relativePage = dependencyGameObject.AddComponent<RelativePage>();
                    relativePage.referenceObject = component.Content.GetChild(dependencyGameObject.transform.GetSiblingIndex() - 1) as RectTransform;

                    ScrollSnapController.ScrollSnapElement dependencyData;
                    SpriteUtils iconSprite = null;

                    if (addIcon == true) {
                        // Create and parent an icon to go with the panel
                        GameObject iconElement = PrefabUtility.InstantiatePrefab(iconObject) as GameObject;
                        Undo.RegisterCreatedObjectUndo(iconElement, "create icon");
                        Undo.SetTransformParent(iconElement.transform, component.SnapUtils.IconContainer, "set icon parent");

                        // Reset icon positions on the component
                        SetIconInterval(iconInterval, component.SnapUtils);

                        // Set icon sprite to be saved onto the component
                        iconSprite = iconElement.GetComponentInChildren<SpriteUtils>();
                    }

                    // Initialize dependency
                    dependencyData = new ScrollSnapController.ScrollSnapElement(dependencyGameObject.GetComponent<RectTransform>(), iconSprite);

                    // Get reference to the serialized attribute on the component and create new array element
                    var serializedComponent = new SerializedObject(component);
                    SerializedProperty serializedProperty = serializedComponent.FindProperty(nameof(component.scrollSnapElements));
                    serializedProperty.arraySize++;
                    var serializedArrayElement = serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1);

                    // Replace array element values with properties of our new created element
                    serializedArrayElement.FindPropertyRelative(nameof(dependencyData.sourceTransform)).objectReferenceValue = dependencyData.sourceTransform as RectTransform;
                    serializedArrayElement.FindPropertyRelative(nameof(dependencyData.elementIcon)).objectReferenceValue = dependencyData.elementIcon as SpriteUtils;
                    serializedArrayElement.FindPropertyRelative(nameof(dependencyData.iconEnabledColor)).colorValue = dependencyData.iconEnabledColor;
                    serializedArrayElement.FindPropertyRelative(nameof(dependencyData.iconDisabledColor)).colorValue = dependencyData.iconDisabledColor;
                    serializedComponent.ApplyModifiedProperties();
                }
            }

            return componentList.ToArray();
        }

        static ScrollSnapUtils[] CallSetIconInterval(GameObject[] objectSelection, float iconInterval)
        {
            List<ScrollSnapUtils> componentList = new List<ScrollSnapUtils>();

            for (int i = 0; i < objectSelection.Length; i++) {

                ScrollSnapController component = objectSelection[i].GetComponent<ScrollSnapController>();
                ScrollSnapUtils scrollSnapUtils = component.SnapUtils;
                componentList.Add(scrollSnapUtils);

                SetIconInterval(iconInterval, scrollSnapUtils);
            }

            return componentList.ToArray();
        }

        static ScrollSnapUtils SetIconInterval(float iconInterval, ScrollSnapUtils targetSnapUtils)
        {
            int iconCount = targetSnapUtils.IconContainer.childCount;
            int elementCount = iconCount + 2;

            List<RectTransform> childElements = new List<RectTransform>();
            childElements.Add(targetSnapUtils.PrevBtn.GetComponent<RectTransform>());

            for(int i=0; i<iconCount; i++) {
                childElements.Add(targetSnapUtils.IconContainer.GetChild(i) as RectTransform);
            }

            childElements.Add(targetSnapUtils.NextBtn.GetComponent<RectTransform>());

            for (int i = 0; i < elementCount; i++) {

                float modifiedIndex = i - ((elementCount - 1f) / 2);

                Undo.RecordObject(childElements[i], "set element position");

                childElements[i].anchoredPosition3D = new Vector3(modifiedIndex * iconInterval, 0, 0);
            }

            return targetSnapUtils;
        }
    }
}