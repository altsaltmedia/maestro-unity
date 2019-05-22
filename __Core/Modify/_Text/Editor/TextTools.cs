﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Xml;
using System.IO;
using System.Linq;

namespace AltSalt
{
    public class TextTools : ModifyTools
    {
        
        string filePath = "";
        string fileName = "";

        GameObject rootObject;

        protected TextCollectionBank textCollectionBank;
        protected TextFamily textFamily;

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

            textFamily = EditorGUILayout.ObjectField("Text Family Destination", textFamily, typeof(TextFamily), false) as TextFamily;
            EditorGUI.BeginDisabledGroup(DisablePopulateCorpus());
            if (GUILayout.Button("Populate Bank")) {
                PopulateCorpus();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            base.OnGUI();

            GUILayout.Space(20);

            if (textFamily != null) {
                if (textFamily.supportedLayouts.Count > 0) {
                    loadedLayoutName = textFamily.supportedLayouts[0].name;
                } else {
                    loadedLayoutName = modifySettings.defaultLayout.name;
                }

                GUILayout.Label("'" + textFamily.name + "' text family loaded layout: " + loadedLayoutName);
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
        }


        private void LoadFile()
        {
            filePath = EditorUtility.OpenFilePanel("Select text file", filePath, "xml");
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
            foreach (XmlNode slide in xmlDocument["container"]) {
                XmlAttribute attr = xmlDocument.CreateAttribute("key");
                attr.Value = keyList[count];
                slide.Attributes.Append(attr);
                count++;
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
            if (filePath.Length == 0 || textCollectionBank == null || textFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        void PopulateCorpus()
        {
            if (filePath != null) {
                if (EditorUtility.DisplayDialog("Replace " + textFamily.name + " text?", "This will populate the '" + textCollectionBank.name + "' text family '" +
                        textFamily.name + "' using \n" + filePath, "Proceed", "Cancel")) {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(File.ReadAllText(filePath));

                    foreach (XmlNode textObject in xmlDocument["container"].ChildNodes) {
                        StringReader strReader = new StringReader(textObject.InnerText);
                        string currentLine = null;
                        string parsedText = null;
                        while ((currentLine = strReader.ReadLine()) != null) {
                            parsedText += currentLine.TrimStart(' ', '\t');
                        }

                        if (textCollectionBank.textCollection.ContainsKey(textFamily) == false) {
                            textCollectionBank.textCollection.Add(textFamily, new TextNodes());
                        }
                        textCollectionBank.textCollection[textFamily][textObject.Attributes["key"].Value] = parsedText;
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
            if (textFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        protected bool DisableTextUpdate()
        {
            if (textCollectionBank == null || textFamily == null) {
                return true;
            } else {
                return false;
            }
        }

        protected void TriggerTextUpdate()
        {
            if (EditorUtility.DisplayDialog("Set active language and update texts?", "This will set the active language to " + textFamily.name +
                " in ModifySettings and update texts that use the " + textCollectionBank.name + " text bank, and change the active layout to " + loadedLayoutName + ".", "Proceed", "Cancel")) {
                modifySettings.activeTextFamily = textFamily;
                textUpdate.Raise(textCollectionBank);
                if (modifySettings.activeTextFamily.supportedLayouts.Count == 0) {
                    modifySettings.activeLayout = modifySettings.defaultLayout;
                    layoutUpdate.Raise();
                } else {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings.activeLayout == modifySettings.activeTextFamily.supportedLayouts[i]) {
                            triggerLayoutChange = false;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        modifySettings.activeLayout = modifySettings.activeTextFamily.supportedLayouts[0];
                        layoutUpdate.Raise();
                    }
                }
            }
        }

        protected void TriggerTextUpdateAll()
        {
            if (EditorUtility.DisplayDialog("Set active language and update texts?", "This will set the active language to " + textFamily.name +
                " in ModifySettings and update ALL texts, and change the active layout to " + loadedLayoutName + ".", "Proceed", "Cancel")) {
                modifySettings.activeTextFamily = textFamily;
                textUpdate.Raise();
                if (modifySettings.activeTextFamily.supportedLayouts.Count == 0) {
                    modifySettings.activeLayout = modifySettings.defaultLayout;
                    layoutUpdate.Raise();
                } else {
                    bool triggerLayoutChange = true;
                    for (int i = 0; i < modifySettings.activeTextFamily.supportedLayouts.Count; i++) {
                        if (modifySettings.activeLayout == modifySettings.activeTextFamily.supportedLayouts[i]) {
                            triggerLayoutChange = false;
                        }
                    }
                    if (triggerLayoutChange == true) {
                        modifySettings.activeLayout = modifySettings.activeTextFamily.supportedLayouts[0];
                        layoutUpdate.Raise();
                    }
                }
            }
        }

    }
}