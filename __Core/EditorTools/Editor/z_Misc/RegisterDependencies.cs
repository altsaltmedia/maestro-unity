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
using System;
using UnityEngine.Events;
using UnityEngine.Timeline;
using System.Linq;

namespace AltSalt
{
    public class RegisterDependencies : EditorWindow
    {
        List<ScriptableObject> scriptableObjectList = new List<ScriptableObject>();
        string targetScenePath = "";
        string targetSceneName = "";
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
            GUILayout.Space(10);

            ShowSingleSceneOptions();

            GUILayout.Space(20);

            ShowAllSceneOptions();

            GUILayout.Space(20);

            if (GUILayout.Button("Remove Dependencies Folder")) {
                if (EditorUtility.DisplayDialog("Remove dependencies folder?", "This will delete the dependencies folder at " + Utils.GetProjectPath() + "/z_Dependencies", "Proceed", "Cancel")) {
                    RemoveDependenciesFolder();
                }
            }
        }

        void ShowSingleSceneOptions()
        {
            if (targetScenePath.Length > 0) {
                GUILayout.Label("Ready to find dependencies in '" + targetSceneName + "'.");
            } else {
                GUILayout.Label("Please load a target scene.");
            }
            if (GUILayout.Button("Load target scene")) {
                StoreTargetSceneInfo();
            }

            EditorGUI.BeginDisabledGroup(targetScenePath.Length == 0);
            if (GUILayout.Button("Register Dependencies (Target Scene)")) {
                TriggerRegisterDependenciesInTargetScene();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(targetScenePath.Length == 0);
            if (GUILayout.Button(new GUIContent("Clear Hidden Values (Target Scene)", "If you're finding erroneous dependencies on objects, use this to clear any unused values."))) {
                ClearHiddenValuesInTargetScene();
            }
            EditorGUI.EndDisabledGroup();
        }

        void ShowAllSceneOptions()
        {
            if (callbackScenePath.Length > 0) {
                GUILayout.Label("Callback scene '" + callbackSceneName + "' loaded.");
            } else {
                GUILayout.Label("No scene loaded.");
            }

            if (GUILayout.Button("Load callback scene")) {
                StoreCallbackSceneInfo();
            }

            EditorGUI.BeginDisabledGroup(callbackScenePath.Length == 0);
            if (GUILayout.Button("Register Dependencies (All Scenes)")) {
                TriggerRegisterAllDependencies();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(callbackScenePath.Length == 0);
            if (GUILayout.Button(new GUIContent("Clear Hidden Values (All Scenes)", "If you're finding erroneous dependencies on objects, use this to clear any unused values."))) {
                ClearAllHiddenValues();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void StoreTargetSceneInfo()
        {
            targetScenePath = EditorUtility.OpenFilePanel("Select text file", targetScenePath, "unity");
            if (File.Exists(targetScenePath)) {
                targetSceneName = Path.GetFileNameWithoutExtension(targetScenePath);
            }
        }

        private void StoreCallbackSceneInfo()
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

        void PopulateScriptableObjectList()
        {
            string scriptableObjectQuery = typeof(RegisterableScriptableObject).Name;

            string[] scriptableObjectGuids = AssetDatabase.FindAssets(string.Format("t:{0}", scriptableObjectQuery));

            int searchCounter = 0;

            foreach (string scriptableObjectGuid in scriptableObjectGuids) {
                EditorUtility.DisplayProgressBar("Scanning project", "Populating scriptable object list", 1.0f / scriptableObjectGuids.Length * searchCounter);
                string assetPath = AssetDatabase.GUIDToAssetPath(scriptableObjectGuid);
                ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));
                scriptableObjectList.Add(scriptableObject);

                searchCounter++;
            }
            EditorUtility.ClearProgressBar();
        }

        void ClearScriptableObjectList()
        {
            scriptableObjectList.Clear();
        }

        void TriggerRegisterDependenciesInTargetScene()
        {
            string componentRegistryScenePath = Utils.GetProjectPath() + "/z_Dependencies/Components/" + targetSceneName;

            if (EditorUtility.DisplayDialog("Register dependencies from " + targetSceneName + "?", "This will delete the component dependencies folder at " + componentRegistryScenePath + " then scan " + targetSceneName +
                " for dependencies. \n\nThis will likely put the variables folder out of sync (use all scenes registration to sync all dependencies)", "Proceed", "Cancel")) {

                if (Directory.Exists(componentRegistryScenePath)) {
                    Directory.Delete(componentRegistryScenePath, true);
                    AssetDatabase.Refresh();
                }

                PopulateScriptableObjectList();
                Scene scene = EditorSceneManager.OpenScene(targetScenePath);
                FindDependencies(scene.name);
                AssetDatabase.Refresh();
                ClearScriptableObjectList();
            }
        }

        void TriggerRegisterAllDependencies()
        {
            if (EditorUtility.DisplayDialog("Register dependencies in all scenes?", "This will scan the entire project and recreate the dependencies folder. This cannot be undone.", "Proceed", "Cancel")) {

                RemoveDependenciesFolder();
                PopulateScriptableObjectList();
                string[] sceneGuids = GetSceneGuids();
                for (int i = 0; i < sceneGuids.Length; i++) {

                    Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));
                    EditorUtility.DisplayProgressBar("Scanning project", "Finding dependencies in " + scene.name, 1.0f / sceneGuids.Length * i);

                    FindDependencies(scene.name);
                }
                AssetDatabase.Refresh();
                LoadCallbackScene();
                EditorUtility.ClearProgressBar();
                ClearScriptableObjectList();
            }
        }

