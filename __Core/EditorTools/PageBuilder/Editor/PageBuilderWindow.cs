using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt
{
    public class PageBuilderWindow : EditorWindow
    {
        public PageBuilderReferences pageBuilderReferences;
        public string objectName = "[INSERT NAME HERE]";

        Dictionary<EnableCondition, List<VisualElement>> toggleData = new Dictionary<EnableCondition, List<VisualElement>>();

        // Sequences
        public SequenceController sequenceControllerObject;
        public SequenceList targetSequenceList;
        public string newSequenceName = "";

        // Text
        public string textContent;
        public float fontSize;
        public Color fontColor;

        // Rect Transform
        public Vector3 rectPosition;
        public Vector3 rectTransposePosition;

        VisualElement pageBuilderStructure;

        SerializedObject editorWindowSerialized;

        public bool selectCreatedObject;
        GameObject createdGameObject;

        enum ButtonNames
        {
            NewText,
            NewSprite,
            NewContainer,
            NewResponsiveContainer,
            NewSequenceTouchApplier,
            NewSequenceAutoplayer,
            NewSequenceList,
            NewSwipeDirector,
            RenameElements,
            RefreshLayout,
        }

        enum PropertyFieldNames
        {
            RectPosition,
            RectTransposePosition,
            TextContent,
            FontSize,
            FontColor
        }

        enum EnableCondition
        {
            SequenceControllerObjectPopulated,
            SequenceListNamePopulated,
            TextSelected,
            RectTransformSelected,
        }

        enum ToggleGroups
        {
            RectTransformComponent,
            TextMeshProComponent
        }

        enum UpdateWindowTriggers
        {
            SequenceControllerObject,
            TargetSequenceList,
            NewSequenceName
        }

        [MenuItem("Tools/AltSalt/Page Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<PageBuilderWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(PageBuilderWindow)).Show();
        }

        void RenderLayout()
        {
            rootVisualElement.Clear();
            AssetDatabase.Refresh();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_AltSalt/__Core/EditorTools/TimelineUtilities/Editor/TimelineUtilities_Style.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var pageBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_AltSalt/__Core/EditorTools/PageBuilder/Editor/PageBuilderWindow.uxml");
            pageBuilderStructure = pageBuilderTree.CloneTree();
            rootVisualElement.Add(pageBuilderStructure);

            editorWindowSerialized = new SerializedObject(this);
            rootVisualElement.Bind(editorWindowSerialized);

            var buttons = rootVisualElement.Query<Button>();
            buttons.ForEach(SetupButton);

            var propertyFields = rootVisualElement.Query<PropertyField>();
            propertyFields.ForEach(SetupPropertyField);

            var toggleGroups = rootVisualElement.Query<VisualElement>(null, "toggleable-group");
            toggleGroups.ForEach(SetupToggleGroups);

            var updateWindowTriggers = rootVisualElement.Query<VisualElement>(null, "update-window-trigger");
            updateWindowTriggers.ForEach(SetupUpdateWindowTriggers);
        }

        void OnEnable()
        {
            pageBuilderReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;
            RenderLayout();
            Selection.selectionChanged += UpdateElementStructure;
        }

        void OnDestroy()
        {
            Selection.selectionChanged -= UpdateElementStructure;
        }

        void UpdateElementStructure()
        {
            if (sequenceControllerObject != null) {
                ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.SequenceControllerObjectPopulated, false);
            }

            if (targetSequenceList != null && newSequenceName.Length > 0) {
                ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.SequenceListNamePopulated, false);
            }

            bool textSelected = false;
            bool rectTransformSelected = false;

            for (int i = 0; i < Selection.objects.Length; i++) {
                if (Selection.objects[i] is GameObject && ((GameObject)Selection.objects[i]).GetComponent<TMP_Text>() != null) {
                    textSelected = true;
                }

                if (Selection.objects[i] is GameObject && ((GameObject)Selection.objects[i]).GetComponent<RectTransform>() != null) {
                    rectTransformSelected = true;
                }
            }

            if (textSelected == true) {
                ToggleVisualElements(toggleData, EnableCondition.TextSelected, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.TextSelected, false);
            }

            if (rectTransformSelected == true) {
                ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, true);
            } else {
                ToggleVisualElements(toggleData, EnableCondition.RectTransformSelected, false);
            }
        }

        Button SetupButton(Button button)
        {
            switch(button.name) {

                case nameof(ButtonNames.NewText):
                    button.clickable.clicked += () => {
                        createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.textPrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewSprite):
                    button.clickable.clicked += () => {
                        createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.spritePrefab, objectName);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.containerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewResponsiveContainer):
                    button.clickable.clicked += () => {
                        createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.responsiveContainerPrefab, objectName, true);
                        if (selectCreatedObject == true) {
                            Selection.activeGameObject = createdGameObject;
                        }
                    };
                    break;

                case nameof(ButtonNames.NewSequenceTouchApplier):
                    button.clickable.clicked += () => {
                        CreateSequenceTools.CreateSequenceTouchApplier(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSequenceAutoplayer):
                    button.clickable.clicked += () => {
                        CreateSequenceTools.CreateSequenceAutoplayer(Selection.activeTransform);
                    };
                    break;

                case nameof(ButtonNames.NewSequenceList):
                    button.clickable.clicked += () => {
                        CreateSequenceTools.CreateSequenceList(sequenceControllerObject);
                    };
                    AddToToggleData(toggleData, EnableCondition.SequenceControllerObjectPopulated, button);
                    break;

                case nameof(ButtonNames.NewSwipeDirector):
                    button.clickable.clicked += () => {
                        CreateSequenceTools.CreateSwipeDirector(Selection.activeTransform, targetSequenceList, newSequenceName);
                    };
                    AddToToggleData(toggleData, EnableCondition.SequenceListNamePopulated, button);
                    break;

                case nameof(ButtonNames.RenameElements):
                    button.clickable.clicked += () => {
                        RenameElements(Selection.gameObjects);
                    };
                    break;

                case nameof(ButtonNames.RefreshLayout):
                    button.clickable.clicked += () => {
                        RenderLayout();
                    };
                    break;
            }

            return button;
        }

        PropertyField SetupPropertyField(PropertyField propertyField)
        {
            switch (propertyField.name) {

                case nameof(PropertyFieldNames.RectPosition):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                            if (rectTransform != null) {
                                Undo.RecordObject(rectTransform, "set rect position");
                                rectTransform.anchoredPosition3D = rectPosition;
                            }
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.RectTransposePosition):
                    propertyField.RegisterCallback<ChangeEvent<Vector3>>((ChangeEvent<Vector3> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                            if (rectTransform != null) {
                                Undo.RecordObject(rectTransform, "tranpose rect position");
                                rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x + rectTransposePosition.x, rectTransform.anchoredPosition3D.y + rectTransposePosition.y, rectTransform.anchoredPosition3D.z + rectTransposePosition.z);
                            }
                        }
                        rectTransposePosition = new Vector3(0, 0, 0);
                    });
                    break;

                case nameof(PropertyFieldNames.TextContent):
                    propertyField.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                            Undo.RecordObject(textMeshPro, "set text content");
                            if (textMeshPro != null) {
                                textMeshPro.text = textContent;
                            }
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.FontSize):
                    propertyField.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                            Undo.RecordObject(textMeshPro, "set font size");
                            if (textMeshPro != null) {
                                textMeshPro.fontSize = fontSize;
                            }
                        }
                    });
                    break;

                case nameof(PropertyFieldNames.FontColor):
                    propertyField.RegisterCallback<ChangeEvent<Color>>((ChangeEvent<Color> evt) => {
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                            Undo.RecordObject(textMeshPro, "set font color");
                            if (textMeshPro != null) {
                                textMeshPro.color = fontColor;
                            }
                        }
                    });
                    break;

            }

            return propertyField;
        }

        VisualElement SetupToggleGroups(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(ToggleGroups.RectTransformComponent):
                    AddToToggleData(toggleData, EnableCondition.RectTransformSelected, visualElement);
                    break;

                case nameof(ToggleGroups.TextMeshProComponent):
                    AddToToggleData(toggleData, EnableCondition.TextSelected, visualElement);
                    break;
            }

            return visualElement;
        }


        VisualElement SetupUpdateWindowTriggers(VisualElement visualElement)
        {
            switch (visualElement.name) {

                case nameof(UpdateWindowTriggers.SequenceControllerObject):
                case nameof(UpdateWindowTriggers.TargetSequenceList):
                    visualElement.RegisterCallback<ChangeEvent<UnityEngine.Object>>((ChangeEvent<UnityEngine.Object> evt) => {
                        UpdateElementStructure();
                    });
                    break;

                case nameof(UpdateWindowTriggers.NewSequenceName):
                    visualElement.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) => {
                        UpdateElementStructure();
                    });
                    break;
            }

            return visualElement;
        }


        Dictionary<EnableCondition, List<VisualElement>> AddToToggleData(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, VisualElement elementToAdd)
        {
            if (targetCollection.ContainsKey(targetCondition)) {
                targetCollection[targetCondition].Add(elementToAdd);
            } else {
                List<VisualElement> visualElementList = new List<VisualElement>();
                visualElementList.Add(elementToAdd);
                targetCollection.Add(targetCondition, visualElementList);
            }

            return targetCollection;
        }

        Dictionary<EnableCondition, List<VisualElement>> ToggleVisualElements(Dictionary<EnableCondition, List<VisualElement>> targetCollection, EnableCondition targetCondition, bool targetStatus = false)
        {
            if (targetCollection.ContainsKey(targetCondition)) {
                List<VisualElement> visualElementList = targetCollection[targetCondition];
                for (int i = 0; i < visualElementList.Count; i++) {
                    if(visualElementList[i] is Foldout) {
                        (visualElementList[i] as Foldout).value = targetStatus;
                    }
                    visualElementList[i].SetEnabled(targetStatus);
                }
            }
            return targetCollection;
        }

        GameObject[] RenameElements(GameObject[] targetObjects)
        {
            Array.Sort(targetObjects, new Utils.GameObjectSort());

            for(int i=0; i<targetObjects.Length; i++) {
                if (objectName.Contains("{x}")) {
                    targetObjects[i].name = objectName.Replace("{x}", (i + 1).ToString());
                } else {
                    if (i == 0) {
                        targetObjects[i].name = objectName;
                    } else {    
                        targetObjects[i].name = string.Format("{0} ({1})", objectName, i);
                    }
                }

            }

            return targetObjects;
        }

        GameObject CreateNewElement(Transform[] selectedTransforms, GameObject sourceObject, string elementName = "", bool isParent = false)
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            string undoMessage = "create " + sourceObject.name;
            Undo.RegisterCreatedObjectUndo(newElement, undoMessage);

            if (elementName.Length > 0) {
                newElement.name = elementName;
            }

            if (selectedTransforms.Length > 1 || (selectedTransforms.Length > 0 && isParent == true)) {

                Array.Sort(selectedTransforms, new Utils.TransformSort());
                Transform parentTransform = selectedTransforms[0].parent;
                int sibIndex = selectedTransforms[0].GetSiblingIndex();

                for (int i = 0; i < selectedTransforms.Length; i++) {
                    Undo.SetTransformParent(selectedTransforms[i], newElement.transform, "set parent on game objects");
                }

                Undo.SetTransformParent(newElement.transform, parentTransform, "set parent on new element");
                newElement.transform.SetSiblingIndex(sibIndex);

            } else if (selectedTransforms.Length == 1) {
                newElement.transform.SetParent(selectedTransforms[0]);
            }

            return newElement;
        }

    }
}
