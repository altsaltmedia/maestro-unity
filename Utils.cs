/***********************************************

Copyright © AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / artemio@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace AltSalt.Maestro
{
	public enum FadeType { FadeIn, FadeOut }

	public enum DimensionType { Vertical, Horizontal }

    public enum AspectRatioType { x16x9, x9x16, x4x3, x3x4, x1x1, Dynamic }

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
        EnableDynamicElement,
        DisableDynamicElement,
        TextChanged,
        LayoutChanged,
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
        private static MaestroConfig configAsset;

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
        
        public static bool ContainsActiveContentExtensionConfig(List<IContentExtensionConfig> extensionConfigs, out IContentExtensionConfig contentExtensionConfig)
        {
            List<IContentExtensionConfig> activeModifyConfigs = new List<IContentExtensionConfig>();
            for (int i = 0; i < extensionConfigs.Count; i++) {
                if (extensionConfigs[i].active == true) {
                    activeModifyConfigs.Add(extensionConfigs[i]);
                }
            }

            if (activeModifyConfigs.Count > 0) {
                // Set to the one with highest priority
                activeModifyConfigs.Sort(new Utils.ContentExtensionConfigSort());
                contentExtensionConfig = activeModifyConfigs[activeModifyConfigs.Count - 1];
                return true;
            }

            contentExtensionConfig = null;
            return false;
        }
        
        public class ContentExtensionConfigSort : Comparer<IContentExtensionConfig>
        {
            public override int Compare(IContentExtensionConfig x, IContentExtensionConfig y)
            {
                return x.priority.CompareTo(y.priority);
            }
        }
        
        public static FieldInfo GetVariableFieldFromReference(FieldInfo referenceField, object referencingObject, out object referenceValue)
        {
            referenceValue = referenceField.GetValue(referencingObject);
            return referenceValue.GetType().GetField(
                "_variable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

#if UNITY_EDITOR
        public static string Capitalize(this string source)
        {
            switch (source) {

                case null:
                {
                    throw new ArgumentNullException(nameof(source));
                }

                case "":
                {
                    throw new ArgumentException($"{nameof(source)} must not be empty");
                }

                default:
                    return source.First().ToString().ToUpper() + source.Substring(1);
            }
        }
        
        public static GUISkin AltSaltSkin {

            get {
                if(altSaltSkin == null) {
                    altSaltSkin = Resources.Load("AltSaltEditorSkin") as GUISkin;
                }
                return altSaltSkin;
            }
        }

//        private static string CreateConfigFolder()
//        {
//            string[] subfolders = {nameof(AltSalt), nameof(MaestroConfig)};
//            string currentFolder = "Assets";
//            for (int i = 0; i < subfolders.Length; i++) {
//                if (AssetDatabase.IsValidFolder(($"{currentFolder}/{subfolders[i]}")) == false) {
//                    AssetDatabase.CreateFolder(currentFolder, subfolders[i]);
//                }
//                currentFolder += "/" + subfolders[i];
//            }
//
//            return currentFolder + "/";
//        }

        private static void GetConfigAsset()
        {
            if (configAsset == null) {
                configAsset = GetScriptableObject(nameof(MaestroConfig)) as MaestroConfig;
                
                if (configAsset == null) {
                    var instance = ScriptableObject.CreateInstance(typeof(MaestroConfig));
                    string configDirectory = GetDirectory(new[] {"Assets", nameof(AltSalt), nameof(MaestroConfig)});
                    string filePath = Utils.GetFilePath(configDirectory, nameof(MaestroConfig), ".asset");
                    AssetDatabase.CreateAsset(instance, filePath);
                    configAsset = AssetDatabase.LoadMainAssetAtPath(filePath) as MaestroConfig;
                }
            }
        }

        public static string rootNamespace => $"{nameof(AltSalt)}.{nameof(Maestro)}";

        public static string projectPath
        {
            get
            {
                GetConfigAsset();
                return configAsset.projectAssetsPath;
            }
        }

        public static string settingsPath
        {
            get
            {
                GetConfigAsset();
                return configAsset.settingsPath;
            }
        }

        public static string scriptsPath
        {
            get
            {
                GetConfigAsset();
                return configAsset.scriptsPath;
            }
        }

        public static string stylesheetPath => scriptsPath + "/Editor/EditorStyles.uss";

        public static string GetProjectDirectory(string[] subfolders)
        {
            List<string> projectSubfolders = projectPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            projectSubfolders.AddRange(subfolders);

            return GetDirectory(projectSubfolders.ToArray());
        }
        
        public static string GetDirectory(string[] subfolders)
        {
            string currentFolder = subfolders[0];
            
            if (AssetDatabase.IsValidFolder(($"{currentFolder}")) == false) {
                AssetDatabase.CreateFolder("", currentFolder);
            }

            for (int i = 1; i < subfolders.Length; i++) {
                if (AssetDatabase.IsValidFolder(($"{currentFolder}/{subfolders[i]}")) == false) {
                    AssetDatabase.CreateFolder(currentFolder, subfolders[i]);
                }
                currentFolder += "/" + subfolders[i];
            }
            return currentFolder + "/";
        }

        public static string GetFilePath(string basePath, string fileName, string extension = "")
        {
            StringBuilder path = new StringBuilder(basePath);
            string[] pathComponents = { fileName, extension };
            for (int i = 0; i < pathComponents.Length; i++) {
                path.Append(pathComponents[i]);
            }
            return path.ToString();
            
        }

        public static string ConvertRelativeToAbsolutePath(string assetPath)
        {
            string sanitizedPath = assetPath.Replace("Assets/", "");
            return Application.dataPath + sanitizedPath;
        }

        public static string GetAssetPathFromObject(Object targetObject)
        {
            string targetGUID = GetGUIDFromObject(targetObject);
            return AssetDatabase.GUIDToAssetPath(targetGUID);
        }

        public static string GetGUIDFromObject(Object targetObject)
        {
            string guid;
            long file;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetObject, out guid, out file);
            return guid;
        }
        
        // Will add (Clone) before a file extension
        public static string GetCloneAssetPath(string filePath, string suffix = "(Clone)")
        {
            string fileExtension = Path.GetExtension(filePath);
            string modifiedExtension = fileExtension.Replace(".", $"{suffix}.");
            return filePath.Replace(fileExtension, modifiedExtension);
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

        public static GameObject[] GetRootGameObjects(GameObject[] selection)
        {
            // Prep for traversal by getting all child objects from our selection, filtering out the root objects
            List<GameObject> childObjects = GetChildGameObjects(Utils.SortGameObjectSelection(selection)).ToList();

            List<GameObject> selectionRootObjects = new List<GameObject>();
            // Loop through our selection, and create a list of the root objects
            for (int i = 0; i < selection.Length; i++) {
                if (childObjects.Contains(selection[i]) == false) {
                    selectionRootObjects.Add(selection[i]);
                }
            }

            return selectionRootObjects.ToArray();
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
                gameObjectList = TraverseTransformChildren(gameObjectList, selection[i].transform, selection[i].transform, null, includeRoot);
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
        public static List<GameObject> TraverseTransformChildren(List<GameObject> targetList, Transform rootTransform, Transform currentNode, TraverseTransformDelegate traverseTransformDelegate = null, bool includeRoot = false)
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
                    TraverseTransformChildren(targetList, rootTransform, childTransform, traverseTransformDelegate);
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
                List<GameObject> totalHierarchySelection = TraverseTransformChildren(new List<GameObject>(), rootObjects[q].transform, rootObjects[q].transform, (List<GameObject> hierarchySelection, Transform currentTransform) => {
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

        public static bool TargetTypeSelected(Object currentSelection, Type targetType)
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

        public static bool TargetTypeSelected(Object[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                Type currentType = currentSelection[i].GetType();
                if (currentType.IsSubclassOf(targetType) || currentType == targetType) {
                    return true;
                }
            }
            return false;
        }

        public static Object[] RenameElements(string newName, Object[] targetObjects)
        {
            Array.Sort(targetObjects, new Utils.GameObjectSort());

            for (int i = 0; i < targetObjects.Length; i++) {
                if (newName.Contains("{x}")) {
                    targetObjects[i].name = newName.Replace("{x}", (i + 1).ToString());
                } else {
                    if (i == 0) {
                        targetObjects[i].name = newName;
                    } else {
                        targetObjects[i].name = String.Format("{0} ({1})", newName, i);
                    }
                }

            }

            return targetObjects;
        }
        public static Object[] FilterSelection(GameObject[] currentSelection, Type typeToSelect)
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

        public static Object[] FilterSelection(GameObject[] currentSelection, Type[] typeToSelect)
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

        public static Object[] FilterSelection(Object[] currentSelection, Type typeToSelect)
        {
            List<Object> newSelection = new List<Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                
                if (typeToSelect == typeof(GameObject)) {
                    if (currentSelection[i] is GameObject) {
                        newSelection.Add(currentSelection[i]);
                    }
                }
                else {
                    Type objectType = currentSelection[i].GetType();
                    if (objectType.IsSubclassOf(typeToSelect) || objectType == typeToSelect) {
                        newSelection.Add(currentSelection[i]);
                    }
                }
            }

            return newSelection.ToArray();
        }


        public static Object[] FilterSelection(Object[] currentSelection, Type[] typeToSelect)
        {
            List<Object> newSelection = new List<Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                for (int q = 0; q < typeToSelect.Length; q++) {
                    
                    if (typeToSelect[q] == typeof(GameObject)) {
                        if (currentSelection[i] is GameObject) {
                            newSelection.Add(currentSelection[i]);
                        }
                    }
                    else {
                        Type objectType = currentSelection[i].GetType();
                        if (objectType.IsSubclassOf(typeToSelect[q]) || objectType == typeToSelect[q]) {
                            newSelection.Add(currentSelection[i]);
                        }
                    }
                }
            }

            return newSelection.ToArray();
        }


        public static Object[] CullSelection(Object[] currentSelection, Type typeToOmit)
        {
            List<Object> newSelection = new List<Object>();

            for (int i = 0; i < currentSelection.Length; i++) {
                Type objectType = currentSelection[i].GetType();
                if (objectType.IsSubclassOf(typeToOmit) || objectType == typeToOmit) {
                    continue;
                }
                newSelection.Add(currentSelection[i]);
                
            }

            return newSelection.ToArray();
        }

        public static Object[] CullSelection(Object[] currentSelection, Type[] typeToOmit)
        {
            List<Object> newSelection = new List<Object>();

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
        
        
        public static GameObject[] DuplicateHierarchy(GameObject selection)
        {
            return DuplicateHierarchy(new[] { selection });
        }
        
        
        // Given a selection of game objects, we will traverse
        // and duplicate the hierarchy starting from the root objects 
        public static GameObject[] DuplicateHierarchy(GameObject[] selection)
        {
            // Ensure our selection is valid
            GameObject[] childObjects = GetChildGameObjects(Utils.SortGameObjectSelection(selection));
            for (int i = 0; i < childObjects.Length; i++) {
                if (PrefabUtility.IsPartOfPrefabAsset(childObjects[i]) == true) {
                    Debug.LogError("Duplicating prefab assets is not allowed." +
                                   "Please ensure your selection only contains prefab instances.");
                    return selection;
                }
            }
            
            GameObject[] selectionRootObjects = GetRootGameObjects(selection);
            
            // Given our list of root objects, pass those into our traversal function
            List<GameObject> duplicatedHierarchy = new List<GameObject>();
            for (int q = 0; q < selectionRootObjects.Length; q++) {
                duplicatedHierarchy = TraverseAndDuplicateHierarchy(duplicatedHierarchy, new Dictionary<int, GameObject>(), selectionRootObjects[q]);
            }
            return duplicatedHierarchy.ToArray();
        }
        
        // Given an empty list and a game object, will recursively traverse through the a hierarchy,
        // duplicating and re-parenting the duplicates as it goes. Important to note: This method requires an
        // empty dictionary, which we use to map instanceIDs of the old objects to the duplicated objects.
        public static List<GameObject> TraverseAndDuplicateHierarchy(List<GameObject> targetList,
            Dictionary<int, GameObject> originalDuplicateMap, GameObject currentParent, GameObject duplicatedParent = null)
        {
            // On the first call to this function, we duplicate the object passed in so
            // we can begin re-parenting the children 
            if (duplicatedParent == null) {
                
                // Use the overload of DuplicateGameObject that allows us to pass in a map, which
                // will contain a record of all the objects that get created
                duplicatedParent = Utils.DuplicateGameObject(currentParent, originalDuplicateMap, true);
                targetList.Add(duplicatedParent);
                
                // Make sure to add any dynamically duplicated children in the case of prefabs 
                GameObject[] duplicatedChildren = Utils.GetChildGameObjects(duplicatedParent);
                for (int i = 0; i < duplicatedChildren.Length; i++) {
                    if(targetList.Contains(duplicatedChildren[i]) == false) targetList.Add(duplicatedChildren[i]);
                }
            }
            
            // Begin traversing children
            for (int i = 0; i < currentParent.transform.childCount; i++) {
                GameObject currentChild = currentParent.transform.GetChild(i).gameObject;
                
                // Duplicate and re-parent the child, BUT ONLY if the current child isn't part of the same prefab
                // instance as its parent. The reason for this is that game objects inside prefabs, when they get
                // duplicated, must be cloned from a prefab asset, which already contains the whole hierarchy.
                // This means that, by the time we get to a child object of the prefab instance, it's already been duplicated.
                if (PrefabUtility.GetOutermostPrefabInstanceRoot(currentChild) != PrefabUtility.GetOutermostPrefabInstanceRoot(currentParent) &&
                    PrefabUtility.GetNearestPrefabInstanceRoot(currentChild) != currentParent) {
                    
                    GameObject newDuplicatedParent = Utils.DuplicateGameObject(currentChild, originalDuplicateMap, true);
                    newDuplicatedParent.transform.SetParent(duplicatedParent.transform);
                    targetList.Add(newDuplicatedParent);
                    
                    // Make sure to add any dynamically duplicated children in the case of prefabs 
                    GameObject[] duplicatedChildren = Utils.GetChildGameObjects(duplicatedParent);
                    for (int j = 0; j < duplicatedChildren.Length; j++) {
                        if(targetList.Contains(duplicatedChildren[j]) == false) targetList.Add(duplicatedChildren[j]);
                    }
                    
                    TraverseAndDuplicateHierarchy(targetList, originalDuplicateMap, currentChild, newDuplicatedParent);
                    continue;
                }
                
                // If our current child is part of the parent prefab, we still need to traverse through its children to
                // duplicate any added game objects that aren't part of the parent prefab. To do this, we check against our map
                // to find the already duplicated object, and pass that in to allow for re-parenting as we drill down. 
                if (originalDuplicateMap.ContainsKey(currentChild.GetInstanceID())) {
                    TraverseAndDuplicateHierarchy(targetList, originalDuplicateMap, currentChild, originalDuplicateMap[currentChild.GetInstanceID()]);
                }
            }
            
            return targetList;
        }

        public static GameObject DuplicateGameObject(GameObject sourceObject, bool removeAutoGeneratedChildren = false)
        {
            GameObject duplicate;
            
            if (PrefabUtility.IsPartOfPrefabAsset(sourceObject) == true) {
                Debug.LogError("Duplicating prefab assets is not allowed." +
                               "Please ensure your selection only contains prefab instances.");
                return sourceObject;
            }
            
            // Use special handling for any properly connected prefabs to maintain
            // the prefab connection, otherwise just duplicate the object normally
            if (PrefabUtility.GetPrefabInstanceStatus(sourceObject) == PrefabInstanceStatus.Connected) {
                duplicate = DuplicatePrefabInstance(sourceObject);
            } else {
                duplicate = DuplicateSimpleGameObject(sourceObject, removeAutoGeneratedChildren);
            }
            
            // Ensure that we set some correct housekeeping values for our objects
            if (sourceObject.transform.parent != null) {
                duplicate.transform.SetParent(sourceObject.transform.parent);
            }
            duplicate.transform.SetSiblingIndex(sourceObject.transform.GetSiblingIndex() + 1);
            duplicate.name = sourceObject.name;
            duplicate.transform.position = sourceObject.transform.position;
            duplicate.transform.localScale = sourceObject.transform.localScale;
            Undo.RegisterCreatedObjectUndo(duplicate, "duplicate " + sourceObject.name);
            
            return duplicate;
        }

        // Given an empty dictionary, this override creates a record of the original object
        // instanceIDs to the duplicates, allowing us to see which ones have already been cloned
        public static GameObject DuplicateGameObject(GameObject sourceObject, Dictionary<int, GameObject> originalDuplicateMap,
            bool removeAutoGeneratedChildren = false)
        {
            GameObject duplicate = DuplicateGameObject(sourceObject, removeAutoGeneratedChildren);

            GameObject[] sourceObjectHierarchy = Utils.GetChildGameObjects(sourceObject, true);
            GameObject[] duplicateObjectHierarchy = Utils.GetChildGameObjects(duplicate, true);
            
            for (int i = 0; i < duplicateObjectHierarchy.Length; i++) {
                if (originalDuplicateMap.ContainsKey(sourceObjectHierarchy[i].GetInstanceID()) == false) {
                    originalDuplicateMap.Add(sourceObjectHierarchy[i].GetInstanceID(), duplicateObjectHierarchy[i]);
                }
            }

            return duplicate;
        }

        // Duplicates a prefab by replicating its outermost prefab, then stripping away any
        // part of the prefab hierarchy that's not needed based on an originally selected object
        private static GameObject DuplicatePrefabInstance(GameObject selectedObject)
        {
            // Due to the fact that we may be working with nested prefabs, we need to traverse
            // to the outer game object and duplicate the entire prefab hierarchy. The reason is that
            // if a child prefab is instantiated as part of a parent prefab, the only way we can apply
            // its property modifications and added components is by comparing against the parent prefab asset
            GameObject outermostPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(selectedObject);
            GameObject duplicatedObject = PrefabUtility.InstantiatePrefab(outermostPrefabAsset) as GameObject;
            
            // Apply property modifications
            PropertyModification[] propertyModifications = PrefabUtility.GetPropertyModifications(selectedObject);
            PrefabUtility.SetPropertyModifications(duplicatedObject, propertyModifications);

            // Apply added components by going to the root of both the
            // original and duplicated instances to gather all of the additions
            GameObject outermostSelectedInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedObject);
            GameObject outermostDuplicateInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(duplicatedObject);
            GameObject migratedObject = MigrateAddedComponents(outermostSelectedInstance, outermostDuplicateInstance);
            
            // If our originally selected object was the outermost root of the
            // prefab hierarchy, then we're done; just return the object we just migrated
            if (selectedObject == PrefabUtility.GetOutermostPrefabInstanceRoot(selectedObject)) {
                return migratedObject;
            }
            
            // Otherwise, that means our target object is nested within the prefab hierarchy, so we need
            // to strip out the part of the hierarchy we don't need. So we get a reference to the outer object,
            // then unpack the parent until we get to the part of the hierarchy we want - the reason for this
            // is we are unable to re-parent nested prefabs unless their parents have been unpacked.
            duplicatedObject = ExtractNestedPrefabInstance(outermostDuplicateInstance, duplicatedObject);

            // Re-parent our duplicated object out of the hierarchy, and set its sibling index so it resides beside our source object
            duplicatedObject.transform.parent = selectedObject.transform.parent != null ? selectedObject.transform.parent : null;
            duplicatedObject.transform.SetSiblingIndex(selectedObject.transform.parent.GetSiblingIndex() + 1);
            
            // Finally, delete the leftover hierarchy we don't need
            Object.DestroyImmediate(outermostDuplicateInstance);

            return duplicatedObject;
        }
        
        // Given a source prefab instance and a duplicate, will copy added components to the duplicate
        private static GameObject MigrateAddedComponents(GameObject sourceObject, GameObject duplicateObject)
        {
            // Create a dictionary to map the old instanceIDs to the new game objects.
            Dictionary<int, GameObject> migrationDictionary = new Dictionary<int, GameObject>();
            
            // It's possible that the source object may contain added game objects, and since
            // our duplicate is of the original prefab, we need to filter out those added objects
            List<GameObject> sourceObjectHierarchy = Utils.GetChildGameObjects(sourceObject, true).ToList();
            List<GameObject> sanitizedSourceObjectHierarchy = new List<GameObject>();
            for (int i = 0; i < sourceObjectHierarchy.Count; i++) {
                GameObject childObject = sourceObjectHierarchy[i];
                GameObject outermostRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(childObject); 
                if (outermostRoot == sourceObject) {
                    sanitizedSourceObjectHierarchy.Add(childObject);
                }
            }
            
            // No need to sanitize the duplicate hierarchy - it contains the objects we're trying to populate
            GameObject[] duplicateObjectHierarchy = Utils.GetChildGameObjects(duplicateObject, true);

            if (sanitizedSourceObjectHierarchy.Count != duplicateObjectHierarchy.Length) {
                Debug.LogError("Sanitized source hierarchy and duplicate hierarchy are not identical. " +
                               "Added components may not get duplicated as intended.");
            }

            // Map the old instanceIDs to the new objects
            for (int i = 0; i < duplicateObjectHierarchy.Length; i++) {
                migrationDictionary.Add(sanitizedSourceObjectHierarchy[i].GetInstanceID(), duplicateObjectHierarchy[i]);
            }
            
            List<AddedComponent> addedComponents = PrefabUtility.GetAddedComponents(sourceObject);

            for (int i = 0; i < addedComponents.Count; i++) {
                foreach (var migrationNode in migrationDictionary) {
                    
                    // Put the added components on corresponding duplicate objects using our map 
                    if (addedComponents[i].instanceComponent.gameObject.GetInstanceID() == migrationNode.Key) {
                        
                        Type addedComponentType = addedComponents[i].instanceComponent.GetType();
                        Component migratedComponent = migrationNode.Value.AddComponent(addedComponentType);
                        
                        EditorUtility.CopySerialized(addedComponents[i].instanceComponent, migratedComponent);
                        if(migratedComponent is SerializableElement serializableElement) {
                            serializableElement.Reinitialize();
                        }
                    }
                }
            }

            return duplicateObject;
        }

        // Unpacks a parent prefab until it reaches the target nested object
        private static GameObject ExtractNestedPrefabInstance(GameObject parentObject, GameObject nestedObject)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(parentObject) == false ||
                PrefabUtility.IsPartOfAnyPrefab(nestedObject) == false) {
                Debug.LogError("Both parameters must be prefabs");
            }
            
            // Create a collection of all children from the root,
            // as well as a smaller collection of children we actually want.
            GameObject[] allHierarchyChildren = Utils.GetChildGameObjects(parentObject);
            List<GameObject> targetChildren = Utils.GetChildGameObjects(nestedObject, true).ToList();
            
            // Traverse through the hierarchy, unpacking all of the prefabs in it, except for the ones
            // that are a part of our nested object. We need to do this because you can't extract / re-parent
            // a nested prefab without unpacking it first
            PrefabUtility.UnpackPrefabInstance(parentObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            for (int i = 0; i < allHierarchyChildren.Length; i++) {
                GameObject currentChild = allHierarchyChildren[i];
                if (PrefabUtility.IsPartOfPrefabInstance(currentChild) == true && targetChildren.Contains(currentChild) == false) {
                    parentObject = PrefabUtility.GetOutermostPrefabInstanceRoot(allHierarchyChildren[i]);
                    PrefabUtility.UnpackPrefabInstance(parentObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                }
            }

            return nestedObject;
        }

        private static GameObject DuplicateSimpleGameObject(GameObject sourceObject, bool removeAutoGeneratedChildren)
        {
            GameObject duplicate = Object.Instantiate(sourceObject);
            GameObject[] duplicateChildren = Utils.GetChildGameObjects(duplicate);
                
            // By default, Unity will duplicate all of a game object's children.
            // In some cases, we want to manually create the hierarchy one by one
            // (or not at all), so this removes those auto-generated objects
            if (removeAutoGeneratedChildren == true) {
                for (int i = 0; i < duplicateChildren.Length; i++) {
                    Object.DestroyImmediate(duplicateChildren[i]);
                }
            }
                
            Component[] serializableComponents = sourceObject.GetComponents(typeof(SerializableElement));
            for (int i = 0; i < serializableComponents.Length; i++) {
                (serializableComponents[i] as SerializableElement).Reinitialize();
            }

            return duplicate;
        }

        public class ClipTimeSort : Comparer<TimelineClip>
        {
            public override int Compare(TimelineClip x, TimelineClip y)
            {
                return x.start.CompareTo(y.start);
            }
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

        public static ComplexPayload CreateEventPayload()
        {
            ComplexPayload payloadInstance = ScriptableObject.CreateInstance(typeof(ComplexPayload)) as ComplexPayload;
            return payloadInstance;
        }

#if UNITY_EDITOR
        static void LogDuplicateAssetWarning(string assetName)
        {
            Debug.LogWarning("More than one matching asset found for search " + assetName + ". Please check to see if this is intentional.");
        }

        public static ScriptableObject CreateScriptableObjectAsset(Type assetType, string name, string path)
        {
            var instance = ScriptableObject.CreateInstance(assetType);
            string[] arrayPath = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
            string productionSettingsDirectory = Utils.GetDirectory(arrayPath);
            string filePath = Utils.GetFilePath(productionSettingsDirectory, name, ".asset");
            AssetDatabase.CreateAsset(instance, filePath);
            return AssetDatabase.LoadMainAssetAtPath(filePath) as ScriptableObject;
        }

        public static AppSettings GetAppSettings()
        {
            string[] rawGuids;
            string path;

            rawGuids = AssetDatabase.FindAssets("t:" + nameof(AppSettings), new [] { settingsPath, projectPath });
            string[] guids = rawGuids.Distinct().ToArray();

            if (guids.Length < 1) {
                Debug.Log($"App settings not found, creating new instance.");
                return CreateScriptableObjectAsset(typeof(AppSettings), nameof(AppSettings), settingsPath) as AppSettings;
            }
            
            if(guids.Length > 1) {
                Debug.LogWarning("More than one matching App Settings asset found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (AppSettings)AssetDatabase.LoadAssetAtPath(path, typeof(AppSettings));
        }

        public static SimpleEvent GetSimpleEvent(string target)
        {
            string[] guids;
            string path;
            string typeName = typeof(SimpleEvent).Name;

            guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }
            
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

            guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }
            
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

            guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }
            
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

            guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }
            
            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (BoolVariable)AssetDatabase.LoadAssetAtPath(path, typeof(BoolVariable));
        }

        public static dynamic GetScriptableObject(string target)
        {
            if (String.IsNullOrEmpty(target) == true) {
                return null;
            }
            
            string typeName = typeof(ScriptableObject).Name;
            string[] guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }

            // Get the file paths and filter for exact matches.
            List<string> filePaths = new List<string>();
            
            for (int i = 0; i < guids.Length; i++) {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                string fileName = Path.GetFileNameWithoutExtension(filePath); 
                if (fileName == target) {
                    filePaths.Add(filePath);
                }
            }

            if (filePaths.Count > 1) {
                LogDuplicateAssetWarning(target);
            }

            return (ScriptableObject)AssetDatabase.LoadAssetAtPath(filePaths[0], typeof(ScriptableObject));
        }

        
        public static dynamic GetCustomKey(string target)
        {
            if (String.IsNullOrEmpty(target) == true) {
                return null;
            }
            
            string[] guids;
            string path;
            string typeName = typeof(CustomKey).Name;

            guids = AssetDatabase.FindAssets(String.Format("{0} t:{1}", target, typeName));

            if (guids.Length < 1) {
                Debug.Log($"Asset {target} of type  {typeName} not found.");
                return null;
            }
            
            if (guids.Length > 1) {
                LogDuplicateAssetWarning(target);
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (CustomKey)AssetDatabase.LoadAssetAtPath(path, typeof(CustomKey));
        }

        public static void RepaintInspector(Type t)
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
        
        public static SerializedProperty FindReferenceProperty(SerializedObject sourceObject, string referencePropertyPath)
        {
            return FindReferenceProperty(sourceObject, referencePropertyPath.Split('.'));
        }

        public static SerializedProperty FindReferenceProperty(SerializedObject sourceObject, string[] referencePropertyPath)
        {
            var variableReferencePath = sourceObject.FindProperty(referencePropertyPath[0]);
            
            // If our serialized property path is only one item long, that means the variable reference
            // exists at the root of our serialized object, so we can just look for the _variable field right away
            if (referencePropertyPath.Length == 1) {
                return variableReferencePath;
            }
            
            // Otherwise, drill down through the defined path
            for (int i = 1; i < referencePropertyPath.Length; i++) {
                if (Int32.TryParse(referencePropertyPath[i], out var arrayIndex) == false) {
                    variableReferencePath = variableReferencePath.FindPropertyRelative(referencePropertyPath[i]);
                }
                else {
                    variableReferencePath = variableReferencePath.GetArrayElementAtIndex(arrayIndex);
                }
            }

            return variableReferencePath;
        }
        
        public static SerializedProperty FindReferenceProperty(SerializedObject sourceObject, string[] referencePropertyPath, string targetProperty)
        {
            var variableReferencePath = sourceObject.FindProperty(referencePropertyPath[0]);
            
            // If our serialized property path is only one item long, that means the variable reference
            // exists at the root of our serialized object, so we can just look for the _variable field right away
            if (referencePropertyPath.Length == 1) {
                return variableReferencePath.FindPropertyRelative(targetProperty);
            }
            
            // Otherwise, drill down through the defined path
            for (int i = 1; i < referencePropertyPath.Length; i++) {
                if (Int32.TryParse(referencePropertyPath[i], out var arrayIndex) == false) {
                    variableReferencePath = variableReferencePath.FindPropertyRelative(referencePropertyPath[i]);
                }
                else {
                    variableReferencePath = variableReferencePath.GetArrayElementAtIndex(arrayIndex);
                }
            }

            return variableReferencePath.FindPropertyRelative(targetProperty);
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

        public static Vector2 InvertV2Values(Vector2 vector2, string[] axesToInvert) {
            for (int i = 0; i < axesToInvert.Length; i++) {
                int axisId = Utils.GetAxisId(axesToInvert[i]);
                vector2[axisId] *= -1;
            }
            return vector2;
        }

        public static int GetV2Sign(Vector2 vector3)
        {
            float vectorValue = vector3.x + vector3.y;
            if(vectorValue >= 0) {
                return 1;
            }

            return -1;
        }

        // Raises a Vector3 to a power while retaining
        // its original positive or negative values
        public static Vector3 ExponentiateV2(Vector2 rawVector, float power)
        {
            Vector3 exponentV3 = new Vector3();
            for (int i = 0; i < 2; i++) {
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

        public static bool IsPopulated(SimpleEventReference attribute)
        {
            if (attribute.GetVariable() != null) {
                return true;
            } else {
                return false;
            }
        }
        
        public static bool IsPopulated(SimpleEventTrigger attribute)
        {
            if (attribute.GetVariable() != null) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            if (attribute.GetVariable() != null) {
                return true;
            } else {
                return false;
            }
        }
        
        public static bool IsPopulated(ReferenceBase attribute)
        {
            if (attribute.GetVariable() != null) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsPopulated(ColorReference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
            }
        }

        public static bool IsPopulated(FloatReference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
            }
        }

        public static bool IsPopulated(IntReference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
            }
        }


        public static bool IsPopulated(BoolReference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
            }
        }

        public static bool IsPopulated(V2Reference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
            }
        }

        public static bool IsPopulated(V3Reference attribute)
        {
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
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
        
        public static bool IsPopulated(GameObjectGenericAction attribute)
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

        public static bool IsPopulated(ComplexPayloadGenericAction attribute)
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
            if (attribute.useConstant == true) {
                return true;
            }
            else {
                return attribute.GetVariable() == null ? false : true;
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