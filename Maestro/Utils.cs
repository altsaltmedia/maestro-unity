﻿/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine.Experimental.XR;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace AltSalt.Maestro
{
	public enum FadeType { FadeIn, FadeOut }

	public enum DimensionType { Vertical, Horizontal }

    public enum AspectRatioType { x16x9, x9x16, x4x3, x3x4, Dynamic }

    public enum RatioType { Numerator, Denominator }

    public enum MaterialAttributeType { Color, Float }

    public enum AxisType { X, Y }
    
    public enum SwipeDirection { yPositive, yNegative, xPositive, xNegative }

    public enum AxisDestination { fromAxis, toAxis }

    public enum DataType { stringType, floatType, boolType, intType, scriptableObjectType, systemObjectType }

    public enum ComparisonValues { GreaterThan, LessThan, EqualTo }
    
    public enum VarDependencies {
        AppSettings,
        ModifySettings,
        DeviceAspectRatio,
        DeviceWidth,
        DeviceHeight,
        SceneAspectRatio,
        SceneWidth,
        SceneHeight,
        TimelineCurrentTime,
        FrameStepValue,
        SwipeModifierOutput,
        OnGraphStart,
        ResponsiveElementEnable,
        ResponsiveElementDisable,
        TextUpdate,
        LayoutUpdate,
        ScreenResized,
        IsReversing,
        ySwipeAxis,
        xSwipeAxis,
        yMomentumAxis,
        xMomentumAxis,
        ScrubberActive,
        yNorthBranch,
        ySouthBranch,
        xEastBranch,
        xWestBranch
    }

    public enum ScriptNames
    {
        CustomKey,
        BranchKey
    }

    public enum XMLValues
    {
        container,
        textObject
    }

    public static class Utils
    {

        public static Color transparent = new Color(1, 1, 1, 0);
        public static float pageHeight = 6.3f;

        public static string ComplexEventString = typeof(ComplexEvent).Name;
        public static string SimpleEventString = typeof(SimpleEvent).Name;
        static GUISkin altSaltSkin;

        // Custom equation of an exponential function - equation is in the form y = a^x * b
        // It is derived by taking two (X,Y) coordinates along the line, creating two equations
        // in the form above, then dividing one equation by the other to solve for a and b.
        // Values are converted to a double here to preserve precision.
        public static double GetResponsiveWidth(float height, float width)
        {
            double aspectRatio = height / width;
            return Math.Pow(0.561993755433366d, aspectRatio) * 10.03014554127636d;
        }
        
        public static string LimitLength(this string source, int maxLength)
        {
            if (source.Length <= maxLength)
            {
                return source;
            }

            return source.Substring(0, maxLength);
        }
        
        public static bool ContainsActiveModifyConfig(List<IModifyConfig> modifyConfigs, out IModifyConfig modifyConfig)
        {
            List<IModifyConfig> activeModifyConfigs = new List<IModifyConfig>();
            for (int i = 0; i < modifyConfigs.Count; i++) {
                if (modifyConfigs[i].active == true) {
                    activeModifyConfigs.Add(modifyConfigs[i]);
                }
            }

            if (activeModifyConfigs.Count > 0) {
                // Set to the one with highest priority
                activeModifyConfigs.Sort(new Utils.ModifyConfigSort());
                modifyConfig = activeModifyConfigs[activeModifyConfigs.Count - 1];
                return true;
            }

            modifyConfig = null;
            return false;
        }
        
        public class ModifyConfigSort : Comparer<IModifyConfig>
        {
            public override int Compare(IModifyConfig x, IModifyConfig y)
            {
                return x.priority.CompareTo(y.priority);
            }
        }

#if UNITY_EDITOR
        public static GUISkin AltSaltSkin {

            get {
                if(altSaltSkin == null) {
                    altSaltSkin = Resources.Load("AltSaltEditorSkin") as GUISkin;
                }
                return altSaltSkin;
            }
        }

        public static string GetStylesheetPath()
        {
            return "Assets/_AltSalt/Maestro/Editor/EditorStyles.uss";
        }
        
        public static GameObject[] GetAllGameObjects()
        {
            return GetChildGameObjects(GetRootGameObjects());
        }
        
        public static GameObject[] GetRootGameObjects()
        {
            GameObject[] rootGameObjects =  SceneManager.GetActiveScene().GetRootGameObjects();
            Array.Sort(rootGameObjects, new GameObjectSort());
            return rootGameObjects;
        }

        public static GameObject[] GetParentGameObjects(GameObject selection, bool includeRoot = false)
        {
            GameObject[] gameObjectArray = { selection };
            return GetParentGameObjects(gameObjectArray, includeRoot);
        }
        
        public static GameObject[] GetParentGameObjects(GameObject[] selection, bool includeRoot = false)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++) {
                gameObjectList = TraverseTransformParents(gameObjectList, selection[i].transform, selection[i].transform, null, includeRoot);
            }
            return gameObjectList.ToArray();
        }

        public static GameObject[] GetChildGameObjects(GameObject selection, bool includeRoot = false)
        {
            GameObject[] gameObjectArray = { selection };
            return GetChildGameObjects(gameObjectArray, includeRoot);
        }

        public static GameObject[] GetChildGameObjects(GameObject[] selection, bool includeRoot = false)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++) {
                gameObjectList = TraverseTransformChildrenHierarchy(gameObjectList, selection[i].transform, selection[i].transform, null, includeRoot);
            }
            return gameObjectList.ToArray();
        }

        public delegate void TraverseTransformDelegate(List<GameObject> gameObjectList, Transform transform);
        
        public static List<GameObject> TraverseTransformParents(List<GameObject> targetList, Transform rootChildTransform, Transform currentNode, TraverseTransformDelegate traverseTransformDelegate = null, bool includeRoot = false)
        {
            if(includeRoot == true) {
                if(targetList.Contains(rootChildTransform.gameObject) == false) {
                    if (traverseTransformDelegate != null) {
                        traverseTransformDelegate.Invoke(targetList, rootChildTransform);
                    } else {
                        targetList.Add(rootChildTransform.gameObject);
                    }
                }
            }

            if (currentNode.transform.parent == null) {
                return targetList;
            } else {
                
                Transform parentTransform = currentNode.parent;
                if (traverseTransformDelegate != null) {
                    traverseTransformDelegate.Invoke(targetList, parentTransform);
                }
                else {
                    targetList.Add(parentTransform.gameObject);
                }
                TraverseTransformParents(targetList, rootChildTransform, parentTransform, traverseTransformDelegate);
            
                return targetList;
            }
        }

        // Given an empty list of game objects and a root transform, will recursively go through the root
        // transform's children and populate the list with the children in the order they appear in the hierarchy.
        // Optionally, can take a delegate to perform custom handling on each child transform as we come across it.
        public static List<GameObject> TraverseTransformChildrenHierarchy(List<GameObject> targetList, Transform rootTransform, Transform currentNode, TraverseTransformDelegate traverseTransformDelegate = null, bool includeRoot = false)
        {
            if(includeRoot == true) {
                if(targetList.Contains(rootTransform.gameObject) == false) {
                    if (traverseTransformDelegate != null) {
                        traverseTransformDelegate.Invoke(targetList, rootTransform);
                    } else {
                        targetList.Add(rootTransform.gameObject);
                    }
                }
            }

            if (currentNode.childCount == 0) {
                return targetList;
            } else {
                for (int i = 0; i < currentNode.childCount; i++) {
                    Transform childTransform = currentNode.GetChild(i);
                    if (traverseTransformDelegate != null) {
                        traverseTransformDelegate.Invoke(targetList, childTransform);
                    }
                    else {
                        targetList.Add(childTransform.gameObject);
                    }
                    TraverseTransformChildrenHierarchy(targetList, rootTransform, childTransform, traverseTransformDelegate);
                }
                return targetList;
            }
        }
        
        public static GameObject[] SortGameObjectSelection(GameObject[] gameObjects)
        {
            // Collect all selected game objects into a dictionary based on parent...
            Dictionary<Transform, List<GameObject>> selectionByParent = new Dictionary<Transform, List<GameObject>>();
            // (or, if it's a root and has no parent, save into a separate dictionary)
            Dictionary<Transform, GameObject> selectionRootObjects = new Dictionary<Transform, GameObject>();

            for (int i=0; i<gameObjects.Length; i++) {
                Transform parent = gameObjects[i].transform.parent;

                if(parent != null) {
                    if(selectionByParent.ContainsKey(parent)) {
                        selectionByParent[parent].Add(gameObjects[i]);
                    } else {
                        selectionByParent.Add(parent, new List<GameObject>());
                        selectionByParent[parent].Add(gameObjects[i]);
                    }
                } else {
                    selectionRootObjects.Add(gameObjects[i].transform, gameObjects[i]);
                }
            }

            // Sort the game object lists underneath each node based on sibling index
            foreach (KeyValuePair<Transform, List<GameObject>> childObjectList in selectionByParent) {
                childObjectList.Value.Sort(new GameObjectSort());
            }

            // Add any root objects into our lists, inserting at the beginning if the list
            // already exists, else creating a new list to house our root
            foreach (KeyValuePair<Transform, GameObject> rootObject in selectionRootObjects) {
                if (selectionByParent.ContainsKey(rootObject.Key)) {
                    selectionByParent[rootObject.Key].Insert(0, rootObject.Value);
                } else {
                    selectionByParent.Add(rootObject.Key, new List<GameObject>());
                    selectionByParent[rootObject.Key].Add(rootObject.Value);
                }
            }

            // The list that we will contain our final, ordered selection
            List<GameObject> finalOrderedSelection = new List<GameObject>();

            // Our scene's root objects, in order
            GameObject[] rootObjects = GetRootGameObjects();

            for(int q=0; q<rootObjects.Length; q++) {
                 
                // Traverse all the game objects in our scene. As we hit each iteration, we build the list by comparing it to
                // our dictionary of parents and child objects. Since this traversal is happening in order, this returns the
                // same contents of our dictionary, just sorted based on each object's appearance in the hierarchy
                List<GameObject> totalHierarchySelection = TraverseTransformChildrenHierarchy(new List<GameObject>(), rootObjects[q].transform, rootObjects[q].transform, (List<GameObject> hierarchySelection, Transform currentTransform) => {
                    foreach (KeyValuePair<Transform, List<GameObject>> parentObject in selectionByParent) {
                        for(int i=0; i<parentObject.Value.Count; i++) {
                            if(currentTransform.gameObject == parentObject.Value[i]) {
                                hierarchySelection.Add(currentTransform.gameObject);
                            }
                        }
                    }
                }, true);

                finalOrderedSelection.AddRange(totalHierarchySelection);
            }

            return finalOrderedSelection.ToArray();
        }

        public class GameObjectSort : Comparer<GameObject>
        {
            public override int Compare(GameObject x, GameObject y)
            {
                return x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex());
            }
        }

        public class TransformSort : Comparer<Transform>
        {
            public override int Compare(Transform x, Transform y)
            {
                return x.GetSiblingIndex().CompareTo(y.GetSiblingIndex());
            }
        }

        public static bool TargetComponentSelected(GameObject currentSelection, Type targetType)
        {
            if (currentSelection.GetComponent(targetType) != null) {
                return true;
            }
            
            return false;
        }

        public static bool TargetTypeSelected(UnityEngine.Object currentSelection, Type targetType)
        {
            Type currentType = currentSelection.GetType();
            if (currentType.IsSubclassOf(targetType) || currentType == targetType) {
                return true;
            }
            
            return false;
        }

        public static bool TargetComponentSelected(GameObject[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                if (currentSelection[i].GetComponent(targetType) != null) {
                    return true;
                }
            }
            return false;
        }

        public static bool TargetTypeSelected(UnityEngine.Object[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                Type currentType = currentSelection[i].GetType();
                if (currentType.IsSubclassOf(targetType) || currentType == targetType) {
                    return true;
                }
            }
            return false;
        }

        public static UnityEngine.Object[] RenameElements(string newName, UnityEngine.Object[] targetObjects)
        {
            Array.Sort(targetObjects, new Utils.GameObjectSort());

            for (int i = 0; i < targetObjects.Length; i++) {
                if (newName.Contains("{x}")) {
                    targetObjects[i].name = newName.Replace("{x}", (i + 1).ToString());
                } else {
                    if (i == 0) {
                        targetObjects[i].name = newName;
                    } else {
                        targetObjects[i].name = string.Format("{0} ({1})", newName, i);
                    }
                }

            }

            return targetObjects;
        }
        public static UnityEngine.Object[] CullSelection(GameObject[] currentSelection, Type typeToSelect)
        {
            List<GameObject> newSelection = new List<GameObject>();

            for (int i = 0; i < currentSelection.Length; i++) {
                Component component = currentSelection[i].GetComponent(typeToSelect);
                if (component != null && (component.GetType().IsSubclassOf(typeToSelect) || component.GetType() == typeToSelect)) {
                    newSelection.Add(currentSelection[i]);
                }
            }

            return newSelection.ToArray();
        }

        public static UnityEngine.Object[] CullSelection(GameObject[] currentSelection, Type[] typeToSelect)
        {
            List<GameObject> newSelection = new List<GameObject>();

            for (int i = 0; i < currentSelection.Length; i++) {
                for (int q = 0; q < typeToSelect.Length; q++) {
                    Component component = currentSelection[i].GetComponent(typeToSelect[q]);
                    if (component != null && (component.GetType().IsSubclassOf(typeToSelect[q]) || component.GetType() == typeToSelect[q])) {
                        newSelection.Add(currentSelection[i]);
                    }
                }
            }

            return newSelection.ToArray();
        }

        public static UnityEngine.Object[] CullSelection(UnityEngine.Object[] currentSelection, Type typeToSelect)
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                Type objectType = currentSelection[i].GetType();
                if (objectType.IsSubclassOf(typeToSelect) || objectType == typeToSelect) {
                    newSelection.Add(currentSelection[i]);
                }
            }

            return newSelection.ToArray();
        }


        public static UnityEngine.Object[] CullSelection(UnityEngine.Object[] currentSelection, Type[] typeToSelect)
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                for (int q = 0; q < typeToSelect.Length; q++) {
                    Type objectType = currentSelection[i].GetType();
                    if (objectType.IsSubclassOf(typeToSelect[q]) || objectType == typeToSelect[q]) {
                        newSelection.Add(currentSelection[i]);
                    }
                }
            }

            return newSelection.ToArray();
        }


        public static UnityEngine.Object[] FilterSelection(UnityEngine.Object[] currentSelection, Type typeToOmit)
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                Type objectType = currentSelection[i].GetType();
                if (objectType.IsSubclassOf(typeToOmit) || objectType == typeToOmit) {
                    continue;
                }
                newSelection.Add(currentSelection[i]);
                
            }

            return newSelection.ToArray();
        }

        public static UnityEngine.Object[] FilterSelection(UnityEngine.Object[] currentSelection, Type[] typeToOmit)
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();

            for(int i=0; i<currentSelection.Length; i++) {
                for(int q=0; q<typeToOmit.Length; q++) {
                    Type objectType = currentSelection[i].GetType();
                    if (objectType.IsSubclassOf(typeToOmit[q]) || objectType == typeToOmit[q]) {
                        continue;
                    }
                    newSelection.Add(currentSelection[i]);
                }
            }

            return newSelection.ToArray();
        }

        public static GameObject DuplicateObject(GameObject sourceObject)
        {
            GameObject duplicate;

            GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(sourceObject) as GameObject;
            if (prefabRoot != null) {
                duplicate = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                PrefabUtility.SetPropertyModifications(duplicate, PrefabUtility.GetPropertyModifications(sourceObject));
                List<AddedComponent> addedComponents = PrefabUtility.GetAddedComponents(sourceObject);
                for (int i = 0; i < addedComponents.Count; i++) {
                    Component duplicateComponent = duplicate.AddComponent(addedComponents[i].instanceComponent.GetType());
                    EditorUtility.CopySerialized(addedComponents[i].instanceComponent, duplicateComponent);
                    if(duplicateComponent is SerializableElement) {
                        (duplicateComponent as SerializableElement).Reinitialize();
                    }
                }
            } else {
                duplicate = UnityEngine.Object.Instantiate(sourceObject);
            }

            duplicate.transform.position = sourceObject.transform.position;
            duplicate.transform.localScale = sourceObject.transform.localScale;
            duplicate.name = sourceObject.name;

            return duplicate;
        }

        public class ClipTimeSort : Comparer<TimelineClip>
        {
            public override int Compare(TimelineClip x, TimelineClip y)
            {
                return x.start.CompareTo(y.start);
            }
        }

        public static string GetProjectPath()
        {
            return Application.dataPath + "/__Project";
        }

        public static string GetDirectory(string[] subfolders)
        {
            string directoryPath = GetProjectPath();

            for (int i = 0; i < subfolders.Length; i++) {
                directoryPath += subfolders[i];
                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            return directoryPath;
        }

        public static string GetFilePath(string basePath, string fileName, string extension = "")
        {
            StringBuilder path = new StringBuilder(basePath);
            string[] pathComponents = { "/", fileName, extension };
            for (int i = 0; i < pathComponents.Length; i++) {
                path.Append(pathComponents[i]);
            }
            return path.ToString();
        }

        public static string GetAssetPathFromObject(UnityEngine.Object targetObject)
        {
            string targetGUID = GetGUIDFromObject(targetObject);
            return AssetDatabase.GUIDToAssetPath(targetGUID);
        }

        public static string GetGUIDFromObject(UnityEngine.Object targetObject)
        {
            string guid;
            long file;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetObject, out guid, out file);
            return guid;
        }
