using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt.Maestro
{
    public class EditRectTransformComponent : ChildUIElementsWindow
    {
        static PageBuilderWindow pageBuilderWindow;
        static Foldout elementUXML;

        public override ChildUIElementsWindow Init(EditorWindow parentWindow)
        {
            pageBuilderWindow = parentWindow as PageBuilderWindow;
            VisualElement parentVisualElement = parentWindow.rootVisualElement;

            elementUXML = parentVisualElement.Query<Foldout>("EditRectTransformComponent", EditorToolsCore.ToggleableGroup);

            var propertyFields = elementUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = elementUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            PageBuilderWindow.selectionChangedDelegate += UpdateDisplay;

            return this;
        }

        void OnDestroy()
        {
            PageBuilderWindow.selectionChangedDelegate -= UpdateDisplay;
        }

        static VisualElementToggleData toggleData = new VisualElementToggleData();

        public Vector3 position;
        public Vector3 positionInterval;
        public bool setIntervalOnValueChange;
        public Vector3 transposePosition;
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public Vector3 rotation;
        public float width = 1;
        public float height = 1;

        static bool populateButtonPressed = false;

        enum PropertyFieldNames
        {
            Position,
            TransposePosition,
            PositionInterval,
            SetIntervalOnValueChange,
            Pivot,
            Rotation,
            Width,
            Height
        }

        enum ButtonNames
        {
            PopulatePosition,
            SetPosition,
            SetPositionUsingInterval,
            PopulatePivot,
            SetPivot,
            PopulateRotation,
            SetRotation,
            PopulateWidth,
            SetWidth,
            PopulateHeight,
            SetHeight,
            AddResponsivePosition,
            AddResponsiveAutoWidthHeight
        }

        enum EnableCondition
        {
            MultipleObjectsSelected
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                if (Selection.gameObjects[i].GetComponent<RectTransform>() != null) {
                    dependencySelected = true;
                    break;
                }
            }

            elementUXML.SetEnabled(dependencySelected);
            elementUXML.value = dependencySelected;

            if (Selection.gameObjects.Length > 1) {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.MultipleObjectsSelected, true);
            } else {
                EditorToolsCore.ToggleVisualElements(toggleData, EnableCondition.MultipleObjectsSelected, false);
            }
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.Position):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if(populateButtonPressed == false) {
                            SetPosition(Selection.gameObjects, position);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.PositionInterval):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if(setIntervalOnValueChange == true) {
                            SetPositionUsingInterval(Selection.gameObjects, position, positionInterval);
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.SetIntervalOnValueChange):
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleObjectsSelected, propertyField);
                    break;

                case nameof(PropertyFieldNames.TransposePosition):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                            if (rectTransform != null) {
                                Undo.RecordObject(rectTransform, "tranpose rect position");
                                rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x + transposePosition.x, rectTransform.anchoredPosition3D.y + transposePosition.y, rectTransform.anchoredPosition3D.z + transposePosition.z);
                            }
                        }
                        transposePosition = new Vector3(0, 0, 0);
                    });
                    break;

                case nameof(PropertyFieldNames.Pivot):
                    propertyField.RegisterCallback<ChangeEvent<Vector2>>((ChangeEvent<Vector2> evt) => {
                        if (populateButtonPressed == false) {
                            SetPivot(Selection.gameObjects, pivot);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.Rotation):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        if (populateButtonPressed == false) {
                            SetRotation(Selection.gameObjects, rotation);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.Width):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (populateButtonPressed == false) {
                            SetWidth(Selection.gameObjects, width);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.Height):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (populateButtonPressed == false) {
                            SetHeight(Selection.gameObjects, height);
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

                case nameof(ButtonNames.PopulatePosition):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        position = GetObjectPositionFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetPosition):
                    button.clickable.clicked += () => {
                        SetPosition(Selection.gameObjects, position);
                    };
                    break;

                case nameof(ButtonNames.SetPositionUsingInterval):
                    button.clickable.clicked += () => {
                        SetPositionUsingInterval(Selection.gameObjects, position, positionInterval);
                    };
                    EditorToolsCore.AddToVisualElementToggleData(toggleData, EnableCondition.MultipleObjectsSelected, button);
                    break;

                case nameof(ButtonNames.PopulatePivot):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        pivot = GetObjectPivotFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetPivot):
                    button.clickable.clicked += () => {
                        SetPivot(Selection.gameObjects, pivot);
                    };
                    break;

                case nameof(ButtonNames.PopulateRotation):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        rotation = GetObjectRotationFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetRotation):
                    button.clickable.clicked += () => {
                        SetRotation(Selection.gameObjects, rotation);
                    };
                    break;

                case nameof(ButtonNames.PopulateWidth):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        width = GetObjectWidthFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetWidth):
                    button.clickable.clicked += () => {
                        SetWidth(Selection.gameObjects, width);
                    };
                    break;

                case nameof(ButtonNames.PopulateHeight):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        height = GetObjectHeightFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetHeight):
                    button.clickable.clicked += () => {
                        SetHeight(Selection.gameObjects, height);
                    };
                    break;

                case nameof(ButtonNames.AddResponsivePosition):
                    button.clickable.clicked += () => {
                        PageBuilderCore.AddComponentToSelection(Selection.gameObjects, typeof(ResponsivePosition));
                    };
                    break;

                case nameof(ButtonNames.AddResponsiveAutoWidthHeight):
                    button.clickable.clicked += () => {
                        PageBuilderCore.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveAutoWidthHeight));
                    };
                    break;
            }

            return button;
        }

        public static Vector3 GetObjectPositionFromSelection(GameObject[] objectSelection)
        {
            Vector3 value = Vector3.zero;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform component = objectSelection[i].GetComponent<RectTransform>();
                if (component != null) {
                    value = component.anchoredPosition3D;
                    break;
                }
            }

            return value;
        }

        public static RectTransform[] SetPosition(GameObject[] objectSelection, Vector3 targetValue)
        {
            List<RectTransform> componentList = new List<RectTransform>();

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect position");
                    componentList.Add(rectTransform);
                    rectTransform.anchoredPosition3D = targetValue;
                }
            }

            return componentList.ToArray();
        }

        public static RectTransform[] SetPositionUsingInterval(GameObject[] objectSelection, Vector3 sourcePosition, Vector3 interval)
        {
            List<RectTransform> componentList = new List<RectTransform>();
            Array.Sort(objectSelection, new Utils.GameObjectSort());
            Vector3 newPosition = sourcePosition;

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect interval position");
                    componentList.Add(rectTransform);
                    rectTransform.anchoredPosition3D = newPosition;
                    newPosition += interval;
                }
            }

            return componentList.ToArray();
        }

        public static Vector2 GetObjectPivotFromSelection(GameObject[] objectSelection)
        {
            Vector2 value = Vector2.zero;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform component = objectSelection[i].GetComponent<RectTransform>();
                if (component != null) {
                    value = component.pivot;
                    break;
                }
            }

            return value;
        }

        public static RectTransform[] SetPivot(GameObject[] objectSelection, Vector2 targetValue)
        {
            List<RectTransform> componentList = new List<RectTransform>();

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect pivot");
                    componentList.Add(rectTransform);
                    rectTransform.pivot = targetValue;
                }
            }

            return componentList.ToArray();
        }

        public static Vector2 GetObjectRotationFromSelection(GameObject[] objectSelection)
        {
            Vector3 value = Vector3.zero;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform component = objectSelection[i].GetComponent<RectTransform>();
                if (component != null) {
                    value = component.localEulerAngles;
                    break;
                }
            }

            return value;
        }

        public static RectTransform[] SetRotation(GameObject[] objectSelection, Vector3 targetValue)
        {
            List<RectTransform> componentList = new List<RectTransform>();

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect rotation");
                    componentList.Add(rectTransform);
                    rectTransform.localEulerAngles = targetValue;
                }
            }

            return componentList.ToArray();
        }

        public static float GetObjectWidthFromSelection(GameObject[] objectSelection)
        {
            float value = float.NaN;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform component = objectSelection[i].GetComponent<RectTransform>();
                if (component != null) {
                    value = component.sizeDelta.x;
                    break;
                }
            }

            return value;
        }

        public static RectTransform[] SetWidth(GameObject[] objectSelection, float targetValue)
        {
            List<RectTransform> componentList = new List<RectTransform>();

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect width");
                    componentList.Add(rectTransform);
                    rectTransform.sizeDelta = new Vector2(targetValue, rectTransform.sizeDelta.y);
                }
            }

            return componentList.ToArray();
        }

        public static float GetObjectHeightFromSelection(GameObject[] objectSelection)
        {
            float value = float.NaN;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform component = objectSelection[i].GetComponent<RectTransform>();
                if (component != null) {
                    value = component.sizeDelta.y;
                    break;
                }
            }

            return value;
        }

        public static RectTransform[] SetHeight(GameObject[] objectSelection, float targetValue)
        {
            List<RectTransform> componentList = new List<RectTransform>();

            for (int i = 0; i < objectSelection.Length; i++) {
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Undo.RecordObject(rectTransform, "set rect height");
                    componentList.Add(rectTransform);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetValue);
                }
            }

            return componentList.ToArray();
        }
    }
}
