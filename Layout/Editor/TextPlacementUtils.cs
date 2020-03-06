using System;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    public class TextPlacementUtils : ModuleWindow
    {
        static TextPlacementUtils textPlacementUtils;

        public TMP_Text onionSkin;
        public bool toggleOnionSkin;

        [SerializeField]
        public string openingWrapper;

        [SerializeField]
        public string closingWrapper;

        [MenuItem("Tools/Maestro/Text Placement Utils")]
        public static void ShowWindow()
        {
            var moduleWindow = CreateInstance<TextPlacementUtils>();
            moduleWindow.Init();
            moduleWindow.Show();
        }

        private void Init()
        {
            titleContent = new GUIContent("Text Placement Utils");
            ModuleWindow moduleWindow = Configure(null, ProjectNamespaceData.namespaceData[ModuleNamespace.Layout].editorPath + nameof(TextPlacementUtils)+"_UXML.uxml");
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Utils.stylesheetPath);
            rootVisualElement.styleSheets.Add(styleSheet);
            rootVisualElement.AddToClassList("altsalt");
            
            rootVisualElement.Add(moduleWindow.moduleWindowUXML);
        }

        enum ButtonNames
        {
            PopulateOnionSkin,
            PositionOnionSkin,
            SelectOnionSkin,
            ExtractText,
            ExtractTextLetters,
            SliceText,
            ExtractTextLines,
            RefreshLayout,
            AlignLeft,
            AlignCenter,
            AlignRight,
            Justify,
            Flush
        }

        enum PropertyFieldNames
        {
            OnionSkin,
            ToggleOnionSkin
        }

        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            textPlacementUtils = this;

            var propertyFields = moduleWindowUXML.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            return this;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.ToggleOnionSkin):
                    if(onionSkin != null) {
                        toggleOnionSkin = onionSkin.gameObject.activeSelf;
                    }
                    propertyField.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) => {
                        onionSkin.gameObject.SetActive(toggleOnionSkin);
                    });
                    break;
            }

            return propertyField;
        }

        Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.PopulateOnionSkin):
                    button.clickable.clicked += () => {
                        PopulateOnionSkin();
                    };
                    break;

                case nameof(ButtonNames.PositionOnionSkin):
                    button.clickable.clicked += () => {
                        if(Selection.gameObjects.Length >= 0) {
                            RectTransform newPosition = PositionObject(onionSkin.gameObject, Selection.gameObjects[0]);
                        }
                    };
                    break;

                case nameof(ButtonNames.SelectOnionSkin):
                    button.clickable.clicked += () => {
                        SelectOnionSkin();
                    };
                    break;

                case nameof(ButtonNames.AlignLeft):
                    button.clickable.clicked += () => {
                        EditTMProComponent.AlignText(new GameObject[] { onionSkin.gameObject }, TextAlignmentOptions.TopLeft);
                    };
                    break;

                case nameof(ButtonNames.AlignCenter):
                    button.clickable.clicked += () => {
                        EditTMProComponent.AlignText(new GameObject[] { onionSkin.gameObject }, TextAlignmentOptions.Top);
                    };
                    break;

                case nameof(ButtonNames.AlignRight):
                    button.clickable.clicked += () => {
                        EditTMProComponent.AlignText(new GameObject[] { onionSkin.gameObject }, TextAlignmentOptions.TopRight);
                    };
                    break;

                case nameof(ButtonNames.Justify):
                    button.clickable.clicked += () => {
                        EditTMProComponent.AlignText(new GameObject[] { onionSkin.gameObject }, TextAlignmentOptions.TopJustified);
                    };
                    break;

                case nameof(ButtonNames.Flush):
                    button.clickable.clicked += () => {
                        EditTMProComponent.AlignText(new GameObject[] { onionSkin.gameObject }, TextAlignmentOptions.Flush);
                    };
                    break;

                case nameof(ButtonNames.ExtractText):
                    button.clickable.clicked += () => {
                        TMP_Text[] textSelection = GetTextComponents(Selection.gameObjects);
                        GUIUtility.systemCopyBuffer = ExtractText(textSelection);
                        Debug.Log("Copied string to clipboard");
                    };
                    break;

                case nameof(ButtonNames.ExtractTextLetters):
                    button.clickable.clicked += () => {
                        TMP_Text[] textSelection = GetTextComponents(Selection.gameObjects);
                        GUIUtility.systemCopyBuffer = ExtractTextByLetters(textSelection, openingWrapper, closingWrapper);
                        Debug.Log("Copied string to clipboard");
                    };
                    break;
                
                case nameof(ButtonNames.SliceText):
                    button.clickable.clicked += () => {
                        TMP_Text[] textSelection = GetTextComponents(Selection.gameObjects);
                        Selection.objects = SliceText(textSelection);
                        Debug.Log("Sliced text");
                    };
                    break;
                
                case nameof(ButtonNames.ExtractTextLines):
                    button.clickable.clicked += () => {
                        TMP_Text[] textSelection = GetTextComponents(Selection.gameObjects);
                        GUIUtility.systemCopyBuffer = ExtractTextByLines(textSelection, openingWrapper, closingWrapper);
                        Debug.Log("Copied string to clipboard");
                    };
                    break;

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        //Configure();
                    };
                    break;
            }

            return button;
        }

        [MenuItem("Edit/Maestro/Text Placement Utils/Toggle Onion Skin", false, 0)]
        public static TMP_Text ToggleOnionSkin()
        {
            textPlacementUtils.toggleOnionSkin = !textPlacementUtils.toggleOnionSkin;
            textPlacementUtils.onionSkin.gameObject.SetActive(textPlacementUtils.toggleOnionSkin);
            return textPlacementUtils.onionSkin;
        }

        [MenuItem("Edit/Maestro/Text Placement Utils/Populate Onion Skin", false, 0)]
        public static TMP_Text PopulateOnionSkin()
        {
            TMP_Text[] textSelection = GetTextComponents(Selection.gameObjects);
            string fullText = ExtractText(textSelection);
            textPlacementUtils.onionSkin.SetText(fullText);
            return textPlacementUtils.onionSkin;
        }

        [MenuItem("Edit/Maestro/Text Placement Utils/Select Onion Skin", false, 0)]
        public static TMP_Text SelectOnionSkin()
        {
            Selection.activeGameObject = textPlacementUtils.onionSkin.gameObject;
            return textPlacementUtils.onionSkin;
        }

        public static RectTransform PositionObject(GameObject selectedObject, GameObject targetObject)
        {
            RectTransform selectedTransform = selectedObject.GetComponent<RectTransform>();
            RectTransform targetTransform = targetObject.GetComponent<RectTransform>();

            if(selectedTransform != null && targetTransform != null) {
                selectedTransform.anchoredPosition3D = targetTransform.anchoredPosition3D;
                selectedTransform.pivot = targetTransform.pivot;
                selectedTransform.sizeDelta = targetTransform.sizeDelta;
            }

            return selectedTransform;
        }

        public static TMP_Text[] GetTextComponents(GameObject[] selection) {
            Array.Sort(selection, new Utils.GameObjectSort());

            UnityEngine.Object[] culledSelectionRaw = Utils.FilterSelection(selection, typeof(TMP_Text));
            var culledSelection = Array.ConvertAll(culledSelectionRaw, item => (GameObject)item);

            List<TMP_Text> components = new List<TMP_Text>();
            for(int i=0; i<culledSelection.Length; i++) {
                components.Add(culledSelection[i].GetComponent<TMP_Text>());
            }
            
            return components.ToArray();
        }

        public static string ExtractText(TMP_Text[] selection)
        {
            string fullString = "";

            for (int z = 0; z < selection.Length; z++) {
                fullString += selection[z].text + " "; 
            }

            return fullString;
        }
        
        public static string ExtractTextByLetters(TMP_Text[] selection, string openingWrapper, string closingWrapper)
        {
            string fullString = "";

            for(int z=0; z<selection.Length; z++) {

                TMP_Text textRenderer = selection[z];

                for (int i = 0; i < textRenderer.textInfo.characterCount; i++) {

                    string character = textRenderer.text.Substring(i, 1);
                    
                    if (string.IsNullOrEmpty(character.Trim())) {
                        continue;
                    }
                    
                    string characterString = openingWrapper + "\n";
                    characterString += ("\t" + character + "\n");
                    characterString += closingWrapper + "\n";

                    fullString += characterString;                    
                }
            }

            return fullString;
        }

        public static GameObject[] SliceText(TMP_Text[] selection)
        {
            List<Vector3> letterPositions = new List<Vector3>();
            List<GameObject> createdObjects = new List<GameObject>();
            
            for(int z=0; z<selection.Length; z++) {

                TMP_Text textRenderer = selection[z];

                List<string> characterList = new List<string>();

                for (int j = 0; j < textRenderer.textInfo.characterCount; j++) {

                    string character = textRenderer.text.Substring(j, 1);

                    if (string.IsNullOrEmpty(character.Trim())) {
                        continue;
                    }
                    
                    characterList.Add(character);
                }

                for (int i = textRenderer.textInfo.characterInfo.Length - 1; i >= 0; i--) {

                    if (textRenderer.textInfo.characterInfo[i].isVisible == false) {
                        continue;
                    }
                    
                    TMP_CharacterInfo characterInfo =  textRenderer.textInfo.characterInfo[i];
                    letterPositions.Add(characterInfo.bottomLeft);

                    GameObject textElement = Utils.DuplicateGameObject(textRenderer.gameObject);
                    
                    EditTMProComponent.SetTextContent(new[] { textElement }, characterInfo.character.ToString());
                    textElement.GetComponent<TextMeshPro>().ForceMeshUpdate();
                    EditTMProComponent.TrimTextBox(new[] { textElement });
                    EditTMProComponent.AlignText(new[] {textElement}, TextAlignmentOptions.Center);
                    
                    RectTransform textRect = textElement.GetComponent<RectTransform>();
                    
                    Vector3 topLeft = textRenderer.transform.TransformPoint(characterInfo.topLeft);
                    Vector3 bottomLeft = textRenderer.transform.TransformPoint(characterInfo.bottomLeft);
                    Vector3 bottomRight = textRenderer.transform.TransformPoint(characterInfo.bottomRight);

                    float xPos = (bottomLeft.x + bottomRight.x) / 2;
                    float yPos = characterInfo.baseLine;

                    textRect.localPosition = textRect.transform.parent.InverseTransformPoint(new Vector3(xPos, yPos, bottomLeft.z));
                    
                    createdObjects.Add(textElement);
                }
            }

            for (int i = 0; i < selection.Length; i++) {
                Undo.DestroyObjectImmediate(selection[i].gameObject);
            }
            
            return createdObjects.ToArray();
        }
        

        public static string ExtractTextByLines(TMP_Text[] selection, string openingWrapper, string closingWrapper)
        {
            string fullString = "";

            for(int z=0; z<selection.Length; z++) {

                TMP_Text textRenderer = selection[z];

                for (int i = 0; i < textRenderer.textInfo.lineCount; i++) {

                    TMP_LineInfo lineInfo = textRenderer.textInfo.lineInfo[i];
                    int lineStart = lineInfo.firstCharacterIndex;
                    int lineEnd = lineInfo.lastCharacterIndex + 1;

                    string lineString = openingWrapper + "\n";
                    lineString += ("\t" + textRenderer.text.Substring(lineStart, lineEnd - lineStart) + "\n");
                    lineString += closingWrapper + "\n";

                    fullString += lineString;                    
                }
            }

            return fullString;
        }
    }
}