#endif
        public static TrackAsset GetTrackFromTimelineAsset(TimelineAsset timelineAsset, Type trackType)
        {
            IEnumerable<TrackAsset> trackAssets = timelineAsset.GetOutputTracks();
                    
            foreach (TrackAsset trackAsset in trackAssets)
            {
                if (trackAsset.GetType() == trackType)
                {
                    return trackAsset;
                }
            }

            return null;
        }
        
        public static JSONNode AddToJSONArray(JSONNode node, string key, string value)
        {
            if (key.Length < 1) {
                node.Add(value);

            } else if (node[key] != null) {
                bool dependencyExists = false;
                foreach (string childVal in node[key].Children) {
                    if (childVal == value) {
                        dependencyExists = true;
                    }
                }
                if (dependencyExists == false) {
                    node[key].Add(value);
                }

            } else {
                Debug.LogWarning("Unable to add to JSON array");
            }

            return node;
        }

        public static JSONNode AddToJSONArray(JSONNode node, string key, JSONNode value)
        {
            if (key.Length < 1) {
                node.Add(value);

            } else if (node[key] != null) {
                bool dependencyExists = false;
                foreach (string childVal in node[key].Children) {
                    if (childVal == value) {
                        dependencyExists = true;
                    }
                }
                if (dependencyExists == false) {
                    node[key].Add(value);
                }

            } else {
                Debug.LogWarning("Unable to add to JSON array");
            }

            return node;
        }

        public static JSONNode AddToJSONObject(JSONNode node, string key, JSONNode value)
        {
            if (node[key] != null) {
                bool dependencyExists = false;
                foreach (string childVal in node[key].Children) {
                    if (childVal == value) {
                        dependencyExists = true;
                    }
                }
                if (dependencyExists == false) {
                    node[key].Add(value);
                }

            } else {
                Debug.LogWarning("Unable to add to JSON array");
            }

            return node;
        }

        public static int GetAxisId(string axisName)
        {
            switch (axisName) {
                case "X":
                    return 0;

                case "Y":
                    return 1;

                case "Z":
                    return 2;

                default:
                    return 0;
            }
        }

        public static EventPayload CreateEventPayload()
        {
            EventPayload payloadInstance = ScriptableObject.CreateInstance(typeof(EventPayload)) as EventPayload;
            return payloadInstance;
        }

