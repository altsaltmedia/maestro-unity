using System;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Text.RegularExpressions;

namespace AltSalt.PageBuilder
{
    public class PageBuilderWindow : EditorWindow
    {
        Vector2 scrollPos;
        public PageBuilderReferences pageBuilderReferences;
        string objectName;

        int toolbarInt = 0;
        string[] toolbarStrings = { "Container", "Text", "Image" };
        bool showSelectionTools = false;

        bool showTextTools = false;
        string textContent;
        float fontSize;
        Color fontColor;

        bool showRectTransformTools = false;
        Vector3 rectTransPosition;

        [MenuItem("Tools/AltSalt/Page Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<PageBuilderWindow>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(PageBuilderWindow)).Show();
        }

        void OnEnable()
        {
            Selection.selectionChanged += SetObjectNameDefault;
        }

        void OnDestroy()
        {
            Selection.selectionChanged -= SetObjectNameDefault;
        }

        void SetObjectNameDefault()
        {
            if(Selection.activeGameObject == null) {
                return;
            }

            string originalName = Selection.activeGameObject.name;
            
            Match match = Regex.Match(originalName, @"[0-9]+", RegexOptions.RightToLeft);

            string newName = "";

            if(match.Value != string.Empty) {
                int numValue = Int32.Parse(match.Value);
                numValue++;
                newName = originalName.Replace(match.Value, numValue.ToString());
            } else {
                newName = originalName + " (1)";
            }

            objectName = newName;
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            pageBuilderReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;

            showSelectionTools = EditorGUILayout.Foldout(showSelectionTools, "Selection Tools");
            if(showSelectionTools == true) {
                SelectionTools.ShowSelectionTools();
            }

            GUILayout.Space(10);

            objectName = EditorGUILayout.TextField("Object name", objectName);

            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            GUILayout.Space(10);

            ShowCreationTools();

            GUILayout.Space(10);

            ShowEditingTools(Selection.gameObjects);

            EditorGUILayout.EndScrollView();
        }

        void ShowCreationTools()
        {
            switch (toolbarInt) {

                case 0:
                    if (GUILayout.Button("Create Element(s)")) {
                        CreateNewElement(Selection.activeTransform, pageBuilderReferences.responsiveContainerPrefab, objectName);
                    }
                    if (GUILayout.Button("Create Element(s) and Select")) {
                        Selection.activeGameObject = CreateNewElement(Selection.activeTransform, pageBuilderReferences.responsiveContainerPrefab, objectName);
                    }
                    break;

                case 1:
                    if (GUILayout.Button("Create Element(s)")) {
                        CreateNewElement(Selection.activeTransform, pageBuilderReferences.textPrefab, objectName);
                    }
                    if (GUILayout.Button("Create Element(s) and Select")) {
                        Selection.activeGameObject = CreateNewElement(Selection.activeTransform, pageBuilderReferences.textPrefab, objectName);
                    }
                    break;

                case 2:
                    if (GUILayout.Button("Create Element(s)")) {
                        CreateNewElement(Selection.activeTransform, pageBuilderReferences.spritePrefab, objectName);
                    }
                    if (GUILayout.Button("Create Element(s) and Select")) {
                        Selection.activeGameObject = CreateNewElement(Selection.activeTransform, pageBuilderReferences.spritePrefab, objectName);
                    }
                    break;

            }

            if (GUILayout.Button("Rename Element(s)")) {
                RenameElement(Selection.gameObjects);
            }
        }

        GameObject[] RenameElement(GameObject[] targetObjects)
        {
            for(int i=0; i<targetObjects.Length; i++) {
                if(i == 0) {
                    targetObjects[i].name = objectName;
                } else {
                    targetObjects[i].name = string.Format("{0} ({1})", objectName, i);
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
                    Undo.RecordObject(Selection.gameObjects[i], "Save text content");
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                    if (textMeshPro != null) {
                        textMeshPro.text = textContent;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            fontSize = EditorGUILayout.FloatField("Font Size", fontSize);

            if(EditorGUI.EndChangeCheck() == true) {
                for(int i=0; i<Selection.gameObjects.Length; i++) {
                    Undo.RecordObject(Selection.gameObjects[i], "Save font size");
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
                    if (textMeshPro != null) {
                        textMeshPro.fontSize = fontSize;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            fontColor = EditorGUILayout.ColorField("Font Color", fontColor);

            if (EditorGUI.EndChangeCheck() == true) {
                for (int i = 0; i < Selection.gameObjects.Length; i++) {
                    Undo.RecordObject(Selection.gameObjects[i], "Save font color");
                    TMP_Text textMeshPro = Selection.gameObjects[i].GetComponent<TMP_Text>();
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
                        rectTransform.anchoredPosition3D = rectTransPosition;
                    }
                }
            }

            GUILayout.Space(10);
        }

        GameObject CreateNewElement(Transform parentTransform, GameObject sourceObject, string elementName)
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            Undo.RegisterCreatedObjectUndo(newElement, "Create " + sourceObject.name);

            if (parentTransform != null) {
                newElement.transform.SetParent(parentTransform);
            }

            if (elementName.Length > 0) {
                newElement.name = elementName;
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
