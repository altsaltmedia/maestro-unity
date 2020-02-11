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
using QuickEngine.Extensions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    public class RegisterDependencies : EditorWindow
    {
        private static List<ScriptableObject> masterScriptableObjectList = new List<ScriptableObject>();
        
        string targetScenePath = "";
        string targetSceneName = "";
        string callbackScenePath = "";
        string callbackSceneName = "";
        
        private static IntVariable counter;
        
        [SerializeField]
        private static IntVariable maxValue;

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
            
            if (GUILayout.Button(new GUIContent("Find Counter"))) {
                FindCounter();
            }
        }

        void FindCounter()
        {
            if (counter == null) {
                counter = Utils.GetScriptableObject("RegisterDependenciesCounter") as IntVariable;
                maxValue = Utils.GetScriptableObject("MaxValue") as IntVariable;
            }
            
            EditorUtility.ClearProgressBar();
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

        private static List<ScriptableObject> PopulateScriptableObjectList()
        {
            string scriptableObjectQuery = typeof(RegisterableScriptableObject).Name;
            string[] scriptableObjectGuids = AssetDatabase.FindAssets(string.Format("t:{0}", scriptableObjectQuery));

            List<ScriptableObject> registerableScriptableObjects = new List<ScriptableObject>();
            
            int searchCounter = 0;

            foreach (string scriptableObjectGuid in scriptableObjectGuids) {
                EditorUtility.DisplayProgressBar("Scanning project", "Populating scriptable object list", 1.0f / scriptableObjectGuids.Length * searchCounter);
                string assetPath = AssetDatabase.GUIDToAssetPath(scriptableObjectGuid);
                ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));
                registerableScriptableObjects.Add(scriptableObject);

                searchCounter++;
            }
            EditorUtility.ClearProgressBar();

            return registerableScriptableObjects;
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

                masterScriptableObjectList = PopulateScriptableObjectList();
                Scene scene = EditorSceneManager.OpenScene(targetScenePath);
                FindDependencies();
                AssetDatabase.Refresh();
                masterScriptableObjectList.Clear();
            }
        }

        private void TriggerRegisterAllDependencies()
        {
            if (EditorUtility.DisplayDialog("Register dependencies in all scenes?", "This will scan the entire project and recreate the dependencies folder. This cannot be undone.", "Proceed", "Cancel")) {

                RemoveDependenciesFolder();
                masterScriptableObjectList = PopulateScriptableObjectList();
                string[] sceneGuids = GetSceneGuids();
                for (int i = 0; i < sceneGuids.Length; i++) {

                    Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));
                    EditorUtility.DisplayProgressBar("Scanning project", "Finding dependencies in " + scene.name, 1.0f / sceneGuids.Length * i);

                    FindDependencies();
                }
                AssetDatabase.Refresh();
                LoadCallbackScene();
                EditorUtility.ClearProgressBar();
                masterScriptableObjectList.Clear();
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
            var playableDirectors = Resources.FindObjectsOfTypeAll(typeof(PlayableDirector)) as Component[];
            if (playableDirectors == null) return;
            
            playableDirectors = Utils.SortComponentSelection(playableDirectors);

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

            components = Utils.SortComponentSelection(components);

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

                    if (component is IRegisterActionTriggerBehaviour actionTriggerBehaviour) {
                        actionTriggerBehaviour.SyncTriggerDescriptions();
                    }

                    FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    List<Tuple<string, JSONNode>> sceneRegistration = TraverseObjectFields(component, component);
                    foreach (var registration in sceneRegistration) {
                        if(registration == null) continue;
                        var ( filePath, jsonData ) = registration;
                        WriteData(filePath, jsonData);
                    }
                }
            }
        }
        private static List<Tuple<string, JSONNode>> TraverseObjectFields(object source, Component rootComponent, string serializedPropertyPath = "")
        {
            List<Tuple<string, JSONNode>> fieldRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fieldInfoList = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (counter.value > maxValue.value) {
                return null;
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
                    
                    string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, fieldInfo);

                    fieldRegistration.AddRange(ParseUnityEvent(fieldValue as UnityEventBase, rootComponent, relativePropertyPath));
                }
                
                else if (fieldType.IsSubclassOf(typeof(ReferenceBase))) {
                
                    string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, fieldInfo);
                    
                    fieldRegistration.AddRange(ParseReference(fieldValue as ReferenceBase, rootComponent, relativePropertyPath, out var scriptableObjectList));
                    fieldRegistration.AddRange(GetScriptableObjectListRegistration(rootComponent, relativePropertyPath, scriptableObjectList));
                }

                else if (fieldType.GetInterfaces().Contains(typeof(IEnumerable)) && fieldType.IsGenericType) {
                    
                    string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, fieldInfo);
                
                    fieldRegistration.AddRange(ParseFieldList(fieldValue as IEnumerable, fieldInfo, rootComponent, relativePropertyPath));
                }

                else if(fieldType.GetInterfaces().Contains(typeof(IRegisterActionTrigger))) {
                    
                    string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, fieldInfo);
                    
                    fieldRegistration.AddRange(TraverseObjectFields(fieldValue, rootComponent, relativePropertyPath));
                }
            }

            return fieldRegistration;
        }
        
        private static List<Tuple<string, JSONNode>> ParseUnityEvent(UnityEventBase unityEvent, Component rootComponent, string serializedPropertyPath)
        {
            var eventRegistration = new List<Tuple<string, JSONNode>>();
            
            if (unityEvent.GetPersistentEventCount() < 1) {
                return eventRegistration;
            }
            
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return eventRegistration;
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
                    eventRegistration.Add(GetUnityEventReferenceRegistration(target, rootComponent, serializedPropertyPath, eventItem));
                    eventItem["target"] = unityEventData[i].targetName;
                }
                
                if (unityEventData[i].parameter.value is ScriptableObject parameter) {
                    eventItem["parameter"] = eventItem["parameter"] + " (this)";
                    eventRegistration.Add(GetUnityEventReferenceRegistration(parameter, rootComponent, serializedPropertyPath, eventItem));
                    eventItem["parameter"] = unityEventData[i].parameter.valueName;
                }
                
                eventData.Add(eventItem);
            }

            eventRegistration.Add(GetUnityEventSceneRegistration(eventData, rootComponent, serializedPropertyPath));

            return eventRegistration;
        }
        
        private static Tuple<string, JSONNode> GetUnityEventReferenceRegistration(ScriptableObject target, Component rootComponent, string serializedPropertypath, JSONNode eventItem)
        {
            string filePath = GetAssetRegistryFilePath(target);
            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[$"Scene: {rootComponent.gameObject.scene.name}"]
                [$"Object: {rootComponent.gameObject.name} > {rootComponent.GetType().Name} > {SanitizePropertyPath(serializedPropertypath)}"].Add(eventItem);

            return new Tuple<string, JSONNode>(filePath, jsonData);
            //WriteData(filePath, jsonData);
        }

        private static Tuple<string, JSONNode> GetUnityEventSceneRegistration(JSONNode eventData, Component rootComponent, string serializedPropertyPath)
        {
            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);
            JSONNode jsonData = GetRootJsonNode(filePath);

            jsonData[$"Component: {rootComponent.GetType().Name} > {SanitizePropertyPath(serializedPropertyPath)}"]["Unity Event"].Add(eventData);

            return new Tuple<string, JSONNode>(filePath, jsonData);
            //WriteData(filePath, jsonData);
        }

        private static List<Tuple<string, JSONNode>> ParseActionData(IRegisterActionData actionData, Component rootComponent,
            string serializedPropertyPath)
        {
            var actionDataRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fields = actionData.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            JSONNode descriptionNode = new JSONString(actionData.actionDescription);
            
            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(actionData);
                string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, field);

                if (fieldValue is ReferenceBase referenceBase) {
                    actionDataRegistration.AddRange(ParseReference(referenceBase, rootComponent, relativePropertyPath,
                        out var scriptableObjecList));

                    actionDataRegistration.AddRange(GetScriptableObjectListRegistration(rootComponent, serializedPropertyPath, scriptableObjecList, descriptionNode));
                } 
            }
            
            // Some action data contains nested fields that also need to be parsed
            if (actionData is IRegisterNestedActionData) {
                actionDataRegistration.AddRange(TraverseObjectFields(actionData, rootComponent, serializedPropertyPath));
            }

            return actionDataRegistration;
        }

        private static List<Tuple<string, JSONNode>> ParseReference(ReferenceBase reference, Component rootComponent, string serializedPropertyPath, out List<ScriptableObject> objectsAtReferenceRoot)
        {
            var referenceRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fields = reference.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            objectsAtReferenceRoot = new List<ScriptableObject>();
            
            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(reference);
                Type fieldType = field.FieldType;

                if (fieldValue != null && fieldValue is string == false) {
                    
                    if (fieldType.GetInterfaces().Contains(typeof(IEnumerable))) {
                        
                        referenceRegistration.AddRange(ParseFieldList(fieldValue as IEnumerable, field, rootComponent, serializedPropertyPath));
                        
                    } else if (fieldValue is ScriptableObject scriptableObject) {
                        
                        objectsAtReferenceRoot.Add(scriptableObject);
                    }
                }
            }

            return referenceRegistration;
        }
        
        // If the field is a list, register any scriptable objects in the list. Otherwise, recurse and drill
        // further into the list to see if it contains one of our types flagged above
        private static List<Tuple<string, JSONNode>> ParseFieldList(IEnumerable list, FieldInfo fieldInfo, Component rootComponent, string serializedPropertyPath)
        {
            var listRegistration = new List<Tuple<string, JSONNode>>();
            
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
                        listRegistration.AddRange(GetScriptableObjectRegistration(scriptableObject, rootComponent, relativePropertyPath));
                    }
                    else if (listItem is ReferenceBase referenceBase) {
                        listRegistration.AddRange(ParseReference(referenceBase, rootComponent, relativePropertyPath, out var scriptableObjectList));
                        listRegistration.AddRange(GetScriptableObjectListRegistration(rootComponent, relativePropertyPath, scriptableObjectList));
                    }
                    // Note: When it comes to parsing ActionData, which is housed underneath ActionTrigger,
                    // we only want the serialized data, not the editor-only generic, abstract ActionData list that is
                    // used only in the inspector (see the ActionTrigger class for details).
                    else if (listItem is IRegisterConditionResponseActionData conditionResponseData
                             && list.GetType().GetGenericArguments()[0].IsAbstract == false) {
                        listRegistration.AddRange(ParseConditionResponseActionData(conditionResponseData, rootComponent, relativePropertyPath));
                    }
                    else if (listItem is IRegisterActionData actionData && list.GetType().GetGenericArguments()[0].IsAbstract == false) {
                        listRegistration.AddRange(ParseActionData(actionData, rootComponent, relativePropertyPath));   
                    }
                    else {
                        listRegistration.AddRange(TraverseObjectFields(listItem, rootComponent, relativePropertyPath));
                    }
                }
            }

            return listRegistration;
        }
        
        private static List<Tuple<string, JSONNode>> ParseConditionResponseActionData(IRegisterConditionResponseActionData conditionResponseData, Component rootComponent,
            string serializedPropertyPath)
        {
            bool isComplexConditionResponse =
                string.IsNullOrEmpty(conditionResponseData.genericActionDescription) == false;

            if (isComplexConditionResponse == false) {
                return ParseStandardConditionResponse(conditionResponseData, rootComponent, serializedPropertyPath);
            }

            return ParseComplexConditionResponse(conditionResponseData, rootComponent, serializedPropertyPath);
        }

        private static List<Tuple<string, JSONNode>> ParseStandardConditionResponse(IRegisterConditionResponseActionData conditionResponseData,
            Component rootComponent, string serializedPropertyPath)
        {
            var conditionResponseDataRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fields = conditionResponseData.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        
            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(conditionResponseData);
                string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, field);

                if (fieldValue is IEnumerable listItems) {
                    int counter = 0;
                    foreach (var item in listItems) {
                        if (item is IRegisterConditionResponse conditionResponse) {
                            string listPropertyPath = $"{relativePropertyPath}.Array.data[{counter}]";
                            conditionResponseDataRegistration.AddRange(
                                    GetFullConditionResponseRegistration(conditionResponse, rootComponent, listPropertyPath));
                            counter++;
                        }
                    }
                }
            }
            
            return conditionResponseDataRegistration;
        }
        
        private static List<Tuple<string, JSONNode>> ParseComplexConditionResponse(IRegisterConditionResponseActionData conditionResponseData,
            Component rootComponent, string serializedPropertyPath)
        {
            var conditionResponseDataRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fields = conditionResponseData.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);
            JSONNode jsonData = GetRootJsonNode(filePath);
            string componentName = rootComponent.GetType().Name;

            foreach (FieldInfo field in fields) {
                
                var fieldValue = field.GetValue(conditionResponseData);
                string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, field);

                if (fieldValue is IEnumerable listItems) {
                    int counter = 0;
                    foreach (var item in listItems) {
                        if (item is IRegisterConditionResponse conditionResponse) {
                            string listPropertyPath = $"{relativePropertyPath}.Array.data[{counter}]";
                            var conditionRegistration =
                                GetPartialConditionResponseRegistration(conditionResponse, rootComponent, listPropertyPath);
                            conditionResponseDataRegistration.Add(conditionRegistration);
                            jsonData[$"Component: {componentName}"]
                                [SanitizePropertyPath(serializedPropertyPath)].Add(conditionResponse.conditionEventTitle);
                            // foreach (var condition in conditionRegistration) {
                            //     var ( writePath, data ) = condition;
                            //     filePath = string.IsNullOrEmpty(filePath) ? writePath : filePath;
                            //     jsonData[$"Scene: {rootComponent.gameObject.scene.name}"].Add(data);
                            // }
                            counter++;
                        }
                    }
                }
                else if (fieldValue is UnityEventBase unityEvent) {
                    var eventRegistrations= ParseUnityEvent(unityEvent, rootComponent,
                        relativePropertyPath);
                    foreach (var action in eventRegistrations) {
                        var ( writePath, data ) = action;
                        filePath = string.IsNullOrEmpty(filePath) ? writePath : filePath;
                        JSONNode sanitizedNode = data[ $"Component: {componentName} > {SanitizePropertyPath(relativePropertyPath)}"];
                        jsonData[$"Component: {componentName}"]
                            [SanitizePropertyPath(serializedPropertyPath)].Add(sanitizedNode);
                    }
                }
            }

            conditionResponseDataRegistration.Add(new Tuple<string, JSONNode>(filePath, jsonData));
            return conditionResponseDataRegistration;
        }
        
        private static List<Tuple<string, JSONNode>> GetFullConditionResponseRegistration(IRegisterConditionResponse conditionResponse, Component rootComponent,
            string serializedPropertyPath)
        {
            var conditionResponseRegistration = new List<Tuple<string, JSONNode>>();
            
            FieldInfo[] fields = conditionResponse.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            JSONNode descriptionNode = new JSONArray();
            descriptionNode.Add(conditionResponse.conditionEventTitle);

            JSONNode eventDescription = new JSONArray();
            string[] rawDescription = conditionResponse.eventDescription.Split('\n');
            for (int i = 0; i < rawDescription.Length; i++) {
                eventDescription.Add(rawDescription[i]);
            }
            
            descriptionNode.Add(eventDescription);

            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(conditionResponse);
                string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, field);

                if (fieldValue is ReferenceBase referenceBase) {
                    
                    conditionResponseRegistration.AddRange(ParseReference(referenceBase, rootComponent, relativePropertyPath,
                        out var scriptableObjecList));
                    
                    conditionResponseRegistration.AddRange(
                        GetScriptableObjectListRegistration(rootComponent, serializedPropertyPath, scriptableObjecList, descriptionNode));
                    
                } else if (fieldValue is UnityEventBase unityEvent) {
                    conditionResponseRegistration.AddRange(ParseUnityEvent(unityEvent, rootComponent, relativePropertyPath));
                }
            }

            return conditionResponseRegistration;
        }
        
        private static Tuple<string, JSONNode> GetPartialConditionResponseRegistration(IRegisterConditionResponse conditionResponse, Component rootComponent,
            string serializedPropertyPath)
        {
            Tuple<string, JSONNode> conditionResponseRegistration = null;
            
            FieldInfo[] fields = conditionResponse.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            JSONNode descriptionNode = new JSONArray();
            descriptionNode.Add(conditionResponse.conditionEventTitle);

            foreach (FieldInfo field in fields) {

                var fieldValue = field.GetValue(conditionResponse);
                string relativePropertyPath = GetRelativePropertyPath(serializedPropertyPath, field);

                if (fieldValue is ReferenceBase referenceBase) {

                    ParseReference(referenceBase, rootComponent, relativePropertyPath, out var scriptableObjectList);

                    foreach (var scriptableObject in scriptableObjectList) {

                        // When registering the asset, if the action is referencing
                        // the asset itself, let's add a flag to make that explicit
                        if (descriptionNode.Value.Contains(scriptableObject.name)) {
                            JSONNode descriptionOverride =
                                descriptionNode.Value.Replace(scriptableObject.name, $"{scriptableObject.name} (this)");
                            conditionResponseRegistration = GetAssetRegistration(scriptableObject, rootComponent,
                                serializedPropertyPath, descriptionOverride);
                        }
                        else {
                            conditionResponseRegistration = GetAssetRegistration(scriptableObject, rootComponent,
                                serializedPropertyPath, descriptionNode);
                        }
                    }
                }
            }

            return conditionResponseRegistration;
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

        private static List<Tuple<string, JSONNode>> GetScriptableObjectListRegistration(Component rootComponent, string relativePropertyPath,
            List<ScriptableObject> scriptableObjectList)
        {
            var scriptableObjectRegistration = new List<Tuple<string, JSONNode>>();
            
            foreach (var scriptableObject in scriptableObjectList) {
                scriptableObjectRegistration.Add(GetAssetRegistration(scriptableObject, rootComponent, relativePropertyPath));
                scriptableObjectRegistration.Add(GetSceneRegistration(scriptableObject, rootComponent, relativePropertyPath));
            }

            return scriptableObjectRegistration;
        }
        
        private static List<Tuple<string, JSONNode>> GetScriptableObjectListRegistration(Component rootComponent,
            string serializedPropertyPath, List<ScriptableObject> scriptableObjectList, JSONNode descriptionNode)
        {
            var scriptableObjectRegistration = new List<Tuple<string, JSONNode>>();
            
            foreach (var scriptableObject in scriptableObjectList) {

                // When registering the asset, if the action is referencing
                // the asset itself, let's add a flag to make that explicit
                if (descriptionNode.Value.Contains(scriptableObject.name)) {
                    JSONNode descriptionOverride = 
                        descriptionNode.Value.Replace(scriptableObject.name, $"{scriptableObject.name} (this)");
                    scriptableObjectRegistration.Add(GetAssetRegistration(scriptableObject, rootComponent, serializedPropertyPath, descriptionOverride));
                }
                else {
                    scriptableObjectRegistration.Add(GetAssetRegistration(scriptableObject, rootComponent, serializedPropertyPath, descriptionNode));
                }
                        
                scriptableObjectRegistration.Add(GetSceneRegistration(scriptableObject, rootComponent, serializedPropertyPath, descriptionNode));
            }

            return scriptableObjectRegistration;
        }


        private static List<Tuple<string, JSONNode>> GetScriptableObjectRegistration(ScriptableObject target, Component rootComponent, string serializedPropertyPath)
        {
            var scriptableObjectRegistration = new List<Tuple<string, JSONNode>>
            {
                GetAssetRegistration(target, rootComponent, serializedPropertyPath),
                GetSceneRegistration(target, rootComponent, serializedPropertyPath)
            };
            
            return scriptableObjectRegistration;
        }

        private static Tuple<string, JSONNode> GetAssetRegistration(ScriptableObject scriptableObject, Component rootComponent, string serializedPropertyPath, JSONNode registrationData = null)
        {
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return null;
            }
            
            string filePath = GetAssetRegistryFilePath(scriptableObject);
            JSONNode jsonData = GetRootJsonNode(filePath);
            
            GameObject referencingObject = rootComponent.gameObject;

            string assetString = SanitizePropertyPath(serializedPropertyPath);

            if (assetString.Contains(scriptableObject.name)) {
                assetString = assetString.
                    Replace(scriptableObject.name, $"{scriptableObject.name} (this)");
            }
            else {
                assetString = $"{assetString} > {scriptableObject.name} (this)";
            }

            jsonData[$"Scene: {referencingObject.scene.name}"]
                [$"Object: {referencingObject.name} > {rootComponent.GetType().Name}"]
                .Add(assetString);
            
            if (registrationData != null) {
                jsonData[$"Scene: {referencingObject.scene.name}"]
                    [$"Object: {referencingObject.name} > {rootComponent.GetType().Name}"]
                    .Add(registrationData);
            }
            
            return new Tuple<string, JSONNode>(filePath, jsonData);
            
            //return WriteData(filePath, jsonData);
        }

        private static Tuple<string, JSONNode> GetSceneRegistration(ScriptableObject scriptableObject, Component rootComponent, string serializedPropertyPath, JSONNode registrationData = null)
        {
            var serializedObject = new SerializedObject(rootComponent);
            var serializedProperty = serializedObject.FindProperty(serializedPropertyPath);

            if (serializedProperty == null) {
                return null;
            }
            
            string filePath = GetSceneRegistryFilePath(rootComponent.gameObject);
            JSONNode jsonData = GetRootJsonNode(filePath);
            jsonData[$"Component: {rootComponent.GetType().Name}"]
                .Add($"{SanitizePropertyPath(serializedPropertyPath)} > {scriptableObject.name}");

            if (registrationData != null) {
                jsonData[$"Component: {rootComponent.GetType().Name}"]
                    .Add(registrationData);
            }
            
            return new Tuple<string, JSONNode>(filePath, jsonData);
            
            //return WriteData(filePath, jsonData);
        }

        private static void ClearHiddenValues(string scenePath)
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

        private void ClearHiddenValuesInTargetScene()
        {
            if (EditorUtility.DisplayDialog("Clear hidden values in " + targetSceneName+ "?", "This will search " + targetSceneName + " for hidden values " +
                "on relevant components and erase them. This action cannot be undone.", "Proceed", "Cancel")) {

                ClearHiddenValues(targetScenePath);

            }
        }

        private void ClearAllHiddenValues()
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

        private void LoadCallbackScene()
        {
            EditorSceneManager.OpenScene(callbackScenePath);
        }

        private static void RemoveDependenciesFolder()
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
        
        private static string GetRelativePropertyPath(string serializedPropertyPath, FieldInfo fieldInfo)
        {
            string relativePropertyPath;
            
            if (string.IsNullOrEmpty(serializedPropertyPath) == true) {
                relativePropertyPath = fieldInfo.Name;
            }
            else {
                relativePropertyPath = $"{serializedPropertyPath}.{fieldInfo.Name}";
            }

            return relativePropertyPath;
        }

        private static string WriteData(string filePath, JSONNode jsonData)
        {
            string data = jsonData.ToString(2);
            File.WriteAllText(filePath, data);
            return data;
        }

    }
}