#if UNITY_EDITOR
        static void LogDuplicateAssetWarning(string assetName)
        {
            Debug.LogWarning("More than one matching asset found for search " + assetName + ". Please check to see if this is intentional.");
        }

        public static AppSettings GetAppSettings()
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets("t:" + nameof(VarDependencies.AppSettings));

            if(guids.Length > 1) {
                Debug.LogWarning("More than one matching App Settings asset found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (AppSettings)AssetDatabase.LoadAssetAtPath(path, typeof(AppSettings));
        }

        public static ModifySettings GetModifySettings()
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets("t:" + nameof(VarDependencies.ModifySettings));

            if(guids.Length == 0) {
                throw new Exception("Modify Settings not found! You must create an instance of Modify Settings.");
            }

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching Modify Settings asset found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ModifySettings)AssetDatabase.LoadAssetAtPath(path, typeof(ModifySettings));
        }

        public static SimpleEvent GetSimpleEvent(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(SimpleEvent).Name;

            guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", target, typeName));

            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (SimpleEvent)AssetDatabase.LoadAssetAtPath(path, typeof(SimpleEvent));
        }

        public static ComplexEvent GetComplexEvent(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(ComplexEvent).Name;

            guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", target, typeName));

            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ComplexEvent)AssetDatabase.LoadAssetAtPath(path, typeof(ComplexEvent));
        }

        public static FloatVariable GetFloatVariable(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(FloatVariable).Name;

            guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", target, typeName));

            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (FloatVariable)AssetDatabase.LoadAssetAtPath(path, typeof(FloatVariable));
        }

        public static BoolVariable GetBoolVariable(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(BoolVariable).Name;

            guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", target, typeName));

            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (BoolVariable)AssetDatabase.LoadAssetAtPath(path, typeof(BoolVariable));
        }

        public static ScriptableObject GetScriptableObject(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(ScriptableObject).Name;

            guids = AssetDatabase.FindAssets(string.Format("{0} t:{1}", target, typeName));

            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));
        }

        public static void RepaintInspector(System.Type t)
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++) {
                if (ed[i].GetType() == t) {
                    ed[i].Repaint();
                    return;
                }
            }
        }

        public static void RepaintInspector()
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++) {
                ed[i].Repaint();
                return;
            }
        }
