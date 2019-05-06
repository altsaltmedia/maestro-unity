using System;
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
    public class PopulateText : EditorWindow
    {
        
        string filePath = "";
        string fileName = "";

        LocalizationCorpus localizationCorpus;
        Language targetLanguage;
        GameObject rootObject;
        ModifySettings modifySettings;
        ComplexEvent languageUpdate;

        [MenuItem("Tools/AltSalt/Populate Text")]
        public static void ShowWindow()
        {
            var window = GetWindow<PopulateText>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(PopulateText)).Show();
        }

        public void OnGUI()
        {
            GUILayout.Space(20);

            if (filePath.Length > 0) {
                GUILayout.Label("Ready to populate " + GetFilename(filePath));
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

            localizationCorpus = EditorGUILayout.ObjectField("Localization Corpus", localizationCorpus, typeof(LocalizationCorpus), false) as LocalizationCorpus;
            EditorGUI.BeginDisabledGroup(DisablePopulateGameObjects());
            if (GUILayout.Button("Generate Keys for Game Objects")) {
                GenerateKeysForGameObjects();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            targetLanguage = EditorGUILayout.ObjectField("Target Language", targetLanguage, typeof(Language), false) as Language;
            EditorGUI.BeginDisabledGroup(DisablePopulateCorpus());
            if (GUILayout.Button("Populate Corpus")) {
                PopulateCorpus();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            GUILayout.Label("These values are automatically populated via script.");
            EditorGUI.BeginDisabledGroup(true);
            modifySettings = EditorGUILayout.ObjectField("App Settings", modifySettings, typeof(ModifySettings), false) as ModifySettings;
            modifySettings = Utils.GetModifySettings();
            languageUpdate = EditorGUILayout.ObjectField("Language Update", languageUpdate, typeof(ComplexEvent), false) as ComplexEvent;
            languageUpdate = Utils.GetComplexEvent("LanguageUpdate");
            EditorGUI.EndDisabledGroup();

            if(modifySettings == null) {
                GUILayout.Label("WARNING: No Modify Settings found! Please create an instance of Modify Settings.");
            }

            EditorGUI.BeginDisabledGroup(DisableLanguageUpdate());
            if (GUILayout.Button("Trigger Language Update")) {
                LanguageUpdate();
            }
            EditorGUI.EndDisabledGroup();
        }


        private void LoadFile()
        {
            filePath = EditorUtility.OpenFilePanel("Select text file", Application.streamingAssetsPath, "xml");
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

            List<LocalizedText> localizedTexts = new List<LocalizedText>();
            localizedTexts = GetLocalizedTexts(localizedTexts, rootObject.transform);

            if (EditorUtility.DisplayDialog("Replace keys for text objects?",
                "This will overwrite keys for localized texts under " + rootObject.name + " that use the " + localizationCorpus.name + " corpus.", "Proceed", "Cancel")) {
                List<LocalizedText> selectedTexts = new List<LocalizedText>();
                for (int i = 0; i < localizedTexts.Count; i++) {
                    if(localizedTexts[i].localizationCorpus == localizationCorpus) {
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
            if (rootObject == null || localizationCorpus == null) {
                return true;
            } else {
                return false;
            }
        }

        bool DisablePopulateCorpus()
        {
            if (filePath.Length == 0 || localizationCorpus == null || targetLanguage == null) {
                return true;
            } else {
                return false;
            }
        }

        bool DisableLanguageUpdate()
        {
            if (localizationCorpus == null || targetLanguage == null) {
                return true;
            } else {
                return false;
            }
        }

        void PopulateCorpus()
        {
            if (filePath != null) {
                if (EditorUtility.DisplayDialog("Replace " + targetLanguage.name + " text?", "This will replace '" + localizationCorpus.name + " - " +
                        targetLanguage.name + "' using \n" + filePath, "Proceed", "Cancel")) {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(File.ReadAllText(filePath));

                    foreach (XmlNode textObject in xmlDocument["container"].ChildNodes) {
                        StringReader strReader = new StringReader(textObject.InnerText);
                        string currentLine = null;
                        string parsedText = null;
                        while ((currentLine = strReader.ReadLine()) != null) {
                            parsedText += currentLine.TrimStart(' ', '\t');
                        }
                        localizationCorpus.languageSet[targetLanguage][textObject.Attributes["key"].Value] = parsedText;
                    }
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

        private List<LocalizedText> GetLocalizedTexts(List<LocalizedText> textList, Transform transform)
        {
            if (transform.childCount == 0) {
                return textList;
            } else {
                for (int i = 0; i < transform.childCount; i++) {
                    Transform textTransform = transform.GetChild(i);
                    GetLocalizedTexts(textList, textTransform);
                    LocalizedText localizedText = textTransform.gameObject.GetComponent<LocalizedText>();
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

        void LanguageUpdate()
        {
            if (EditorUtility.DisplayDialog("Set active language and update texts?", "This will set the active language to " + targetLanguage.name +
                " in AppSettings and update all texts that use the " + localizationCorpus.name + " corpus.", "Proceed", "Cancel")) {
                modifySettings.activeLanguage = targetLanguage;
                languageUpdate.Raise(localizationCorpus);
            }
        }

    }
}