using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    public class EditTMProComponent : ChildModuleWindow
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

        public string textContent;
        static string renamePrefix = "tx - ";
        public float fontSize = 1.55f;
        public Color fontColor;
        public float breakpoint = 1.78f;
        public string targetLayer = "";
        public int targetOrder;

        static bool populateButtonPressed = false;

        enum ButtonNames
        {
            PopulateTextContent,
            SetTextContent,
            RenameUsingContent,
            PopulateFontSize,
            SetFontSize,
            PopulateFontColor,
            SetFontColor,
            SetOpaque,
            SetTransparent,
            TrimTextBox,
            AlignLeft,
            AlignCenter,
            AlignRight,
            Justify,
            Flush,
            AddResponsiveTextSize,
            SetSortingLayer,
            SetSortingOrder
        }

        enum PropertyFieldNames
        {
            TextContent,
            FontSize,
            FontColor
        }

        void UpdateDisplay()
        {
            bool dependencySelected = false;

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                if (Selection.gameObjects[i].GetComponent<TMP_Text>() != null) {
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

                case nameof(PropertyFieldNames.TextContent):
                    propertyField.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) => {
                        if (populateButtonPressed == false) {
                            SetTextContent(Selection.gameObjects, textContent);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.FontSize):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        if (populateButtonPressed == false) {
                            SetFontSize(Selection.gameObjects, fontSize);
                        }
                        populateButtonPressed = false;
                    });
                    break;

                case nameof(PropertyFieldNames.FontColor):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        if (populateButtonPressed == false) {
                            SetFontColor(Selection.gameObjects, fontColor);
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

                case nameof(ButtonNames.PopulateTextContent):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        textContent = GetTextContentFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetTextContent):
                    button.clickable.clicked += () => {
                        SetTextContent(Selection.gameObjects, textContent);
                    };
                    break;

                case nameof(ButtonNames.RenameUsingContent):
                    button.clickable.clicked += () => {
                        CallRenameTextUsingContent();
                    };
                    break;

                case nameof(ButtonNames.PopulateFontSize):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        fontSize = GetFontSizeFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetFontSize):
                    button.clickable.clicked += () => {
                        SetFontSize(Selection.gameObjects, fontSize);
                    };
                    break;

                case nameof(ButtonNames.PopulateFontColor):
                    button.clickable.clicked += () => {
                        populateButtonPressed = true;
                        fontColor = GetFontColorFromSelection(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.SetFontColor):
                    button.clickable.clicked += () => {
                        SetFontColor(Selection.gameObjects, fontColor);
                    };
                    break;

                case nameof(ButtonNames.SetOpaque):
                    button.clickable.clicked += () => {
                        SetFontAlpha(Selection.gameObjects, 1);
                    };
                    break;

                case nameof(ButtonNames.SetTransparent):
                    button.clickable.clicked += () => {
                        SetFontAlpha(Selection.gameObjects, 0);
                    };
                    break;

                case nameof(ButtonNames.AlignLeft):
                    button.clickable.clicked += () => {
                        AlignText(Selection.gameObjects, TextAlignmentOptions.TopLeft);
                    };
                    break;

                case nameof(ButtonNames.AlignCenter):
                    button.clickable.clicked += () => {
                        AlignText(Selection.gameObjects, TextAlignmentOptions.Top);
                    };
                    break;

                case nameof(ButtonNames.AlignRight):
                    button.clickable.clicked += () => {
                        AlignText(Selection.gameObjects, TextAlignmentOptions.TopRight);
                    };
                    break;

                case nameof(ButtonNames.Justify):
                    button.clickable.clicked += () => {
                        AlignText(Selection.gameObjects, TextAlignmentOptions.TopJustified);
                    };
                    break;

                case nameof(ButtonNames.Flush):
                    button.clickable.clicked += () => {
                        AlignText(Selection.gameObjects, TextAlignmentOptions.Flush);
                    };
                    break;

                case nameof(ButtonNames.TrimTextBox):
                    button.clickable.clicked += () => {
                        TrimTextBox(Selection.gameObjects);
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

                case nameof(ButtonNames.AddResponsiveTextSize):
                    button.clickable.clicked += () => {
                        ModuleUtils.AddComponentToSelection(Selection.gameObjects, typeof(ResponsiveTextSize));
                    };
                    break;
            }

            return button;
        }

        [MenuItem("Edit/Maestro/Layout/Rename Text Object", false, 0)]
        public static void CallRenameTextUsingContent()
        {
            RenameTextUsingContent(Selection.gameObjects, renamePrefix);
        }

        public static string GetTextContentFromSelection(GameObject[] objectSelection)
        {
            string value = null;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    value = component.text;
                    break;
                }
            }

            return value;
        }

        public static TMP_Text[] SetTextContent(GameObject[] objectSelection, string targetValue)
        {
            List<TMP_Text> componentList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    Undo.RecordObject(component, "set text content");
                    componentList.Add(component);
                    component.text = targetValue;
                }
            }

            return componentList.ToArray();
        }

        public static GameObject[] RenameTextUsingContent(GameObject[] objectSelection, string prefix)
        {
            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    Undo.RecordObject(component, "rename text using content");
                    string newName;
                    if (component.text.Length < 30) {
                        newName = prefix + component.text.Substring(0, component.text.Length);
                    } else {
                        newName = prefix + component.text.Substring(0, 30);
                    }
                    component.gameObject.name = newName;
                }
            }

            return objectSelection;
        }

        public static float GetFontSizeFromSelection(GameObject[] objectSelection)
        {
            float value = float.NaN;
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    value = component.fontSize;
                    break;
                }
            }

            return value;
        }

        public static TMP_Text[] SetFontSize(GameObject[] objectSelection, float targetSize)
        {
            List<TMP_Text> componentList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    Undo.RecordObject(component, "set font size");
                    componentList.Add(component);
                    component.fontSize = targetSize;
                }
            }

            return componentList.ToArray();
        }

        public static Color GetFontColorFromSelection(GameObject[] objectSelection)
        {
            Color value = new Color(0,0,0,0);
            Array.Sort(objectSelection, new Utils.GameObjectSort());

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text component = objectSelection[i].GetComponent<TMP_Text>();
                if (component != null) {
                    value = component.color;
                    break;
                }
            }

            return value;
        }

        public static TMP_Text[] SetFontColor(GameObject[] objectSelection, Color targetColor)
        {
            List<TMP_Text> componentList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text textMeshPro = objectSelection[i].GetComponent<TMP_Text>();
                if (textMeshPro != null) {
                    Undo.RecordObject(textMeshPro, "set font color");
                    componentList.Add(textMeshPro);
                    textMeshPro.color = targetColor;
                }
            }

            return componentList.ToArray();
        }

        public static TMP_Text[] SetFontAlpha(GameObject[] objectSelection, float targetAlpha)
        {
            List<TMP_Text> componentList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text textMeshPro = objectSelection[i].GetComponent<TMP_Text>();
                if (textMeshPro != null) {
                    Undo.RecordObject(textMeshPro, "set font alpha");
                    componentList.Add(textMeshPro);
                    textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, targetAlpha);
                }
            }

            return componentList.ToArray();
        }

        public static TMP_Text[] AlignText(GameObject[] objectSelection, TextAlignmentOptions textAlignment)
        {
            List<TMP_Text> componentList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text textMeshPro = objectSelection[i].GetComponent<TMP_Text>();
                if (textMeshPro != null) {
                    Undo.RecordObject(textMeshPro, "set text alignment");
                    componentList.Add(textMeshPro);
                    textMeshPro.alignment = textAlignment;
                }
            }

            return componentList.ToArray();
        }

        public static TMP_Text[] TrimTextBox(GameObject[] objectSelection)
        {
            List<TMP_Text> textList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                TMP_Text textMeshPro = objectSelection[i].GetComponent<TMP_Text>();
                RectTransform rectTransform = objectSelection[i].GetComponent<RectTransform>();

                if (textMeshPro != null && rectTransform != null) {
                    Undo.RecordObject(rectTransform, "trim text box(s)");
                    textList.Add(textMeshPro);

                    TMP_TextInfo textInfo = textMeshPro.textInfo;
                    int longestLineID = 0;
                    float longestLineCharCount = 0;

                    for(int q=0; q<textInfo.lineCount; q++) {
                        if(textInfo.lineInfo[q].characterCount > longestLineCharCount) {
                            longestLineID = q;
                        }
                    }

                    float width = textInfo.lineInfo[longestLineID].length + .02f;
                    float height = textInfo.lineCount * textInfo.lineInfo[longestLineID].lineHeight;

                    rectTransform.sizeDelta = new Vector2(width, height);
                    textMeshPro.ForceMeshUpdate();
                }
            }
            return textList.ToArray();
        }

        public static TMP_Text[] SetSortingLayer(GameObject[] objectSelection, string targetSortingLayer)
        {
            List<TMP_Text> textList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if(objectSelection[i].TryGetComponent(typeof(TMP_Text), out Component textMeshPro) && objectSelection[i].TryGetComponent(typeof(MeshRenderer), out Component meshRendererComponent))  {
                    textList.Add(textMeshPro as TMP_Text);

                    MeshRenderer meshRenderer = meshRendererComponent as MeshRenderer;

                    Undo.RecordObject(meshRenderer, "set sorting layer(s)");
                    meshRenderer.sortingLayerName = targetSortingLayer;
                }
            }

            return textList.ToArray();
        }

        public static TMP_Text[] SetSortingOrder(GameObject[] objectSelection, int targetSortingOrder)
        {
            List<TMP_Text> textList = new List<TMP_Text>();

            for (int i = 0; i < objectSelection.Length; i++) {
                if (objectSelection[i].TryGetComponent(typeof(TMP_Text), out Component textMeshPro) && objectSelection[i].TryGetComponent(typeof(MeshRenderer), out Component meshRendererComponent)) {
                    textList.Add(textMeshPro as TMP_Text);

                    MeshRenderer meshRenderer = meshRendererComponent as MeshRenderer;
                    Undo.RecordObject(meshRenderer, "set sorting order(s)");

                    meshRenderer.sortingOrder = targetSortingOrder;
                }
            }

            return textList.ToArray();
        }
    }
}
