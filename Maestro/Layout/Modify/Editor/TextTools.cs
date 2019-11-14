using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Xml;
using System.IO;
using System.Linq;

namespace AltSalt.Maestro
{
    public class TextTools : ModifyTools
    {
        Vector2 scrollPos;
        string filePath = "";
        string fileName = "";

        GameObject rootObject;

        protected TextCollectionBank textCollectionBank;
        protected TextFamily targetTextFamily;

        [MenuItem("Tools/AltSalt/Text Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<TextTools>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(TextTools)).Show();
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

            rootObject = EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true) as GameObject;

            textCollectionBank = EditorGUILayout.ObjectField("Text Collection Bank", textCollectionBank, typeof(TextCollectionBank), false) as TextCollectionBank;
            EditorGUI.BeginDisabledGroup(DisablePopulateGameObjects());
            if (GUILayout.Button("Generate Keys for Game Objects")) {
                GenerateKeysForGameObjects();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            targetTextFamily = EditorGUILayout.ObjectField("Text Family Destination", targetTextFamily, typeof(TextFamily), false) as TextFamily;
            EditorGUI.BeginDisabledGroup(DisablePopulateCorpus());
            if (GUILayout.Button("Populate Bank")) {
                PopulateCorpus();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            base.OnGUI();

            GUILayout.Space(20);

            if (targetTextFamily != null) {
                if (targetTextFamily.supportedLayouts.Count > 0) {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < targetTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings._activeLayoutConfig == targetTextFamily.supportedLayouts[i]) {
                            triggerLayoutChange = false;
                            loadedLayoutName = modifySettings._activeLayoutConfig.name;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        loadedLayoutName = targetTextFamily.supportedLayouts[0].name;
                    }
                } else {
                    loadedLayoutName = modifySettings._defaultLayoutConfig.name;
                }

                GUILayout.Label("'" + targetTextFamily.name + "' text family loaded layout: " + loadedLayoutName);
            }

            EditorGUI.BeginDisabledGroup(DisableTextUpdate());
            if (GUILayout.Button("Trigger Text Update")) {
                TriggerTextUpdate();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(DisableTextUpdateAll());
            if (GUILayout.Button("Trigger Text Update (All)")) {
                TriggerTextUpdateAll();
            }
            EditorGUI.EndDisabledGroup();

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
            List<string> keyList = PopulateKeyList();

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

        private void GenerateKeysForGameObjects()
        {
            List<string> keyList = PopulateKeyList();

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

        bool DisablePopulateGameObjects()
        {
            if (rootObject == null || textCollectionBank == null) {
                return true;
            } else {
                return false;
            }
        }

        bool DisablePopulateCorpus()
        {
            if (filePath.Length == 0 || textCollectionBank == null || targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        void PopulateCorpus()
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

        private List<string> PopulateKeyList()
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

        private List<TextLoader> GetLocalizedTexts(List<TextLoader> textList, Transform transform)
        {
            if (transform.childCount == 0) {
                return textList;
            } else {
                for (int i = 0; i < transform.childCount; i++) {
                    Transform textTransform = transform.GetChild(i);
                    GetLocalizedTexts(textList, textTransform);
                    TextLoader localizedText = textTransform.gameObject.GetComponent<TextLoader>();
                    if(textTransform.gameObject.activeInHierarchy == true && localizedText != null) {
                        textList.Add(localizedText);
                    }
                }
                return textList;
            }
        }

        private string GetFilename(string path)
        {
            int lastIndex = path.LastIndexOf("/", StringComparison.CurrentCulture);
            return path.Substring(lastIndex + 1);
        }

        protected bool DisableTextUpdateAll()
        {
            if (targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        protected bool DisableTextUpdate()
        {
            if (textCollectionBank == null || targetTextFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        protected void TriggerTextUpdate()
        {
            if (EditorUtility.DisplayDialog("Set active language and update texts?", "This will set the active language to " + targetTextFamily.name +
                " in ModifySettings and update texts that use the " + textCollectionBank.name + " text bank, and (if needed) trigger a layout change to " + loadedLayoutName + ".", "Proceed", "Cancel")) {
                modifySettings.activeTextFamily = targetTextFamily;
                textUpdate.RaiseEvent(this, "text tools", "editor window", textCollectionBank);
                if (targetTextFamily.supportedLayouts.Count == 0) {
                    modifySettings._activeLayoutConfig = modifySettings._defaultLayoutConfig;
                    layoutUpdate.RaiseEvent(this, "text tools", "editor window");
                } else {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < targetTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings._activeLayoutConfig == modifySettings.activeTextFamily.supportedLayouts[i]) {
                            triggerLayoutChange = false;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        modifySettings._activeLayoutConfig = targetTextFamily.supportedLayouts[0];
                        layoutUpdate.RaiseEvent(this, "text tools", "editor window");
                    }
                }
            }
        }

        protected void TriggerTextUpdateAll()
        {
            if (EditorUtility.DisplayDialog("Set active language and update texts?", "This will set the active language to " + targetTextFamily.name +
                " in ModifySettings and update ALL texts, and (if needed) trigger a layout change to " + loadedLayoutName + ".", "Proceed", "Cancel")) {
                modifySettings.activeTextFamily = targetTextFamily;
                textUpdate.RaiseEvent(this, "text tools", "editor window", textCollectionBank);
                if (targetTextFamily.supportedLayouts.Count == 0) {
                    modifySettings._activeLayoutConfig = modifySettings._defaultLayoutConfig;
                    layoutUpdate.RaiseEvent(this, "text tools", "editor window");
                } else {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings._activeLayoutConfig == targetTextFamily.supportedLayouts[i]) {
                            triggerLayoutChange = false;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        modifySettings._activeLayoutConfig = targetTextFamily.supportedLayouts[0];
                        layoutUpdate.RaiseEvent(this, "text tools", "editor window");
                    }
                }
            }
        }

    }
}