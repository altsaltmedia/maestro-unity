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

        string inputTextName = "[input text name]";
        string textTitlePrompt = "Choose a text name: ";
        XmlDocument xmlDocument = new XmlDocument();

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
            }
            else {
                GUILayout.Label("No file loaded.");
            }

            if (GUILayout.Button("Load file")) {
                LoadFile();
            }

            EditorGUI.BeginDisabledGroup(filePath.Length == 0);
            if (GUILayout.Button("Generate Keys for Input File")) {
                GenerateKeys();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            inputTextName = EditorGUILayout.TextField(textTitlePrompt, inputTextName);
            if (GUILayout.Button("Map GameObjects to Keys")) {
                MapGameObjectsToKeys();
            }

            if (GUILayout.Button("Populate GameObjects")) {
                PopulateGameObjects();
            }
        }

        private void LoadFile()
        {
            filePath = EditorUtility.OpenFilePanel("Select text file", Application.streamingAssetsPath, "xml");
            if (File.Exists(filePath)) {
                xmlDocument.LoadXml(File.ReadAllText(filePath));
                fileName = GetFilename(filePath);
            }
        }

        private void GenerateKeys()
        {
            List<string> keyList = PopulateKeyList();

            int count = 0;
            foreach (XmlNode slide in xmlDocument["container"]) {
                XmlAttribute attr = xmlDocument.CreateAttribute("key");
                attr.Value = keyList[count];
                slide.Attributes.Append(attr);
                count++;
            }

            var path = EditorUtility.SaveFilePanel("Save XML file", filePath, fileName, "xml");
            if (path.Length != 0) {
                xmlDocument.Save(path);
                filePath = "";
            }
        }

        private void MapGameObjectsToKeys()
        {
            List<string> keyList = PopulateKeyList();
            List<GameObject> textCanvases = GetTextCanvases();
            LocalizationManager localizationManager = GetLocalizationManager();

            for (int i = 0; i < textCanvases.Count; i++) {
                Debug.Log(textCanvases[i]);
                LocalizedText localizedText = textCanvases[i].GetComponent<LocalizedText>();
                Undo.FlushUndoRecordObjects();
                Undo.RecordObject(localizedText, "Save text on textMeshPro");
                if (localizedText.m_localizationManager == null) {
                    localizedText.m_localizationManager = localizationManager;
                }

                localizedText.key = keyList[i];
            }
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        private void PopulateGameObjects()
        {
            List<GameObject> textCanvases = GetTextCanvases();
            LocalizationManager localizationManager = ScriptableObject.CreateInstance("LocalizationManager") as LocalizationManager;

            string inputFilePath = EditorUtility.OpenFilePanel("Select text file", Application.streamingAssetsPath, "xml");
            localizationManager.LoadLocalizedText(inputFilePath, inputTextName);

            for (int i = 0; i < textCanvases.Count; i++) {
                localizationManager.RefreshText(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, textCanvases[i]);
            }
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
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

        private List<GameObject> GetTextCanvases()
        {
            List<GameObject> textCanvasList = new List<GameObject>(GameObject.FindGameObjectsWithTag("textObject"));

            // Unity doesn't easily provide an ordered hierarchical list of game objects, or in other words, whenever we do a search
            // for game objects by tag, they all come in out of order. So that we can actually populate these text objects with keys in
            // the correct order, let's devise a dictionary that utilizes the sibling index of a text's topmost parent to figure out where
            // it lies in the hierarchy. What we'll do is traverse to each text's topmost parent object, then capture the parent's sibling
            // index as a key, which we will use to organize all of the objects accordingly. In other words, we'll group together all the
            // text objects that share the same parent, setting each group as a collection based on their topmost parent's sibling index.
            Dictionary<int, Dictionary<int, GameObject>> textCanvasCollection = new Dictionary<int, Dictionary<int, GameObject>>();

            if (textCanvasList.Count == 0) {
                throw new Exception("No gameObjects with tag 'textObject' found! Please add tag 'textObject' to gameObjects you wish to populate.");
            }

            for (int i = 0; i < textCanvasList.Count; i++) {

                // For each text, we need to traverse up the hierarchy to find its topmost parent
                Transform parent = textCanvasList[i].transform.parent;
                int topParentSibIndex;

                while (true) {
                    // Traverse until we hit the content container
                    if (parent.transform.parent != null && parent.transform.parent.tag != "content") {
                        parent = parent.transform.parent;
                    }
                    else {
                        break;
                    }
                }

                // Once the top parent is found, we set the text object to a key or index based on
                // that parent's position in the hierarchy. To place the text object into this dictionary,
                // we will also use the sibling index on the text itself so that we can identify where it
                // exists relative to the text objects surrounding it
                topParentSibIndex = parent.transform.GetSiblingIndex();
                int textCanvasSibIndex = textCanvasList[i].transform.GetSiblingIndex();

                // Create a dictionary for this parent index if one doesn't already exist
                if (textCanvasCollection.ContainsKey(topParentSibIndex) == false) {
                    textCanvasCollection.Add(topParentSibIndex, new Dictionary<int, GameObject>());
                }

                textCanvasCollection[topParentSibIndex][textCanvasSibIndex] = textCanvasList[i];
            }

            // This is what we will use for our final list of sorted text objects
            List<GameObject> sortedTextList = new List<GameObject>();

            // Since the parent dictionary was created dynamically, we need to sort it so that we
            // loop through it starting from the lowest position in the hierarchy
            List<int> textCollectionIndexes = textCanvasCollection.Keys.ToList();
            textCollectionIndexes.Sort();

            foreach (int key in textCollectionIndexes) {

                // We also sort the actual list of texts so that we populate them with keys in the correct order
                List<int> textIndexes = textCanvasCollection[key].Keys.ToList();
                textIndexes.Sort();

                foreach (int textKey in textIndexes) {
                    sortedTextList.Add(textCanvasCollection[key][textKey]);
                }
            }

            return sortedTextList;
        }

        private string GetFilename(string path)
        {
            int lastIndex = path.LastIndexOf("/", StringComparison.CurrentCulture);
            return path.Substring(lastIndex + 1);
        }

        private LocalizationManager GetLocalizationManager()
        {
            string[] guids;
            List<LocalizationManager> managers = new List<LocalizationManager>();

            guids = AssetDatabase.FindAssets("t:LocalizationManager");

            foreach (string guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(".asset")) {
                    managers.Add((LocalizationManager)AssetDatabase.LoadAssetAtPath(path, typeof(LocalizationManager)));
                }
            }

            if (managers.Count < 1) {
                throw new Exception("No LocalizationManager asset found - please use Asset dropdown to create one.");
            }
            else {
                if (managers.Count > 1) {
                    Debug.LogError("More than one LocalizationManager found - check Assets folder for duplicates.");
                }
                return managers[0];
            }
        }

    }
}