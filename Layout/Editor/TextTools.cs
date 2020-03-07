using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Xml;
using System.IO;
using System.Linq;
using Sirenix.Utilities;

namespace AltSalt.Maestro.Layout
{
    public class TextTools : ContentExtensionTools
    {
        private static TextTools _textTools;

        private static TextTools textTools
        {
            get => _textTools;
            set => _textTools = value;
        }

        Vector2 scrollPos;
        string filePath = "";
        string fileName = "";

        GameObject rootObject;

        protected TextCollectionBank textCollectionBank;
        protected TextFamily targetTextFamily;

        [MenuItem("Tools/Maestro/Text Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<TextTools>();
            window.Init();
            textTools = window;
        }

        private void Init()
        {
            titleContent = new GUIContent("Text Tools");
        }

        protected override void OnGUI()
        {
            base.CreateHeader();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Space(10);

            if (filePath.Length > 0) {
                GUILayout.Label("File '" + GetFilename(filePath) + "' loaded and ready for use.");
            } else {
                GUILayout.Label("No file loaded.");
            }

            if (GUILayout.Button("Load file")) {
                LoadFile();
            }

            EditorGUI.BeginDisabledGroup(filePath.Length == 0);
            if (GUILayout.Button("Generate Keys for Input File")) {
                GenerateKeysForInputFile();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            rootObject = EditorGUILayout.ObjectField("Text Object Parent", rootObject, typeof(GameObject), true) as GameObject;

            textCollectionBank = EditorGUILayout.ObjectField("Text Collection Bank", textCollectionBank, typeof(TextCollectionBank), false) as TextCollectionBank;
            EditorGUI.BeginDisabledGroup(DisablePopulateGameObjects());
            if (GUILayout.Button("Generate Keys for Text Objects")) {
                GenerateKeysForTextObjects();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            targetTextFamily = EditorGUILayout.ObjectField("Text Family Destination", targetTextFamily, typeof(TextFamily), false) as TextFamily;
            EditorGUI.BeginDisabledGroup(DisablePopulateCorpus());
            if (GUILayout.Button(GetPopulateButtonString())) {
                PopulateTextCollection();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            if (targetTextFamily != null) {
                if (targetTextFamily.layoutDependencies.Count > 0) {
                    string layoutNames = "";
                    for (int i = 0; i < targetTextFamily.layoutDependencies.Count; i++) {
                        layoutNames += targetTextFamily.layoutDependencies[i].referenceName;
                        if (i <= targetTextFamily.layoutDependencies.Count - 2) {
                            layoutNames += ", ";
                        }
                    }
                    layoutDependencyNames = layoutNames;
                } else {
                    layoutDependencyNames = "NONE";
                }

                GUILayout.Label("'" + targetTextFamily.name + "' text family loaded layouts: " + layoutDependencyNames);
            }

            EditorGUI.BeginDisabledGroup(DisableTextUpdate());
            if (GUILayout.Button("Activate " + GetTextCollectionString())) {
                ActivateTextCollection();
            }
            if (GUILayout.Button("Deactivate " + GetTextCollectionString())) {
                DeactivateTextCollection();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(DisableTextUpdateAll());
            if (GUILayout.Button("Activate " + GetTextFamilyString() + " - All Texts")) {
                ActivateTextFamilyAll();
            }
            if (GUILayout.Button("Deactivate " + GetTextFamilyString() + " - All Texts")) {
                DeactivateTextFamilyAll();
            }
            EditorGUI.EndDisabledGroup();
            
            base.OnGUI();

            EditorGUILayout.EndScrollView();
        }


        private void LoadFile()
        {
            filePath = Path.GetFullPath(EditorUtility.OpenFilePanel("Select text file", filePath, "xml"));
            if (File.Exists(filePath)) {
                fileName = GetFilename(filePath);
            }
        }

        void OnDestroy()
        {
            filePath = "";
            fileName = "";
        }

        private void GenerateKeysForInputFile()
        {
            List<string> keyList = GetKeyList();

            int count = 0;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(File.ReadAllText(filePath));
            foreach (XmlNode node in xmlDocument[nameof(XMLValues.container)]) {
                if(node.LocalName == nameof(XMLValues.textObject)) {
                    XmlAttribute attr = xmlDocument.CreateAttribute("key");
                    attr.Value = keyList[count];
                    node.Attributes.Append(attr);
                    count++;
                }
            }

            var path = EditorUtility.SaveFilePanel("Save XML file", filePath, fileName, "xml");
            if (path.Length != 0) {
                xmlDocument.Save(path);
            }
        }

        private void GenerateKeysForTextObjects()
        {
            List<string> keyList = GetKeyList();

            List<TextLoader> localizedTexts = new List<TextLoader>();
            localizedTexts = GetLocalizedTexts(localizedTexts, rootObject.transform);

            if (EditorUtility.DisplayDialog("Replace keys for text objects?",
                "This will overwrite keys for localized texts under " + rootObject.name + " that use the " + textCollectionBank.name + " corpus.", "Proceed", "Cancel")) {
                List<TextLoader> selectedTexts = new List<TextLoader>();
                for (int i = 0; i < localizedTexts.Count; i++) {
                    if(localizedTexts[i].textCollectionBank == textCollectionBank) {
                        selectedTexts.Add(localizedTexts[i]);
                    }
                }
                for (int i = 0; i < selectedTexts.Count; i++) {
                    Undo.RecordObject(selectedTexts[i], "Save key on localized text");
                    selectedTexts[i].key = keyList[i];
                }
                EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        private bool DisablePopulateGameObjects()
        {
            if (rootObject == null || textCollectionBank == null) {
                return true;
            } else {
                return false;
            }
        }

        private bool DisablePopulateCorpus()
        {
            if (filePath.Length == 0 || textCollectionBank == null || targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        private string GetPopulateButtonString()
        {
            if (textCollectionBank != null) {
                return $"Populate {textCollectionBank.name}";
            }

            return "Populate Text Collection";
        }

        private string GetTextCollectionString()
        {
            if (targetTextFamily != null && textCollectionBank != null) {
                return $"{targetTextFamily.name} - {textCollectionBank.name}";
            }

            return "Text Collection";
        }

        private string GetTextFamilyString()
        {
            if (targetTextFamily != null) {
                return targetTextFamily.name;
            }

            return "Target Text Family";
        }

        private void PopulateTextCollection()
        {
            if (filePath != null) {
                if (EditorUtility.DisplayDialog("Replace " + targetTextFamily.name + " text?", "This will populate the '" + textCollectionBank.name + "' text family '" +
                        targetTextFamily.name + "' using \n" + filePath, "Proceed", "Cancel")) {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(File.ReadAllText(filePath));

                    foreach (XmlNode textObject in xmlDocument[nameof(XMLValues.container)].ChildNodes) {
                        if(textObject.LocalName == nameof(XMLValues.textObject)) {
                            StringReader strReader = new StringReader(textObject.InnerText);
                            string currentLine = null;
                            string parsedText = null;
                            while ((currentLine = strReader.ReadLine()) != null) {
                                parsedText += currentLine.TrimStart(' ', '\t');
                            }

                            if (textCollectionBank.textCollection.ContainsKey(targetTextFamily) == false) {
                                textCollectionBank.textCollection.Add(targetTextFamily, new TextNodes());
                            }
                            textCollectionBank.textCollection[targetTextFamily][textObject.Attributes["key"].Value] = parsedText;
                            Debug.Log(parsedText);
                        }
                    }
                    EditorUtility.SetDirty(textCollectionBank);
                }
            }
        }

        private static List<string> GetKeyList()
        {
            TextAsset textAsset = Resources.Load("noun-list") as TextAsset;
            if (textAsset == null) {
                throw new Exception("List of nouns not found! Please create a text asset of unique keys called 'noun-list' in a Resources folder.");
            }
            StringReader strReader = new StringReader(textAsset.text);

            string currentLine = null;
            List<string> keys = new List<string>();

            while ((currentLine = strReader.ReadLine()) != null) {
                keys.Add(currentLine);
            }
            return keys;
        }

        private static List<TextLoader> GetLocalizedTexts(List<TextLoader> textList, Transform transform)
        {
            if (transform.childCount == 0) {
                return textList;
            } else {
                for (int i = 0; i < transform.childCount; i++) {
                    Transform textTransform = transform.GetChild(i);
                    GetLocalizedTexts(textList, textTransform);
                    TextLoader localizedText = textTransform.gameObject.GetComponent<TextLoader>();
                    if(textTransform.gameObject.activeInHierarchy == true && localizedText != null && localizedText.isActiveAndEnabled == true) {
                        textList.Add(localizedText);
                    }
                }
                return textList;
            }
        }

        private static string GetFilename(string path)
        {
            int lastIndex = path.LastIndexOf("/", StringComparison.CurrentCulture);
            return path.Substring(lastIndex + 1);
        }

        private bool DisableTextUpdateAll()
        {
            if (targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        private bool DisableTextUpdate()
        {
            if (textCollectionBank == null || targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        private void ActivateTextCollection()
        {
            if (EditorUtility.DisplayDialog("Activate text family and update texts?", 
                "This will activate the " + targetTextFamily.name +
                " text family and update texts that use the " + textCollectionBank.name +
                " text collection bank, and (if needed) trigger layout changes to " + layoutDependencyNames + ".", "Proceed", "Cancel")) {
                TriggerTextUpdate(true);
                TextRefresh(textCollectionBank);
            }
        }

        private void DeactivateTextCollection()
        {
            if (EditorUtility.DisplayDialog("Deactivate text family and update texts?",
                "This will deactivate the " + targetTextFamily.name +
                " text family and update texts that use the " + textCollectionBank.name +
                " text collection  bank, and (if needed) trigger layout changes to " + layoutDependencyNames + ".", "Proceed", "Cancel")) {
                TriggerTextUpdate(false);
                TextRefresh(textCollectionBank);
            }
        }

        private void ActivateTextFamilyAll()
        {
            if (EditorUtility.DisplayDialog("Activate text family and update texts?",
                "This will set the active language to " + targetTextFamily.name +
                " and update ALL texts, and (if needed) trigger a layout change to " +
                layoutDependencyNames + ".", "Proceed", "Cancel")) {
                TriggerTextUpdate(true);
                TextRefreshAll();
            }
        }

        private void DeactivateTextFamilyAll()
        {
            if (EditorUtility.DisplayDialog("Deactivate text family  and update texts?",
                "This will set the active language to " + targetTextFamily.name +
                " and update ALL texts, and (if needed) trigger a layout change to " +
                layoutDependencyNames + ".", "Proceed", "Cancel")) {
                TriggerTextUpdate(false);
                TextRefreshAll();
            }
        }

        private void TriggerTextUpdate(bool targetStatus)
        {
            bool triggerLayoutChange;
            
            if (targetStatus == true) {
                ContentExtensionController.ActivateOriginTextFamily(targetTextFamily, this, out triggerLayoutChange);
            }
            else {
                ContentExtensionController.DeactivateOriginTextFamily(targetTextFamily, this, out triggerLayoutChange);
            }

            if (triggerLayoutChange == true) {
                LayoutTools.LayoutUpdate();
            }
        }

        private static void TextRefresh(TextCollectionBank targetTextCollection)
        {
            List<TextLoader> textLoaders = GetTextLoaders();

            for (int i = 0; i < textLoaders.Count; i++) {
                if(textLoaders[i].isActiveAndEnabled == true &&
                   textLoaders[i].textCollectionBank != null &&
                   textLoaders[i].textCollectionBank == targetTextCollection)
                    textLoaders[i].PopulateWithText();
            }
            
            textChanged.RaiseEvent(textTools, "Editor tools", "Text tools");
        }

        public static void TextRefreshAll()
        {
            List<TextLoader> textLoaders = GetTextLoaders();

            for (int i = 0; i < textLoaders.Count; i++) {
                if (textLoaders[i].isActiveAndEnabled == true) {
                    textLoaders[i].PopulateWithText();
                }
            }
            
            textChanged.RaiseEvent(textTools, "Editor tools", "Text tools");
        }

        private static List<TextLoader> GetTextLoaders()
        {
            UnityEngine.Object[] textObjects = Utils.FilterSelection(Utils.GetAllGameObjects(), typeof(TextLoader));
            List<TextLoader> textLoaders = new List<TextLoader>();
            
            for (int i = 0; i < textObjects.Length; i++) {
                GameObject gameObject = textObjects[i] as GameObject;
                if (gameObject.activeInHierarchy == true) {
                    textLoaders.Add(gameObject.GetComponent<TextLoader>());
                }
            }

            return textLoaders;
        }

    }
}