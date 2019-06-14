﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.IO;
using SimpleJSON;
using UnityEngine.Playables;
using System;
using UnityEngine.Events;

namespace AltSalt
{
    public class RegisterDependencies : EditorWindow
    {
        List<ScriptableObject> scriptableObjectList = new List<ScriptableObject>();
        int counter = 0;
        string callbackScenePath = "";
        string callbackSceneName = "";

        [MenuItem("Tools/AltSalt/Register Dependencies")]
        public static void ShowWindow()
        {
            var window = GetWindow<RegisterDependencies>();
        }

        static void Init()
        {
            EditorWindow.GetWindow(typeof(RegisterDependencies)).Show();
        }

        public void OnGUI()
        {
            if (callbackScenePath.Length > 0) {
                GUILayout.Label("Callback scene '" + callbackSceneName + "' loaded.");
            } else {
                GUILayout.Label("No scene loaded.");
            }

            if (GUILayout.Button("Load callback scene")) {
                LoadCallbackScene();
            }

            EditorGUI.BeginDisabledGroup(callbackScenePath.Length == 0);
            if (GUILayout.Button("Register Dependencies")) {
                TriggerRegisterDependencies();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(callbackScenePath.Length == 0);
            if (GUILayout.Button("Clear Hidden Values in Project")) {
                ClearHiddenValuesInProject();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void LoadCallbackScene()
        {
            callbackScenePath = EditorUtility.OpenFilePanel("Select text file", callbackScenePath, "unity");
            if (File.Exists(callbackScenePath)) {
                callbackSceneName = Path.GetFileNameWithoutExtension(callbackScenePath);
            }
        }

        string[] GetSceneGuids()
        {
            string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/__Project" });
            return sceneGuids;
        }

        void ClearHiddenValuesInProject()
        {
            string[] sceneGuids = GetSceneGuids();
            for (int i = 0; i < sceneGuids.Length; i++) {

                EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));

                var components = Resources.FindObjectsOfTypeAll(typeof(IClearHiddenValues)) as IClearHiddenValues[];
                foreach (IClearHiddenValues component in components) {
                    component.ClearHiddenValues();
                }
            }

            EditorSceneManager.OpenScene(callbackScenePath);
        }

        void TriggerRegisterDependencies()
        {
            string scriptableObjectQuery = typeof(RegisterableScriptableObject).Name;

            string[] scriptableObjectGuids = AssetDatabase.FindAssets(string.Format("t:{0}", scriptableObjectQuery));

            foreach (string scriptableObjectGuid in scriptableObjectGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(scriptableObjectGuid);
                ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));
                scriptableObjectList.Add(scriptableObject);
            }

            //string[] sceneGuids = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/__Project" });
            //for (int i = 0; i < sceneGuids.Length; i++) {

                //EditorUtility.DisplayProgressBar("Scanning project", "Finding dependencies", 1.0f / sceneGuids.Length * i);
                //Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));

                //RegisterComponentDependencies(scriptableObjectGuids, scene.name);
                RegisterComponentDependencies(scriptableObjectGuids, "Splash");
                //   RegisterPlayableDependencies(guids, "Splash");

            //}
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            counter = 0;
        }

        void RegisterComponentDependencies(string[] guids, string sceneName)
        {
            var objs = Resources.FindObjectsOfTypeAll(typeof(ConditionResponseTriggerBehaviour)) as Component[];
            if (objs == null) return;
            //Debug.Log(string.Format("[{0}] IDependable asset found : {1}", scriptableObject.name, scriptableObject.name));
            //Debug.Log(string.Format("[{0}] Dependencies: ", scriptableObject.name));

            foreach (Component obj in objs) {
                // Ignore prefabs
                if (obj.GetType().Namespace != "AltSalt" || EditorUtility.IsPersistent(obj.gameObject) == true || obj == null) {
                    continue;
                }
                FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                ParseFieldInfo(fields, obj, obj, sceneName);
            }
            
        }