        void FindDependencies(string sceneName)
        {
            FindPlayableDependencies(sceneName);
            FindComponentDependencies(sceneName);
        }

        void FindPlayableDependencies(string sceneName)
        {
            var playableDirectors = Resources.FindObjectsOfTypeAll(typeof(PlayableDirector)) as PlayableDirector[];
            if (playableDirectors == null) return;

            foreach (PlayableDirector playableDirector in playableDirectors) {

                // Ignore prefabs
                if (EditorUtility.IsPersistent(playableDirector.gameObject) == true) {
                    continue;
                }

                PlayableAsset playableAsset = playableDirector.playableAsset;
                IEnumerable<PlayableBinding> playableBindings = playableAsset.outputs;

                foreach (PlayableBinding playableBinding in playableBindings) {
                    TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                    // Skip playable bindings that don't contain track assets (e.g. markers)
                    if (trackAsset == null) {
                        continue;
                    }

                    foreach (TimelineClip clip in trackAsset.GetClips()) {
                        Type type = clip.asset.GetType();

                        // Only parse through clips that have been flagged for registration
                        if (type.IsSubclassOf(typeof(RegisterablePlayableAsset))) {
                            FieldInfo[] fields = clip.asset.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                            TraverseObjectFields(clip.asset, playableDirector, sceneName);
                        }
                    }
                }
            }
        }

        void FindComponentDependencies(string sceneName)
        {
            var components = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];
            if (components == null) return;

