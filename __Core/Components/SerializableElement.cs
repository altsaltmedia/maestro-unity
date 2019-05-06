using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEditor;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class SerializableElement : MonoBehaviour
    {
#if UNITY_EDITOR
        string activeLayoutName;
#endif
        [Required]
        [Title("$activeLayoutName")]
        [SerializeField]
        ModifySettings modifySettings;

#if UNITY_EDITOR
        void OnGUI()
        {
            activeLayoutName = "Current active layout: " + modifySettings.activeLayout.name;
        }

        string GetDirectory()
        {
            string directoryPath = Application.dataPath + "/__Project";
            string[] subfolders = { "/Resources", "/Layouts", "/" + SceneManager.GetActiveScene().name, "/" + modifySettings.activeLayout.name };

            for (int i = 0; i < subfolders.Length; i++) {
                directoryPath += subfolders[i];
                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            return directoryPath;
        }

        string GetFilePath(string basePath)
        {
            StringBuilder path = new StringBuilder(basePath);
            string[] pathComponents = { "/", this.name, this.GetInstanceID().ToString(), ".json" };
            for (int i = 0; i < pathComponents.Length; i++) {
                path.Append(pathComponents[i]);
            }
            return path.ToString();
        }

        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveData()
        {
            string data = JsonUtility.ToJson(this, true);
            string filePath = GetFilePath(GetDirectory());
            File.WriteAllText(filePath, data);
        }
#endif

        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void LoadData()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Layouts/" + SceneManager.GetActiveScene().name + "/" + modifySettings.activeLayout.name + "/" + this.name + this.GetInstanceID().ToString());
            if (jsonTextFile != null) {
                JsonUtility.FromJsonOverwrite(jsonTextFile.ToString(), this);
            }
        }

#if UNITY_EDITOR
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshAssets()
        {
            AssetDatabase.Refresh();
        }

        void Reset()
        {
            modifySettings = Utils.GetModifySettings();
        }
#endif
    }
    
}