        void ParseFieldInfo(FieldInfo[] fields, object obj, Component component, string sceneName)
        {
            foreach (FieldInfo fieldInfo in fields) {
                counter++;

                //if(counter > 100) {
                //    return;
                //}

                //Debug.Log(fieldInfo.GetValue(obj));

                if (fieldInfo.GetValue(obj) == null) {
                    continue;
                }

                Type type = fieldInfo.GetValue(obj).GetType();

                //if (recurse == true) {
                //    Debug.Log(fieldInfo.Name);
                //    Debug.Log(fieldInfo.GetValue(obj));
                //    Debug.Log(type.IsArray);
                //    if(type.IsGenericType) {
                //        Debug.Log(type);
                //    }
                //}

                if (fieldInfo.GetValue(obj) is ScriptableObject) {

                    foreach(ScriptableObject scriptableObject in scriptableObjectList) {

                        if((ScriptableObject)fieldInfo.GetValue(obj) == scriptableObject && component.gameObject.name == "LockVertSwipeButton") {
                            Debug.Log(component.gameObject.name, component.gameObject);
                            Debug.Log(obj, component.gameObject);
                            Debug.Log(fieldInfo.GetValue(obj), component.gameObject);
                            Debug.Log(scriptableObject.name, component.gameObject);
                            RegisterAssetDependent(scriptableObject, component, fieldInfo.DeclaringType.Name, sceneName);
                            RegisterComponentDependency(scriptableObject, component, fieldInfo.DeclaringType.Name, sceneName);
                            break;
                        }
                    }

                } else if (type.IsSubclassOf(typeof(EventBase)) || type.IsSubclassOf(typeof(TriggerBase)) || type.IsSubclassOf(typeof(VariableReferenceBase)) || type.IsSubclassOf(typeof(ConditionResponse)) || type is ConditionResponse[]) {

                    FieldInfo[] childFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    //Debug.Log(obj, component.gameObject);
                    //Debug.Log(fieldInfo.Name, component.gameObject);
                    //Debug.Log(fieldInfo.GetValue(obj), component.gameObject);


                    //Debug.Log(childFields.Length);
                    if (childFields.Length > 0) {
                        ParseFieldInfo(childFields, fieldInfo.GetValue(obj), component, sceneName);
                    }

                } else if (type.IsSubclassOf(typeof(UnityEventBase))) {

                    UnityEventBase unityEvent = fieldInfo.GetValue(obj) as UnityEventBase;
                    RegisterUnityEventValues(unityEvent, component, sceneName);

                } else if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType) {
                    IEnumerable enumerable = fieldInfo.GetValue(obj) as IEnumerable;
                    if (enumerable != null) {
                        //Debug.Log(obj);
                        foreach (object element in enumerable) {
                            //if(counter > 17) {
                            //    Debug.Log(type.DeclaringType);
                               //Debug.Log(element);
                            //}
                            if(element is ScriptableObject) {
                                foreach (ScriptableObject scriptableObject in scriptableObjectList) {

                                    if ((ScriptableObject)element == scriptableObject) {
                                        RegisterAssetInList(scriptableObject, component, fieldInfo.Name, sceneName);
                                        break;
                                    }
                                }
                            } else {
                                FieldInfo[] enumerableFields = element.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                ParseFieldInfo(enumerableFields, element, component, sceneName);
                            }
                        }
                    }
                }
            }
        }

        void RegisterAssetDependent(ScriptableObject scriptableObject, Component component, string valueLabel, string sceneName)
        {
            string[] directoryPath = new string[2];
            directoryPath[0] = "/z_Dependencies";

            if (scriptableObject is EventBase) {
                directoryPath[1] = "/Events";
            } else if (scriptableObject is VariableBase) {
                directoryPath[1] = "/Variables";
            } else {
                Debug.LogWarning("Scriptable object type not recognized : " + scriptableObject.name);
                return;
            }

            string registryPath = Utils.GetDirectory(directoryPath);
            string filePath = Utils.GetFilePath(registryPath, scriptableObject.name, ".json");

            JSONNode mainData;

            if (File.Exists(filePath)) {
                string savedData = File.ReadAllText(filePath);
                mainData = JSON.Parse(savedData);

            } else {
                mainData = JSON.Parse("{}");
            }

            mainData[sceneName][component.gameObject.name][component.GetType().Name].Add(valueLabel, scriptableObject.name);
            File.WriteAllText(filePath, mainData.ToString(2));
        }

        void RegisterComponentDependency(ScriptableObject scriptableObject, Component component, string valueLabel, string sceneName)
        {
            string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + sceneName });
            string filePath = Utils.GetFilePath(componentRegistryPath, component.name, ".json");

            JSONNode jsonData;

            if (File.Exists(filePath)) {

                string savedData = File.ReadAllText(filePath);
                jsonData = JSON.Parse(savedData);

            } else {
                jsonData = JSON.Parse("{}");
            }

            jsonData[component.GetType().Name].Add(valueLabel, scriptableObject.name);
            File.WriteAllText(filePath, jsonData.ToString(2));
        }

        void RegisterAssetInList(ScriptableObject scriptableObject, Component component, string valueLabel, string sceneName)
        {
            string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + sceneName });
            string filePath = Utils.GetFilePath(componentRegistryPath, component.name, ".json");

            JSONNode jsonData;

            if (File.Exists(filePath)) {

                string savedData = File.ReadAllText(filePath);
                jsonData = JSON.Parse(savedData);

            } else {
                jsonData = JSON.Parse("{}");
            }

            JSONNode jsonObject = new JSONArray();
            jsonObject.Add(scriptableObject.name);

            jsonData[component.GetType().Name][valueLabel].Add(scriptableObject.name);
            File.WriteAllText(filePath, jsonData.ToString(2));
        }


        void RegisterUnityEventValues(UnityEventBase unityEvent, Component component, string sceneName)
        {

            if (unityEvent != null) {

                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {

                    UnityEngine.Object target = unityEvent.GetPersistentTarget(i);
                    JSONNode jsonObject = new JSONObject();

                    if (target is ScriptableObject) {
                        foreach (ScriptableObject scriptableObject in scriptableObjectList) {
                            if ((ScriptableObject)target == scriptableObject) {
                                RegisterAssetDependent(scriptableObject, component, "UnityEventTarget", sceneName);
                                break;
                            }
                        }
                    }

                    jsonObject.Add("target", target.name);
                    jsonObject.Add("method", unityEvent.GetPersistentMethodName(i));

                    string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + sceneName });
                    string filePath = Utils.GetFilePath(componentRegistryPath, component.name, ".json");

                    JSONNode jsonData;

                    if (File.Exists(filePath)) {

                        string savedData = File.ReadAllText(filePath);
                        jsonData = JSON.Parse(savedData);

                    } else {
                        jsonData = JSON.Parse("{}");
                    }

                    jsonData[component.GetType().Name].Add("UnityEvent", jsonObject);
                    File.WriteAllText(filePath, jsonData.ToString(2));
                }
            }
        }

        void RegisterPlayableDependencies(string[] guids, string sceneName)
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

                        //Debug.Log(fieldInfo.CustomAttributes);
                        //Debug.Log(fieldInfo.FieldType);
                        //Debug.Log(fieldInfo.GetValue(obj));
                        //Debug.Log(obj.gameObject.scene.name);
                        //Debug.Log(obj.gameObject.name);

                        if (fieldInfo.GetValue(obj) is ScriptableObject && (ScriptableObject)fieldInfo.GetValue(obj) == scriptableObject) {
                            Debug.Log(string.Format("[{0}] [{1}] {2}", scriptableObject.name, sceneName, obj.name));
                            RegisterEventDependent(scriptableObject, sceneName, obj.name);
                            RegisterComponentDependencyOld(sceneName, obj.name, scriptableObject.name);
                        }
                    }
                }
            }
        }

        public void RegisterAssetDependentOld(ScriptableObject asset, string dependentScene, string dependentName)
        {
            if(asset is EventBase) {
                RegisterEventDependent(asset, dependentScene, dependentName);
            } else if (asset is VariableBase) {
                RegisterVariableDependent(asset, dependentScene, dependentName);
            } else {
                Debug.LogWarning("Scriptable object type not recognized : " + asset.name);
            }
            AssetDatabase.Refresh();
        }

        public void RegisterComponentDependencyOld(string componentScene, string componentName, string dependencyName)
        {
            string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + componentScene });
            string filePath = Utils.GetFilePath(componentRegistryPath, componentName, ".json");

            RegisterDependencyOld(filePath, "Dependencies", dependencyName);
        }

        public void RegisterComponentDependency(string componentScene, string componentName, JSONNode dependencyNode)
        {
            string componentRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + componentScene });
            string filePath = Utils.GetFilePath(componentRegistryPath, componentName, ".json");

            RegisterDependencyOld(filePath, "Dependencies", dependencyNode);
        }

        public void RegisterEventDependent(ScriptableObject eventAsset, string dependentScene, string dependentName)
        {
            string eventRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Events" });
            string filePath = Utils.GetFilePath(eventRegistryPath, eventAsset.name, ".json");

            RegisterDependencyOld(filePath, dependentScene, dependentName);
        }

        public void RegisterVariableDependent(ScriptableObject variableAsset, string dependentScene, string dependentName)
        {
            string eventRegistryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Variables" });
            string filePath = Utils.GetFilePath(eventRegistryPath, variableAsset.name, ".json");

            RegisterDependencyOld(filePath, dependentScene, dependentName);
        }

        static void RegisterDependencyOld(string filePath, string dependencyKey, string dependencyValue)
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

        static void RegisterDependencyOld(string filePath, string dependencyKey, JSONNode dependencyValue)
        {
            if (File.Exists(filePath)) {

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