            foreach (Component component in components) {

                Type componentType = component.GetType();

                // Only search for AltSalt custom scripts & ignore boilerplate values from
                // SerializableElement, prefabs and other miscellaneous components w/o instances
                if (componentType.Namespace != "AltSalt" || component.GetType().IsSubclassOf(typeof(SerializableElement)) || EditorUtility.IsPersistent(component.gameObject) == true || component == null) {
                    continue;
                }

                // Ignore any components that have been flagged for skipping. Generally these are components
                // that contain default behaviour - and since we're specifically trying to record all *custom* behaviour,
                // we don't need to keep track of these defaults.
                if (componentType.GetInterfaces().Contains(typeof(ISkipRegistration))) {
                    ISkipRegistration skipRegistration = component as ISkipRegistration;
                    if (skipRegistration.DoNotRecord == true) {
                        continue;
                    }
                }

                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                TraverseObjectFields(component, component, sceneName);
            }
        }

        void TraverseObjectFields(object source, Component rootComponent, string sceneName)
        {
            FieldInfo[] fieldInfoList = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            // Check types flagged here for dependencies and drill down into the fields if necessary
            foreach (FieldInfo fieldInfo in fieldInfoList) {

                var fieldValue = fieldInfo.GetValue(source);

                if (fieldValue == null) {
                    continue;
                }

                Type fieldType = fieldInfo.GetValue(source).GetType();

                // Event triggers can contain dependencies at the root level or inside a list
                if (fieldType.IsSubclassOf(typeof(EventTriggerBase)) || fieldType.IsSubclassOf(typeof(PlayableBehaviourTriggerBase))) {

                    ParseEventTriggerBase(fieldType, fieldValue, rootComponent, sceneName);

                // Condition responses contain both conditions, as well as responses in the form of Unity events
                } else if (fieldType.IsSubclassOf(typeof(ConditionResponseTriggerBase))) {

                    ParseConditionResponseTriggerBase(fieldType, fieldValue, rootComponent, sceneName);

                // Compare any field that is a scriptable object to our master list and register dependencies accordingly
                } else if (fieldValue is RegisterableScriptableObject) {

                    ParseScriptableObjectReference(fieldValue, fieldInfo, rootComponent, sceneName);

                // If we reach a variable references, such as BoolReference, FloatReference, etc., we don't need to drill further
                } else if (fieldType.IsSubclassOf(typeof(VariableReferenceBase))) {

                    ParseVariableReference(fieldType, source, fieldValue, fieldInfo, rootComponent, sceneName);

                // If we come to a Unity event, register all relevant event info
                } else if (fieldType.IsSubclassOf(typeof(UnityEventBase))) {

                    ParseUnityEvent(fieldValue, rootComponent, sceneName);

                // If the field is a list, parse and drill down to find dependencies or other flagged types
                } else if (typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType.IsGenericType) {

                    ParseFieldList(fieldValue, fieldInfo, rootComponent, sceneName);
                }
            }
        }

        void ParseEventTriggerBase(Type triggerType, object triggerObject, Component rootComponent, string sceneName)
        {
            FieldInfo[] childFieldInfoList = triggerType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo childFieldInfo in childFieldInfoList) {

                var fieldValue = childFieldInfo.GetValue(triggerObject);

                if (fieldValue == null) {
                    continue;
                }

                Type fieldType = childFieldInfo.GetValue(triggerObject).GetType();

                if (fieldValue != null && typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType.IsGenericType) {
                    ParseFieldList(fieldValue, childFieldInfo, rootComponent, sceneName);
                } else if (fieldValue is RegisterableScriptableObject) {
                    ParseScriptableObjectReference(fieldValue, childFieldInfo, rootComponent, sceneName);
                }
            }
        }

        void ParseConditionResponseTriggerBase(Type conditionResponseType, object conditionResponseObject, Component rootComponent, string sceneName)
        {
            FieldInfo[] childFieldInfoList = conditionResponseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo childFieldInfo in childFieldInfoList) {

                var fieldValue = childFieldInfo.GetValue(conditionResponseObject);
                Type fieldType = childFieldInfo.GetValue(conditionResponseObject).GetType();

                if (fieldValue != null && typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType.IsGenericType) {

                    IEnumerable conditionResponses = fieldValue as IEnumerable;
                    if (conditionResponses != null) {

                        foreach (object conditionResponse in conditionResponses) {

                            FieldInfo[] conditionResponseFields = conditionResponse.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                            foreach(FieldInfo conditionResponseField in conditionResponseFields) {

                                var conditionResponseFieldValue = conditionResponseField.GetValue(conditionResponse);

                                if(conditionResponseFieldValue == null) {
                                    continue;
                                }

                                Type conditionResponseFieldType = conditionResponseField.GetValue(conditionResponse).GetType();

                                if (conditionResponseFieldType.IsSubclassOf(typeof(VariableReferenceBase))) {

                                    ParseVariableReference(conditionResponseFieldType, conditionResponse, conditionResponseFieldValue, conditionResponseField, rootComponent, sceneName);

                                } else if (conditionResponseFieldType.IsSubclassOf(typeof(UnityEventBase))) {

                                    ParseUnityEvent(conditionResponseFieldValue, rootComponent, sceneName);

                                } else if (conditionResponseFieldValue is string){
                                    RegisterComponentConditionResponseDesc(conditionResponseFieldValue, rootComponent, sceneName);
                                }
                            }
                        }
                    }
                }
            }
        }

        void ParseScriptableObjectReference(object scriptableObjValue, FieldInfo fieldInfo, Component rootComponent, string sceneName)
        {
            foreach (ScriptableObject scriptableObject in scriptableObjectList) {

                if ((ScriptableObject)scriptableObjValue == scriptableObject) {

                    // If the root type name equals the field info name, that means we're
                    // at the root of the object, so let's use the field name as a label;
                    // otherwise, use the name of the type that declared the field.
                    if (rootComponent.GetType().Name == fieldInfo.DeclaringType.Name) {
                        RegisterAssetReference(scriptableObject, rootComponent, fieldInfo.Name, sceneName);
                        RegisterComponentDependency(scriptableObject, rootComponent, fieldInfo.Name, sceneName);
                    } else {
                        RegisterAssetReference(scriptableObject, rootComponent, fieldInfo.DeclaringType.Name, sceneName);
                        RegisterComponentDependency(scriptableObject, rootComponent, fieldInfo.DeclaringType.Name, sceneName);
                    }
                    break;
                }
            }
        }

        void ParseVariableReference(Type varReferenceType, object parentObject, object variableReferenceObject, FieldInfo parentFieldInfo, Component rootComponent, string sceneName)
        {
            FieldInfo[] varRefFields = varReferenceType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            string referenceLabel = parentFieldInfo.GetValue(parentObject).GetType().Name;

            JSONNode varRefData = new JSONObject();

            foreach (FieldInfo varRefField in varRefFields) {

                var varRefFieldValue = varRefField.GetValue(variableReferenceObject);

                if (varRefFieldValue != null) {

                    Type varRefFieldType = varRefFieldValue.GetType();

                    // Skip Sirenix lists and other enumerables
                    if (typeof(IEnumerable).IsAssignableFrom(varRefFieldType) || varRefFieldType.IsGenericType) {
                        continue;
                    } else if (varRefFieldValue is ScriptableObject) {
                        ScriptableObject childFieldObject = varRefFieldValue as ScriptableObject;
                        varRefData.Add(varRefField.Name, childFieldObject.name);

                        RegisterAssetReference(childFieldObject, rootComponent, varRefField.DeclaringType.Name, sceneName);
                    } else {
                        varRefData.Add(varRefField.Name, varRefFieldValue.ToString());
                    }
                }
            }

            RegisterComponentVariableReference(referenceLabel, varRefData, rootComponent, sceneName);
        }

        void ParseUnityEvent(object unityEventObject, Component rootComponent, string sceneName)
        {
            UnityEventBase unityEvent = unityEventObject as UnityEventBase;
            
            if (unityEvent != null) {

                JSONNode eventData = new JSONArray();

                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {

                    JSONNode eventItem = new JSONObject();

                    UnityEngine.Object target = unityEvent.GetPersistentTarget(i);

                    if (target is ScriptableObject) {
                        foreach (ScriptableObject scriptableObject in scriptableObjectList) {
                            if ((ScriptableObject)target == scriptableObject) {
                                RegisterAssetReference(scriptableObject, rootComponent, "UnityEventTarget", sceneName);
                                break;
                            }
                        }
                    }

                    eventItem.Add("target", target.name);
                    eventItem.Add("method", unityEvent.GetPersistentMethodName(i));
                    eventData.Add(eventItem);
                }

                RegisterComponentUnityEvents(eventData, rootComponent, sceneName);
            }
        }

        // If the field is a list, register any scriptable objects in the list. Otherwise, recurse and drill
        // further into the list to see if it contains one of our types flagged above
        void ParseFieldList(object enumerableObject, FieldInfo fieldInfo, Component rootComponent, string sceneName)
        {
            IEnumerable enumerable = enumerableObject as IEnumerable;
            if (enumerable != null) {

                foreach (object element in enumerable) {

                    if (element is ScriptableObject) {
                        foreach (ScriptableObject scriptableObject in scriptableObjectList) {

                            if ((ScriptableObject)element == scriptableObject) {
                                RegisterAssetReference(scriptableObject, rootComponent, fieldInfo.Name, sceneName);
                                RegisterComponentListDependency(scriptableObject, rootComponent, fieldInfo.Name, sceneName);
                                break;
                            }
                        }
                    } else if (element is string || element is bool || element is float) {
                        RegisterComponentListDependency(element, rootComponent, fieldInfo.Name, sceneName);
                    } else {
                        TraverseObjectFields(element, rootComponent, sceneName);
                    }
                }
            }
        }

        void RegisterAssetReference(ScriptableObject scriptableObject, Component rootComponent, string fieldName, string sceneName)
        {
            string filePath = GetAssetRegistryFilePath(scriptableObject);

            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[sceneName][rootComponent.gameObject.name][rootComponent.GetType().Name].Add(fieldName);

            WriteData(filePath, jsonData);
        }

        void RegisterComponentDependency(ScriptableObject scriptableObject, Component rootComponent, string fieldName, string sceneName)
        {
            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);

            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[rootComponent.GetType().Name][fieldName].Add(scriptableObject.name);

            WriteData(filePath, jsonData);
        }

        void RegisterComponentListDependency(ScriptableObject scriptableObject, Component rootComponent, string fieldName, string sceneName)
        {
            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);

            JSONNode jsonData = GetRootJsonNode(filePath);

            JSONNode jsonObject = new JSONArray();
            jsonObject.Add(scriptableObject.name);

            jsonData[rootComponent.GetType().Name][fieldName].Add(scriptableObject.name);

            WriteData(filePath, jsonData);
        }

        void RegisterComponentListDependency(object valueObject, Component rootComponent, string fieldName, string sceneName)
        {
            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);

            JSONNode jsonData = GetRootJsonNode(filePath);

            JSONNode jsonObject = new JSONArray();
            jsonObject.Add(valueObject.ToString());

            jsonData[rootComponent.GetType().Name][fieldName].Add(valueObject.ToString());
            WriteData(filePath, jsonData);
        }

        void RegisterComponentVariableReference(string referenceLabel, JSONNode jsonNode, Component rootComponent, string sceneName)
        {
            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);

            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[rootComponent.GetType().Name][referenceLabel].Add(jsonNode);

            WriteData(filePath, jsonData);
        }

        void RegisterComponentUnityEvents(JSONNode eventData, Component rootComponent, string sceneName)
        {
            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);

            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[rootComponent.GetType().Name]["UnityResponses"].Add(eventData);

            WriteData(filePath, jsonData);
        }

        void RegisterComponentConditionResponseDesc(object conditionResponseDescObject, Component rootComponent, string sceneName)
        {
            string conditionDescription = conditionResponseDescObject as string;

            string filePath = GetComponentRegistryFilePath(sceneName, rootComponent.gameObject.name);
            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[rootComponent.GetType().Name]["Conditions"].Add(conditionDescription);

            WriteData(filePath, jsonData);
        }

        void ClearHiddenValues(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath);

            var components = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];
            if (components == null) return;

            foreach (Component component in components) {

                if (component.GetType().Namespace != "AltSalt" || component == null) {
                    continue;
                }

                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                foreach (FieldInfo fieldInfo in fields) {

                    if (fieldInfo.GetValue(component) == null) {
                        continue;
                    }

                    Type type = fieldInfo.GetValue(component).GetType();

                    if (type.GetInterfaces().Contains(typeof(IClearHiddenValues))) {
                        IClearHiddenValues clearableField = fieldInfo.GetValue(component) as IClearHiddenValues;
                        clearableField.ClearHiddenValues();
                        EditorSceneManager.MarkSceneDirty(scene);
                    }
                }
            }
            EditorSceneManager.SaveScene(scene);
        }

        void ClearHiddenValuesInTargetScene()
        {
            if (EditorUtility.DisplayDialog("Clear hidden values in " + targetSceneName+ "?", "This will search " + targetSceneName + " for hidden values " +
                "on relevant components and erase them. This action cannot be undone.", "Proceed", "Cancel")) {

                ClearHiddenValues(targetScenePath);

            }
        }

        void ClearAllHiddenValues()
        {
            if (EditorUtility.DisplayDialog("Clear hidden values in all scenes?", "This will search all scenes for hidden values" +
                "on relevant components and erase them. This action cannot be undone.", "Proceed", "Cancel")) {
                string[] sceneGuids = GetSceneGuids();
                for (int i = 0; i < sceneGuids.Length; i++) {

                    Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));
                    EditorUtility.DisplayProgressBar("Scanning project", "Finding hidden values in " + scene.name, 1.0f / sceneGuids.Length * i);

                    ClearHiddenValues(scene.name);
                }
                LoadCallbackScene();
                EditorUtility.ClearProgressBar();
            }
        }

        void LoadCallbackScene()
        {
            EditorSceneManager.OpenScene(callbackScenePath);
        }

        void RemoveDependenciesFolder()
        {
            string dependenciesFolderPath = Utils.GetProjectPath() + "/z_Dependencies";

            if(Directory.Exists(dependenciesFolderPath)) {
                Directory.Delete(dependenciesFolderPath, true);
            }
            
            AssetDatabase.Refresh();
        }

        string GetComponentRegistryFilePath(string sceneName, string gameObjectName)
        {
            string registryPath = Utils.GetDirectory(new string[] { "/z_Dependencies", "/Components", "/" + sceneName });
            return Utils.GetFilePath(registryPath, gameObjectName, ".json");
        }

        string GetAssetRegistryFilePath(ScriptableObject asset)
        {
            string[] directoryPath = new string[3];
            directoryPath[0] = "/z_Dependencies";

            if (asset is EventBase) {
                directoryPath[1] = "/Events";
            } else {
                directoryPath[1] = "/Variables";
            }
            directoryPath[2] = "/" + asset.GetType().Name;

            string registryPath = Utils.GetDirectory(directoryPath);
            return Utils.GetFilePath(registryPath, asset.name, ".json");
        }

        JSONNode GetRootJsonNode(string filePath)
        {
            JSONNode rootNode;

            if (File.Exists(filePath)) {
                string savedData = File.ReadAllText(filePath);
                rootNode = JSON.Parse(savedData);

            } else {
                rootNode = JSON.Parse("{}");
            }

            return rootNode;
        }

        void WriteData(string filePath, JSONNode jsonData)
        {
            File.WriteAllText(filePath, jsonData.ToString(2));
        }

    }
}
