/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Localization Manager")]
    public class LocalizationManager : ScriptableObject
    {
        public Dictionary<string, Dictionary<string, string>> texts = new Dictionary<string, Dictionary<string, string>>();
        private string missingTextString = "Localized text not found";

        public void LoadLocalizedText(string fileName, string textName)
        {
            Dictionary<string, string> localizedText = new Dictionary<string, string>();

            string filePath = null;
            string streamingAssetsFilePath = Path.Combine(Application.streamingAssetsPath, fileName);

            // Allow for flexibility in naming absolute or relative file path
            if (File.Exists(fileName)) {
                filePath = fileName;
            }
            else if (File.Exists(streamingAssetsFilePath)) {
                filePath = streamingAssetsFilePath;
            }

            if (filePath != null) {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(File.ReadAllText(filePath));

                foreach (XmlNode textObject in xmlDocument["container"].ChildNodes) {
                    switch (textObject.Attributes["type"].Value) {

                        case "standard":
                        default:
                            StringReader strReader = new StringReader(textObject.InnerText);
                            string currentLine = null;
                            string parsedText = null;
                            while ((currentLine = strReader.ReadLine()) != null) {
                                parsedText += currentLine.TrimStart(' ', '\t');
                            }
                            localizedText.Add(textObject.Attributes["key"].Value, parsedText);
                            break;
                    }
                }
                texts.Add(textName, localizedText);
            }
            else {
                Debug.Log(SceneManager.GetActiveScene().name);
                Debug.Log("Localization file not found.");
            }
        }

        public void RefreshText(string textName, GameObject textObject)
        {
            Dictionary<string, string> currentText = texts[textName];
            if (currentText == null) {
                Debug.Log(textName + " text not found!");
            }

            string key = textObject.GetComponent<LocalizedText>().key;
            TextMeshPro textMeshPro = textObject.GetComponent<TextMeshPro>();

            if (currentText.ContainsKey(key)) {
                // Need this code or else will not be saved in Edit mode
                #if UNITY_EDITOR
                Undo.FlushUndoRecordObjects();
                Undo.RecordObject(textMeshPro, "Save text on textMeshPro");
                #endif
                // Must execute Replace() method or else new lines ore not parsed correctly
                string parsedText = currentText[key].Replace("\\n", "\n");
                textMeshPro.text = parsedText;
                textObject.name = "tx - " + parsedText;
            }
            else {
                textMeshPro.text = missingTextString;
            }
        }
    }

}