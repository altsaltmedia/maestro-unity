using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.IO;
using SimpleJSON;
using UnityEngine.Playables;

namespace AltSalt
{
    public static class RegisterDependencies
    {
        [MenuItem("Edit/RegisterDependencies", false, 0)]
        static void TriggerRegisterDependencies()
        {
            string eventQuery = typeof(EventBase).Name;
            string variableQuery = typeof(EventBase).Name;

            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", eventQuery));

            string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/__Project" });

            //for (int i = 0; i < sceneGuids.Length; i++) {

                //EditorUtility.DisplayProgressBar("Scanning project", "Finding dependencies", 1.0f / sceneGuids.Length * i);
                //Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));

                //RegisterComponentDependencies(guids, scene.name);
                RegisterPlayableDependencies(guids, "Splash");

            //}
            AssetDatabase.Refresh();
            //EditorUtility.ClearProgressBar();
        }

        static void RegisterComponentDependencies(string[] guids, string sceneName)
        {
            var objs = Resources.FindObjectsOfTypeAll(typeof(SimpleEventListenerBehaviour)) as Component[];
            if (objs == null) return;

            foreach (string guid in guids) {

                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

                Debug.Log(string.Format("[{0}] IDependable asset found : {1}", scriptableObject.name, scriptableObject.name));
                Debug.Log(string.Format("[{0}] Dependencies: ", scriptableObject.name));

                foreach (Component obj in objs) {
                    // Ignore prefabs
                    if (EditorUtility.IsPersistent(obj.gameObject) == true) {
                        continue;
                    }
                    FieldInfo[] fields =
                        obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                                BindingFlags.Static);
                    foreach (FieldInfo fieldInfo in fields) {

                        //Debug.Log(fieldInfo);
                        //Debug.Log(fieldInfo.GetValue(obj));
                        //Debug.Log(obj.gameObject.scene.name);
                        //Debug.Log(obj.gameObject.name);

                        if (fieldInfo.GetValue(obj) is ScriptableObject && (ScriptableObject)fieldInfo.GetValue(obj) == scriptableObject) {
                            Debug.Log(string.Format("[{0}] [{1}] {2}", scriptableObject.name, sceneName, obj.gameObject.name), obj.gameObject);
                            RegisterEventDependent(scriptableObject, sceneName, obj.gameObject.name);
                            RegisterComponentDependency(sceneName, obj.gameObject.name, scriptableObject.name);
                        }
                    }
                }
            }
        }


        static void RegisterPlayableDependencies(string[] guids, string sceneName)
        {
            var objs = Resources.FindObjectsOfTypeAll(typeof(PlayableAsset)) as PlayableAsset[];
            if (objs == null) return;

            foreach (string guid in guids) {

                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

                //Debug.Log(string.Format("[{0}] IDependable asset found : {1}", scriptableObject.name, scriptableObject.name));
                //Debug.Log(string.Format("[{0}] Dependencies: ", scriptableObject.name));

                foreach (PlayableAsset obj in objs) {

                    FieldInfo[] fields =
                        obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                                BindingFlags.Static);
                    foreach (FieldInfo fieldInfo in fields) {

                        Debug.Log(fieldInfo.CustomAttributes);
                        Debug.Log(fieldInfo.FieldType);
                        //Debug.Log(fieldInfo.GetValue(obj));
                        //Debug.Log(obj.gameObject.scene.name);
                        //Debug.Log(obj.gameObject.name);

                        if (fieldInfo.GetValue(obj) is ScriptableObject && (ScriptableObject)fieldInfo.GetValue(obj) == scriptableObject) {
                            Debug.Log(string.Format("[{0}] [{1}] {2}", scriptableObject.name, sceneName, obj.name));
                            RegisterEventDependent(scriptableObject, sceneName, obj.name);
                            RegisterComponentDependency(sceneName, obj.name, scriptableObject.name);
                        }
                    }
                }
            }
        }

        //static void ParseFieldInfo(FieldInfo[] fields, PlayableAsset obj, ScriptableObject scriptableObject, string sceneName)
        //{
        //    foreach (FieldInfo fieldInfo in fields) {
               
        //        if (fieldInfo.GetValue(obj) is ScriptableObject && (ScriptableObject)fieldInfo.GetValue(obj) == scriptableObject) {
        //            Debug.Log(string.Format("[{0}] [{1}] {2}", scriptableObject.name, sceneName, obj.name));
        //            RegisterEventDependent(scriptableObject, sceneName, obj.name);
        //            RegisterComponentDependency(sceneName, obj.name, scriptableObject.name);
        //        } else {
        //            ParseFieldInfo()
        //        }
        //    }
        //}

        public static void RegisterAssetDependent(ScriptableObject asset, string dependentScene, string dependentName)
        {
            if(asset is EventBase) {
                RegisterEventDependent(asset, dependentScene, dependentName);
            }
            //if (File.Exists(GetFilePath())) {
            //    WriteDependentToExistingFile(dependentScene, dependentName);
            //} else {
            //    WriteDependentToNewFile(dependentScene, dependentName);
            //}
            AssetDatabase.Refresh();
        }


        public static void RegisterComponentDependency(string componentScene, string componentName, string dependencyName)
        {
            string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + componentScene });
            string filePath = Utils.GetFilePath(componentRegistryPath, componentName, ".json");

            RegisterDependency(filePath, "Dependencies", dependencyName);
        }

        public static void RegisterEventDependent(ScriptableObject eventAsset, string dependentScene, string dependentName)
        {
            string eventRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Events" });
            string filePath = Utils.GetFilePath(eventRegistryPath, eventAsset.name, ".json");

            RegisterDependency(filePath, dependentScene, dependentName);
        }

        public static void RegisterVariableDependent(ScriptableObject variableAsset, string dependentScene, string dependentName)
        {
            string eventRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Variables" });
            string filePath = Utils.GetFilePath(eventRegistryPath, variableAsset.name, ".json");

            RegisterDependency(filePath, dependentScene, dependentName);
        }

        static void RegisterDependency(string filePath, string dependencyKey, string dependencyValue)
        {
            if(File.Exists(filePath)) {

                string savedData = File.ReadAllText(filePath);
                JSONNode dataAsJSON = Utils.AddToJSONArray(JSON.Parse(savedData), dependencyKey, dependencyValue);
                File.WriteAllText(filePath, dataAsJSON.ToString(2));

            } else {

                var newJSON = JSON.Parse("{}");
                newJSON[dependencyKey].Add(dependencyValue);
                File.WriteAllText(filePath, newJSON.ToString(2));

            }
        }
    }
}