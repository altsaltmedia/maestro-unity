using System;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Text.RegularExpressions;

namespace AltSalt
{
    public class PageBuilderWindow : EditorWindow
    {
        Vector2 scrollPos;
        public PageBuilderReferences pageBuilderReferences;
        string objectName = "[INSERT NAME HERE]";
        
        bool showSelectionTools = false;
        bool showSequencingTools = false;
        bool showCreateTools = false;

        bool showTextTools = false;
        string textContent;
        float fontSize;
        Color fontColor;

        bool showRectTransformTools = false;
        Vector3 rectTransPosition;
        Vector3 rectTransTransposePosition;

        bool selectCreatedObject;
        GameObject createdGameObject;

        [MenuItem("Tools/AltSalt/Page Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<PageBuilderWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(PageBuilderWindow)).Show();
        }

        //void OnEnable()
        //{
        //    Selection.selectionChanged += SetObjectNameDefault;
        //}

        //void OnDestroy()
        //{
        //    Selection.selectionChanged -= SetObjectNameDefault;
        //}

        //void SetObjectNameDefault()
        //{
        //    if(Selection.activeGameObject == null) {
        //        return;
        //    }

        //    string originalName = Selection.activeGameObject.name;
            
        //    Match match = Regex.Match(originalName, @"[0-9]+", RegexOptions.RightToLeft);

        //    string newName = "";

        //    if(match.Value != string.Empty) {
        //        int numValue = Int32.Parse(match.Value);
        //        numValue++;
        //        newName = originalName.Replace(match.Value, numValue.ToString());
        //    } else {
        //        newName = originalName + " (1)";
        //    }

        //    objectName = newName;
        //}

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            pageBuilderReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;

            GUILayout.Space(10);

            showSelectionTools = EditorGUILayout.Foldout(showSelectionTools, "Selection Tools");
            if(showSelectionTools == true) {
                SelectionTools.ShowSelectionTools();
            }

            GUILayout.Space(10);

            showSequencingTools = EditorGUILayout.Foldout(showSequencingTools, "Create Sequences");
            if (showSequencingTools == true) {
                CreateSequenceTools.ShowSequenceTools();
            }

            GUILayout.Space(10);

            showCreateTools = EditorGUILayout.Foldout(showCreateTools, "Create Sequences");
            if(showCreateTools == true) {
                ShowCreationTools();
            }

            GUILayout.Space(10);

            ShowEditingTools(Selection.gameObjects);

            EditorGUILayout.EndScrollView();
        }

        void ShowCreationTools()
        {
            objectName = EditorGUILayout.TextField("Object name", objectName);

            GUILayout.Space(5);

            selectCreatedObject = EditorGUILayout.Toggle("Select object after creation", selectCreatedObject);

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("New Text")) {
                createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.textPrefab, objectName);
            }
                