#endif

        // Given a value and a list of other values to compare against, will check if
        // a value is in between the numbers in the list, returning the first index
        // where it is greater than or equal to the first value, but less than the following
        // value. If less than the smallest value of the list, returns index 0; if greater
        // than every value of the list, returns the list count.
        public static int GetValueIndexInList(float value, List<float> listOfValues)
        {
            int valueIndex = -1;

            // If there's only one value in our comparison list,
            // make a simple comparison to see if our value is
            // less than or greater than it.
            if (listOfValues.Count == 1) {
                if (value < listOfValues[0]) {
                    valueIndex = 0;
                } else {
                    valueIndex = 1;
                }
            }
            else {
                if (value < listOfValues[0]) {
                    valueIndex = 0;
                } else {
                    for (int i = 0; i < listOfValues.Count - 1; i++) {
                        if (value >= listOfValues[i] && value < listOfValues[i + 1]) {
                            valueIndex = i + 1;
                            break;
                        }
                    }
                    if(valueIndex == -1) {
                        valueIndex = listOfValues.Count;
                    }
                }
            }

            if(valueIndex == -1) {
                Debug.LogError("Given list, could not find index for specified value. Is your list populated correctly?");
            }

            return valueIndex;
        }

        // =========== //
        // Expand List //
        // =========== //

        // Used for responsive elements to ensure the list in question is always
        // one greater than the index, that way we can create responsiveness between breakpoints

        public static void ExpandList(List<Rect> list, int index)
        { 
            while (list.Count <= index) {
                list.Add(new Rect());
            }
        }

        public static void ExpandList(List<Vector2> list, int index)
        {
            while(list.Count <= index) {
                list.Add(new Vector2());
            }
        }

        public static void ExpandList(List<Vector3> list, int index)
        {
            while (list.Count <= index) {
                list.Add(new Vector3());
            }
        }

        public static void ExpandList(List<float> list, int index)
        {
            while (list.Count <= index) {
                list.Add(new float());
            }
        }

        public static void ExpandList(List<DimensionType> list, int index)
        {
            while (list.Count <= index) {
                list.Add(new DimensionType());
            }
        }

        public static void ExpandList(List<AspectRatioType> list, int index)
        {
            while (list.Count <= index) {
                list.Add(new AspectRatioType());
            }
        }

        public static float GetValueFromDesiredPercent(float baseNumber, float desiredPercent)
        {
            return baseNumber * desiredPercent;
        }

        public static float NormalizeFloat(float currentValue, float minValue, float maxValue)
        {
            return (currentValue - minValue) / (maxValue - minValue);
        }

        /*
        * @ClampVectorValue
        * 
        * Restricts the sensitivity of swipe information
        * 
        * @param Vector2 swipe Swipe information from EasyTouch plugin
        * @param float sensitivity Sensitivity for either OnSwipe or OnSwipeEnd
        */
        public static Vector2 ClampVectorValue(Vector2 rawVector, float maxMin = 1f)
        {
            Vector2 clampedVector = new Vector2(0, 0);

            clampedVector.x = Mathf.Clamp(rawVector.x, maxMin * -1f, maxMin);
            clampedVector.y = Mathf.Clamp(rawVector.y, maxMin * -1f, maxMin);

            return clampedVector;
        }

        /*
        * @ConvertV2toV3
        * 
        * Sets the velocity depending on which axis is currently active
        * 
        * @param Vector2 swipe The clamped swipe information
        */
        public static Vector3 ConvertV2toV3(Vector2 rawVector)
        {
            Vector3 v3 = new Vector3(0, 0, 0);

            Vector2 v2 = rawVector;

            v3.x = v2.x;
            v3.y = v2.y;
            v3.z = v2.y;

            return v3;
        }

        public static Vector3 InvertV3Values(Vector3 vector3, string[] axesToInvert) {
            for (int i = 0; i < axesToInvert.Length; i++) {
                int axisId = Utils.GetAxisId(axesToInvert[i]);
                vector3[axisId] *= -1;
            }
            return vector3;
        }

        public static int GetV3Sign(Vector3 vector3)
        {
            float vectorValue = vector3.x + vector3.y;
            if(vectorValue >= 0) {
                return 1;
            } else {
                return -1;
            }
        }

        // Raises a Vector3 to a power while retaining
        // its original positive or negative values
        public static Vector3 ExponentiateV3(Vector3 rawVector, float power)
        {
            Vector3 exponentV3 = new Vector3();
            for (int i = 0; i < 3; i++) {
                exponentV3[i] = ExponentiatePosNegValue(rawVector[i], power);
            }
            return exponentV3;
        }

        // Raises a positive or negative value to a power while
        // retaining its original positive or negative value
        public static float ExponentiatePosNegValue(float rawValue, float power)
        {
            int sign = rawValue >= 0 ? 1 : -1;
            float distance = Mathf.Abs(rawValue);
            float exponentVal = distance > 0 ? Mathf.Pow(distance, power) : 0;
            return exponentVal *= sign;
        }
        
        public static Vector2 GetVector2Direction(Vector2[] vectors, bool invertX = false, bool invertY = false)
        {
            Vector2 vectorTotals = GetVector2Totals(vectors);
            
            if (Mathf.Abs(vectorTotals.x) > Mathf.Abs(vectorTotals.y)) {
                
                if (invertX) {
                    vectorTotals.x *= -1f;
                }

                return vectorTotals.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
            }
            
            if (invertY) {
                vectorTotals.y *= -1f;
            }

            return vectorTotals.y > 0 ? new Vector2(0, 1) : new Vector2(0, -1);
        }

        public static Vector2 GetVector2Totals(Vector2[] vectors)
        {
            float totalXForce = 0;
            float totalYForce = 0;

            for (int z = 0; z < vectors.Length; z++) {
                totalXForce += vectors[z].x;
                totalYForce += vectors[z].y;
            }
            
            return new Vector2(totalXForce, totalYForce);
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length) {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static bool IsPopulated(SimpleEventTrigger attribute)
        {
            if (attribute.SimpleEventTarget != null) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsPopulated(ComplexEventTrigger attribute)
        {
            if (attribute.ComplexEventTarget != null) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsPopulated(ColorReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(FloatReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(IntReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }


        public static bool IsPopulated(BoolReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(V2Reference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(V3Reference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated<t>(List<t> attribute)
        {
            return attribute.Count < 1 ? false : true;
        }

        public static bool IsPopulated(UnityEvent attribute)
        {
            if(attribute == null) {
                return false;
            }

            if(attribute.GetPersistentEventCount() < 1) {
                return false;
            }

            if(attribute.GetPersistentMethodName(0).Length < 1) {
                return false;
            }

            return true;
        }

        public static bool IsPopulated(ComplexUnityEventHandler attribute)
        {
            return attribute.GetPersistentMethodName(0).Length < 1 ? false : true;
        }

        public static int GetNthIndexFromEnd(string s, char t, int n)
        {
            int count = 0;
            for (int i = s.Length - 1; i >= 0; i--) {
                if (s[i] == t) {
                    count++;
                    if (count == n) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static bool IsPopulated(StringReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static float Quadratic(float k)
        {
            return (k * k) / 2;
        }

        private static float Asymptotic(float k)
        {
            return k / ((k * k) - 1);
        }

        private static float Cubic(float k)
        {
            return (k * k * (3 - k));
        }

        private static float Bounce(float k)
        {
            if ((k /= 1f) < (1f / 2.75f)) {
                return 7.5625f * k * k;
            }
            else if (k < (2f / 2.75f)) {
                return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
            }
            else if (k < (2.5 / 2.75)) {
                return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
            }
            else {
                return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
            }
        }

        private static float Circular(float k)
        {
            return Mathf.Sqrt(1f - (--k * k));
        }

        private static float Back(float k)
        {
            float b = 4f;
            return (k = k - 1f) * k * ((b + 1f) * k + b) + 1f;
        }

        private static float Elastic(float k)
        {
            float f = 0.22f;
            float e = 0.4f;

            if (k == 0) { return 0; }
            if (k == 1f) { return 1f; }

            return (e * Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - f / 4f) * (2f * Mathf.PI) / f) + 1f);

        }
    }



}