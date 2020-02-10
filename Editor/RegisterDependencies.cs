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
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    public class RegisterDependencies : EditorWindow
    {
        List<ScriptableObject> scriptableObjectList = new List<ScriptableObject>();
        string targetScenePath = "";
        string targetSceneName = "";
        string callbackScenePath = "";
        string callbackSceneName = "";

        [SerializeField]
        private IntVariable counter;
        
        [SerializeField]
        private IntVariable maxValue;

        [MenuItem("Tools/Maestro/Register Dependencies")]
        public static void ShowWindow()
        {
            var window = CreateInstance<RegisterDependencies>();
            window.ShowTitle();
            window.Show();
        }

        private void ShowTitle()
        {
            if (counter == null) {
                counter = Utils.GetScriptableObject("RegisterDependenciesCounter") as IntVariable;
                maxValue = Utils.GetScriptableObject("MaxValue") as IntVariable;
            }

            titleContent = new GUIContent("Register Dependencies");
        }

        public void OnGUI()
        {
            GUILayout.Space(10);

            ShowSingleSceneOptions();

            GUILayout.Space(20);

            ShowAllSceneOptions();

            GUILayout.Space(20);

            if (GUILayout.Button("Remove Dependencies Folder")) {
                if (EditorUtility.DisplayDialog("Remove dependencies folder?", "This will delete the dependencies folder at " + Utils.projectPath + "/z_Dependencies", "Proceed", "Cancel")) {
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

        private void ClearScriptableObjectList()
        {
            scriptableObjectList.Clear();
        }

        private void TriggerRegisterDependenciesInTargetScene()
        {
            string componentRegistryScenePath = Utils.projectPath + "/z_Dependencies/Components/" + targetSceneName;

            if (EditorUtility.DisplayDialog("Register dependencies from " + targetSceneName + "?", "This will delete the component dependencies folder at " + componentRegistryScenePath + " then scan " + targetSceneName +
                " for dependencies. \n\nThis will likely put the variables folder out of sync (use all scenes registration to sync all dependencies)", "Proceed", "Cancel")) {

                if (Directory.Exists(componentRegistryScenePath)) {
                    Directory.Delete(componentRegistryScenePath, true);
                    AssetDatabase.Refresh();
                }

                PopulateScriptableObjectList();
                Scene scene = EditorSceneManager.OpenScene(targetScenePath);
                FindDependencies();
                AssetDatabase.Refresh();
                ClearScriptableObjectList();
            }
        }

        private void TriggerRegisterAllDependencies()
        {
            if (EditorUtility.DisplayDialog("Register dependencies in all scenes?", "This will scan the entire project and recreate the dependencies folder. This cannot be undone.", "Proceed", "Cancel")) {

                RemoveDependenciesFolder();
                PopulateScriptableObjectList();
                string[] sceneGuids = GetSceneGuids();
                for (int i = 0; i < sceneGuids.Length; i++) {

                    Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));
                    EditorUtility.DisplayProgressBar("Scanning project", "Finding dependencies in " + scene.name, 1.0f / sceneGuids.Length * i);

                    FindDependencies();
                }
                AssetDatabase.Refresh();
                LoadCallbackScene();
                EditorUtility.ClearProgressBar();
                ClearScriptableObjectList();
            }
        }

        private void FindDependencies()
        {
            counter.SetValue(0);
            //FindPlayableDependencies();
            FindComponentDependencies();
        }

        private static void FindPlayableDependencies()
        {
            var playableDirectors = Resources.FindObjectsOfTypeAll(typeof(PlayableDirector)) as PlayableDirector[];
            if (playableDirectors == null) return;

            foreach (PlayableDirector playableDirector in playableDirectors) {

                // Ignore prefabs
                if (EditorUtility.IsPersistent(playableDirector.gameObject) == true) {
                    continue;
                }

                RegisterPlayableDirectorDependencies(playableDirector);
            }
        }

        private static void RegisterPlayableDirectorDependencies(PlayableDirector playableDirector)
        {
            // Let's get all of the references on each playable director
            var trackAssets = TimelineUtils.GetAllTracks(playableDirector.playableAsset as TimelineAsset);

            // First, let's get and sort our clips
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            foreach (TrackAsset trackAsset in trackAssets) {
                timelineClips.AddRange(trackAsset.GetClips());
            }
            timelineClips.Sort(new Utils.ClipTimeSort());
            
            // Next, drill down and find our variable references
            foreach (var timelineClip in timelineClips) {
                
                EditorUtility.DisplayProgressBar("Registering playable assets", $"Scanning {playableDirector.name} for references",
                    1.0f / timelineClips.Count * timelineClips.IndexOf(timelineClip));

                Object clipAsset = timelineClip.asset;
                Type clipType = clipAsset.GetType();

                // Only parse through clip assets that have been flagged for registration
                if (clipType.IsSubclassOf(typeof(RegisterablePlayableAsset)) == false) continue;
                
                // The actual references are stored on the serialized
                // playable behaviours stored as fields on the clip assets
                var triggerBehaviour = GetTimelineTriggerBehaviour(clipAsset);
                if (triggerBehaviour != null) {
                    List<ReferenceBase> references = GetVariableReferences(triggerBehaviour);

                    foreach (var reference in references) {
                        RegisterTimelineTriggerReference(timelineClip, playableDirector, reference);
                    }
                }
            }
            
            EditorUtility.ClearProgressBar();
        }

        private static TimelineTriggerBehaviour GetTimelineTriggerBehaviour(Object clipAsset)
        {
            FieldInfo[] fieldInfoList = clipAsset.GetType().GetFields(); 
            
            foreach (FieldInfo fieldInfo in fieldInfoList) {
                
                var fieldValue = fieldInfo.GetValue(clipAsset);
                Type fieldType = fieldInfo.FieldType;

                if (fieldType.IsSubclassOf(typeof(TimelineTriggerBehaviour))) {
                    return fieldValue as TimelineTriggerBehaviour;
                }
            }

            return null;
        }
        
        private static List<ReferenceBase> GetVariableReferences(TimelineTriggerBehaviour behaviour)
        {
            FieldInfo[] fieldInfoList = behaviour.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<ReferenceBase> referenceBases = new List<ReferenceBase>();
            
            foreach (FieldInfo fieldInfo in fieldInfoList) {

                var fieldValue = fieldInfo.GetValue(behaviour);
                Type fieldType = fieldInfo.FieldType;

                if (fieldValue is IEnumerable enumerable &&
                    fieldType.GetGenericArguments()[0].IsSubclassOf(typeof(ReferenceBase))) {
                    foreach (object element in enumerable) {
                        referenceBases.Add(element as ReferenceBase);
                    }
                }
            }

            return referenceBases;
        }
        
        private static void RegisterTimelineTriggerReference(TimelineClip timelineClip, PlayableDirector playableDirector, ReferenceBase reference)
        {
            Type referenceType = reference.GetType();
            FieldInfo[] fields = referenceType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            for (int i = 0; i < fields.Length; i++) {
                var fieldValue = fields[i].GetValue(reference);
                
                if (fieldValue is ScriptableObject scriptableObject) {
                    
                    // Register the object at the variable path
                    string filePath = GetAssetRegistryFilePath(scriptableObject);
                    JSONNode jsonData = GetRootJsonNode(filePath);
                    jsonData[$"Scene: {playableDirector.gameObject.scene.name}"][playableDirector.name][timelineClip.parentTrack.name][timelineClip.start.ToString()].Add(referenceType.Name);
                    WriteData(filePath, jsonData);
                    
                    // Also register at the component path
                    filePath = GetSceneRegistryFilePath(playableDirector.gameObject);
                    jsonData = GetRootJsonNode(filePath);
                    jsonData[playableDirector.name][timelineClip.parentTrack.name][scriptableObject.name].Add(timelineClip.start.ToString());
                    WriteData(filePath, jsonData);
                }
            }
        }

        private void FindComponentDependencies()
        {
            var components = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];
            if (components == null) return;

            foreach (Component component in components) {

                Type componentType = component.GetType();

                // Ignore boilerplate values from SerializableElement,
                // prefabs and other miscellaneous components w/o instances
                if (componentType.Namespace == null
                    || component.GetType().IsSubclassOf(typeof(SerializableElement))
                    || EditorUtility.IsPersistent(component.gameObject) == true
                    || component == null) {
                    continue;
                } 
                
                // Only parse AltSalt scripts, as well as selected Unity components
                if(componentType.Namespace.Contains($"{nameof(AltSalt)}.{nameof(Maestro)}") == true
                   || component is Button
                   || component is Slider
                   || component is DOTweenAnimation)
                {
                    // Ignore any components that have been flagged for skipping. Generally these are components
                    // that contain default behaviour - and since we're specifically trying to record all *custom* behaviour,
                    // we don't need to keep track of these defaults.
                    if (componentType.GetInterfaces().Contains(typeof(ISkipRegistration))) {
                        ISkipRegistration skipRegistration = component as ISkipRegistration;
                        if (skipRegistration.skipRegistration == true) {
                            continue;
                        }
                    }

                    FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    TraverseObjectFields(component, component);
                }
            }
        }
        private void TraverseObjectFields(object source, Component rootComponent, string serializedPropertyPath = "")
        {
            FieldInfo[] fieldInfoList = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            if (counter.value > maxValue.value) {
                return;
            }
            
            // Check types flagged here for dependencies and drill down into the fields if necessary
            foreach (FieldInfo fieldInfo in fieldInfoList) {
                
                counter.ApplyChange(1);

                if (counter.value > maxValue.value) {
                    break;
                }
                
                var fieldValue = fieldInfo.GetValue(source);
                Type fieldType = fieldInfo.FieldType;

                if (fieldValue == null || fieldValue is AppSettingsReference) {
                    continue;
                }

                if (fieldType.IsSubclassOf(typeof(UnityEventBase))) {

                    string relativePropertyPath;
                    
                    if (string.IsNullOrEmpty(serializedPropertyPath) == true) {
                        relativePropertyPath = fieldInfo.Name;
                    }
                    else {
                        relativePropertyPath = $"{serializedPropertyPath}.{fieldInfo.Name}";
                    }

                    ParseUnityEvent(fieldValue as UnityEventBase, rootComponent, relativePropertyPath);
                }

                // If the field is a list, parse and drill down to find dependencies or other flagged types


                // if (fieldType.GetInterfaces().Contains(typeof(IConditionResponseTrigger))) {
                //
                //     ParseConditionResponseTriggerBase(fieldType, fieldValue, rootComponent, sceneName);
                //
                //     // Compare any field that is a scriptable object to our master list and register dependencies accordingly
                // }
                // else if (fieldValue is RegisterableScriptableObject) {
                //
                //     RegisterScriptableObject(fieldValue, fieldInfo, rootComponent, sceneName);
                //
                // // If we reach a variable references, such as BoolReference, FloatReference, etc., we don't need to drill further
                // }
                //
                else if (fieldType.IsSubclassOf(typeof(ReferenceBase))) {
                
                    string relativePropertyPath;
                    
                    if (string.IsNullOrEmpty(serializedPropertyPath) == true) {
                        relativePropertyPath = fieldInfo.Name;
                    }
                    else {
                        relativePropertyPath = $"{serializedPropertyPath}.{fieldInfo.Name}";
                    }
                    
                    ParseReference(fieldValue as ReferenceBase, rootComponent, relativePropertyPath);
                }

                else if (fieldType.GetInterfaces().Contains(typeof(IEnumerable)) && fieldType.IsGenericType) {
                    
                    string relativePropertyPath;
                    
                    if (string.IsNullOrEmpty(serializedPropertyPath) == true) {
                        relativePropertyPath = fieldInfo.Name;
                    }
                    else {
                        relativePropertyPath = $"{serializedPropertyPath}.{fieldInfo.Name}";
                    }
                
                    ParseFieldList(fieldValue as IEnumerable, fieldInfo, rootComponent, relativePropertyPath);
                }
                
                else if(fieldType.GetInterfaces().Contains(typeof(IRegisterActionData)) == true
                        && fieldType.IsAbstract == false
                        || fieldType.GetInterfaces().Contains(typeof(IRegisterConditionResponse)) == true)

                {
                    string relativePropertyPath;
                    
                    if (string.IsNullOrEmpty(serializedPropertyPath) == true) {
                        relativePropertyPath = fieldInfo.Name;
                    }
                    else {
                        relativePropertyPath = $"{serializedPropertyPath}.{fieldInfo.Name}";
                    }
                    
                    TraverseObjectFields(fieldValue, rootComponent, relativePropertyPath);
                }
            }
        }
    


        // private void ParseConditionResponseTriggerBase(Type conditionResponseType, object conditionResponseObject, Component rootComponent, string sceneName)
        // {
        //     FieldInfo[] childFieldInfoList = conditionResponseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //
        //     foreach (FieldInfo childFieldInfo in childFieldInfoList) {
        //
        //         var fieldValue = childFieldInfo.GetValue(conditionResponseObject);
        //         Type fieldType = childFieldInfo.GetValue(conditionResponseObject).GetType();
        //
        //         if (fieldValue != null && typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType.IsGenericType) {
        //
        //             IEnumerable conditionResponses = fieldValue as IEnumerable;
        //             if (conditionResponses != null) {
        //
        //                 foreach (object conditionResponse in conditionResponses) {
        //
        //                     FieldInfo[] conditionResponseFields = conditionResponse.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //
        //                     foreach(FieldInfo conditionResponseField in conditionResponseFields) {
        //
        //                         var conditionResponseFieldValue = conditionResponseField.GetValue(conditionResponse);
        //
        //                         if(conditionResponseFieldValue == null) {
        //                             continue;
        //                         }
        //
        //                         Type conditionResponseFieldType = conditionResponseField.GetValue(conditionResponse).GetType();
        //
        //                         if (conditionResponseFieldType.IsSubclassOf(typeof(ReferenceBase))) {
        //
        //                             ParseReference(conditionResponseFieldType, conditionResponse, conditionResponseFieldValue, conditionResponseField, rootComponent, sceneName);
        //
        //                         } else if (conditionResponseFieldType.IsSubclassOf(typeof(UnityEventBase))) {
        //
        //                             //ParseUnityEvent(conditionResponseFieldValue, rootComponent, sceneName);
        //
        //                         } else if (conditionResponseFieldValue is string){
        //                             
        //                             RegisterComponentConditionResponseDesc(conditionResponseFieldValue, rootComponent, sceneName);
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }

        private void ParseUnityEvent(UnityEventBase unityEvent, Component rootComponent, string serializedPropertyPath)
        {
            if (unityEvent.GetPersistentEventCount() < 1) {
                return;
            }
            
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return;
            }

            JSONNode eventData = new JSONArray();
            
            UnityEventParameter[] parameters = UnityEventUtils.GetUnityEventParameters(serializedProperty);
            List<UnityEventData> unityEventData = UnityEventUtils.GetUnityEventData(unityEvent, parameters);

            for (int i = 0; i < unityEventData.Count; i++) {

                JSONNode eventItem = new JSONObject();

                eventItem.Add("target", unityEventData[i].targetName);
                eventItem.Add("method", unityEventData[i].methodName);

                if (string.IsNullOrEmpty(unityEventData[i].parameter.typeName) == false) {
                    eventItem.Add("argument type", unityEventData[i].parameter.typeName);
                }
                if(string.IsNullOrEmpty(unityEventData[i].parameter.valueName) == false) {
                    eventItem.Add("parameter", unityEventData[i].parameter.valueName);
                }
                
                // Register with a helpful "this" indicator when
                // looking at the event data from the variable context 
                
                if (unityEvent.GetPersistentTarget(i) is ScriptableObject target) {
                    eventItem["target"] = eventItem["target"] + " (this)";
                    RegisterPersistentUnityEventReference(target, rootComponent, serializedPropertyPath, eventItem);
                    eventItem["target"] = unityEventData[i].targetName;
                }
                
                if (unityEventData[i].parameter.value is ScriptableObject parameter) {
                    eventItem["parameter"] = eventItem["parameter"] + " (this)";
                    RegisterPersistentUnityEventReference(parameter, rootComponent, serializedPropertyPath, eventItem);
                    eventItem["parameter"] = unityEventData[i].parameter.valueName;
                }
                
                eventData.Add(eventItem);
            }

            RegisterSceneUnityEventData(eventData, rootComponent, serializedPropertyPath);
        }

        private void RegisterPersistentUnityEventReference(ScriptableObject target, Component rootComponent, string serializedPropertypath, JSONNode eventItem)
        {
            foreach (ScriptableObject scriptableObject in scriptableObjectList) {
                
                if (target == scriptableObject) {
                    string filePath = GetAssetRegistryFilePath(scriptableObject);
                    JSONNode jsonData = GetRootJsonNode(filePath);

                    jsonData[$"Scene: {rootComponent.gameObject.scene.name}"]
                        [$"Object: {rootComponent.gameObject.name} > {rootComponent.GetType().Name} > {SanitizePropertyPath(serializedPropertypath)}"].Add(eventItem);
                    WriteData(filePath, jsonData);
                    
                    break;
                }
            }
        }

        void RegisterSceneUnityEventData(JSONNode eventData, Component rootComponent, string serializedPropertyPath)
        {
            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);

            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[$"Component: {rootComponent.GetType().Name} > {SanitizePropertyPath(serializedPropertyPath)}"]["Unity Event"].Add(eventData);

            WriteData(filePath, jsonData);
        }
        
        private void ParseReference(ReferenceBase reference, Component rootComponent, string serializedPropertyPath)
        {
            FieldInfo[] fields = reference.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(reference);
                Type fieldType = field.FieldType;

                if (fieldValue != null) {
                    
                    // Skip Sirenix lists and other enumerables
                    if (fieldType.GetInterfaces().Contains(typeof(IEnumerable)) || fieldType.IsGenericType) {
                        ParseFieldList(fieldValue as IEnumerable, field, rootComponent, serializedPropertyPath);
                        
                    } else if (fieldValue is ScriptableObject scriptableObject) {
                        RegisterAssetReference(scriptableObject, rootComponent, serializedPropertyPath);
                        RegisterSceneDependency(scriptableObject, rootComponent, serializedPropertyPath);
                    }
                }
            }
        }

        private static string SanitizePropertyPath(string serializedPropertyPath)
        {
            string sanitizedPath = Regex.Replace(serializedPropertyPath, @"Array.data\[[0-9]\]", x =>
                {
                    return Regex.Match(x.Value, @"[0-9]").Value;
                });
            sanitizedPath = Regex.Replace(sanitizedPath, @"_[a-z]", x =>
            {
                return x.Value.Replace("_", "").Capitalize();
            });
            sanitizedPath = sanitizedPath.Replace(".", " > ");
            return sanitizedPath;
        }

        // If the field is a list, register any scriptable objects in the list. Otherwise, recurse and drill
        // further into the list to see if it contains one of our types flagged above
        private void ParseFieldList(IEnumerable list, FieldInfo fieldInfo, Component rootComponent, string serializedPropertyPath)
        {
            int counter = 0;
            if (list != null) {
                foreach (var listItem in list) {
                    string relativePropertyPath = $"{serializedPropertyPath}.Array.data[{counter}]";

                    if (listItem is ISkipRegistration skipRegistration &&
                        skipRegistration.skipRegistration == true) {
                        continue;
                    }
                    
                    if (listItem is string || listItem is bool || listItem is float) {
                        continue;
                    }
                    
                    if (listItem is ScriptableObject scriptableObject) {
                        RegisterScriptableObjectListItem(scriptableObject, rootComponent, relativePropertyPath);
                    }
                    else {
                        TraverseObjectFields(listItem, rootComponent, relativePropertyPath);
                    }
                }
            }
        }

        private void RegisterScriptableObjectListItem(ScriptableObject target, Component rootComponent, string serializedPropertyPath)
        {
            foreach (ScriptableObject scriptableObject in scriptableObjectList) {
                
                if (target == scriptableObject) {
                    
                    RegisterAssetReference(target, rootComponent, serializedPropertyPath);
                    RegisterSceneDependency(target, rootComponent, serializedPropertyPath);
                    
                    break;
                }
            }
        }

        void RegisterAssetReference(ScriptableObject scriptableObject, Component rootComponent, string serializedPropertyPath)
        {
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return;
            }
            
            string filePath = GetAssetRegistryFilePath(scriptableObject);
            JSONNode jsonData = GetRootJsonNode(filePath);
            
            GameObject referencingObject = rootComponent.gameObject;
            
            jsonData[$"Scene: {referencingObject.scene.name}"]
                [$"Object: {referencingObject.name} > {rootComponent.GetType().Name}"]
                .Add($"{SanitizePropertyPath(serializedPropertyPath)}");
            
            WriteData(filePath, jsonData);
        }

        private static void RegisterSceneDependency(ScriptableObject scriptableObject, Component rootComponent, string serializedPropertyPath)
        {
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return;
            }
            
            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);
            JSONNode jsonData = GetRootJsonNode(filePath);
            jsonData[$"Component: {rootComponent.GetType().Name}"].Add($"{SanitizePropertyPath(serializedPropertyPath)} > {scriptableObject.name}");
            
            WriteData(filePath, jsonData);
        }

        void RegisterComponentConditionResponseDesc(object conditionResponseDescObject, Component rootComponent, string sceneName)
        {
            string conditionDescription = conditionResponseDescObject as string;

            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);
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
            string dependenciesFolderPath = Utils.projectPath + "z_Dependencies";

            if(AssetDatabase.IsValidFolder(dependenciesFolderPath) == true) {
                FileUtil.DeleteFileOrDirectory(dependenciesFolderPath);
            }
            
            AssetDatabase.Refresh();
        }

        private static string GetSceneRegistryFilePath(GameObject gameObject)
        {
            string registryPath = Utils.GetProjectDirectory(new string[] { "z_Dependencies", "Scenes", gameObject.scene.name });
            return Utils.GetFilePath(registryPath, gameObject.name, ".json");
        }

        private static string GetAssetRegistryFilePath(ScriptableObject asset)
        {
            string[] directoryPath = new string[2];
            directoryPath[0] = "z_Dependencies";

            directoryPath[1] = asset.GetType().Name;

            string registryPath = Utils.GetProjectDirectory(directoryPath);
            return Utils.GetFilePath(registryPath, asset.name, ".json");
        }

        private static JSONNode GetRootJsonNode(string filePath)
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

        private static void WriteData(string filePath, JSONNode jsonData)
        {
            File.WriteAllText(filePath, jsonData.ToString(2));
        }

    }
}