            if (GUILayout.Button("New Sprite")) {
                createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.spritePrefab, objectName);
            }

            if (GUILayout.Button("New Container")) {
                createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.containerPrefab, objectName);
            }

            if (GUILayout.Button("New Responsive Container")) {
                createdGameObject = CreateNewElement(Selection.transforms, pageBuilderReferences.responsiveContainerPrefab, objectName);
            }

            if (EditorGUI.EndChangeCheck() == true) {
                if (selectCreatedObject == true) {
                    Selection.activeGameObject = createdGameObject;
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Rename Element(s)")) {
                RenameElement(Selection.gameObjects);
            }

            
        }

        GameObject[] RenameElement(GameObject[] targetObjects)
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

        void ShowEditingTools(GameObject[] objectsToEdit)
        {
            bool textSelected = false;
            bool rectTransformSelected = false;

            for(int i=0; i<objectsToEdit.Length; i++) {

                if(objectsToEdit[i].GetComponent<TMP_Text>() != null) {
                    textSelected = true;
                }

                if (objectsToEdit[i].GetComponent<RectTransform>() != null) {
                    rectTransformSelected = true;
                }
            }

            if(textSelected == true) {
                showTextTools = EditorGUILayout.Foldout(showTextTools, "Text Tools");
                if(showTextTools == true) {
                    ShowTextTools();
                }
            }

            if(rectTransformSelected == true) {
                showRectTransformTools = EditorGUILayout.Foldout(showRectTransformTools, "Rect Transform Tools");
                if(showRectTransformTools == true) {
                    ShowRectTransformTools();
                }
            }
        }

        void ShowTextTools()
        {
            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Text Content");
            textContent = EditorGUILayout.TextArea(textContent, GUILayout.Height(50));

            if (EditorGUI.EndChangeCheck() == true) {
                for (int i = 0; i < Selection.gameObjects.Length; i++) {
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                    Undo.RecordObject(textMeshPro, "set text content");
                    if (textMeshPro != null) {
                        textMeshPro.text = textContent;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            fontSize = EditorGUILayout.FloatField("Font Size", fontSize);

            if(EditorGUI.EndChangeCheck() == true) {
                for(int i=0; i<Selection.gameObjects.Length; i++) {
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                    Undo.RecordObject(textMeshPro, "set font size");
                    if (textMeshPro != null) {
                        textMeshPro.fontSize = fontSize;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            fontColor = EditorGUILayout.ColorField("Font Color", fontColor);

            if (EditorGUI.EndChangeCheck() == true) {
                for (int i = 0; i < Selection.gameObjects.Length; i++) {
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                    Undo.RecordObject(textMeshPro, "set font color");
                    if (textMeshPro != null) {
                        textMeshPro.color = fontColor;
                    }
                }
            }

            GUILayout.Space(10);
        }

        void ShowRectTransformTools()
        {
            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            rectTransPosition = EditorGUILayout.Vector3Field("Rect Position", rectTransPosition);

            if (EditorGUI.EndChangeCheck() == true) {
                for (int i = 0; i < Selection.gameObjects.Length; i++) {
                    RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                    if (rectTransform != null) {
                        Undo.RecordObject(rectTransform, "set rect position");
                        rectTransform.anchoredPosition3D = rectTransPosition;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            rectTransTransposePosition = EditorGUILayout.Vector3Field("Transpose Rect Position", rectTransTransposePosition);

            if (EditorGUI.EndChangeCheck() == true) {
                for (int i = 0; i < Selection.gameObjects.Length; i++) {
                    RectTransform rectTransform = Selection.gameObjects[i].GetComponent<RectTransform>();
                    if (rectTransform != null) {
                        Undo.RecordObject(rectTransform, "tranpose rect position");
                        rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x + rectTransTransposePosition.x, rectTransform.anchoredPosition3D.y + rectTransTransposePosition.y, rectTransform.anchoredPosition3D.z + rectTransTransposePosition.z);
                    }
                }
                rectTransTransposePosition = new Vector3(0, 0, 0);
            }

            GUILayout.Space(10);
        }

        GameObject CreateNewElement(Transform[] selectedTransforms, GameObject sourceObject, string elementName = "")
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            string undoMessage = "create " + sourceObject.name;
            Undo.RegisterCreatedObjectUndo(newElement, undoMessage);

            if (elementName.Length > 0) {
                newElement.name = elementName;
            }

            if (selectedTransforms.Length > 1) {

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

        //public class TextPopup : EditorWindow
        //{
        //    public PageBuilderReferences pageBuilderReferences;
        //    public GameObject parentObject;

        //    string textContent;
        //    GameObject textObject;

        //    public static void ShowWindow()
        //    {
        //        var window = GetWindow<TextPopup>();
        //    }

        //    static void Init()
        //    {
        //        EditorWindow.GetWindow(typeof(TextPopup)).Close();
        //    }

        //    void OnGUI()
        //    {
        //        pageBuilderReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;

        //        textContent = EditorGUILayout.TextField("Text object content: ", textContent);

        //        if (GUILayout.Button("Place Text")) {
        //            PlaceText();
        //        }
        //    }

        //    void PlaceText()
        //    {
        //        textObject = PrefabUtility.InstantiatePrefab(pageBuilderReferences.textPrefab) as GameObject;

        //        textObject.GetComponent<TMP_Text>().text = textContent;

        //        if (parentObject != null) {
        //            textObject.transform.SetParent(parentObject.transform);
        //        }
        //    }
        //}

    }
}
