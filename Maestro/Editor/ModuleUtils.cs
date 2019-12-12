﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    public static class ModuleUtils
    {
        private const string _toggleableGroup = "toggleable-group";
        
        public static string toggleableGroup => _toggleableGroup;
        
        private const string _updateWindowTrigger = "update-window-trigger";
        
        public static string updateWindowTrigger => _updateWindowTrigger;
        
        private static ModuleReferences _moduleReferences;

        public static ModuleReferences moduleReferences {

            get {
                if(_moduleReferences == null) {
                    _moduleReferences = Utils.GetScriptableObject(nameof(ModuleReferences)) as ModuleReferences;
                }
                return _moduleReferences;
            }
        }

        private static FloatReference _sceneAspectRatio = new FloatReference();

        public static float sceneAspectRatio {

            get {
                if (_sceneAspectRatio.Variable == null) {
                    _sceneAspectRatio.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio));
                }
                return _sceneAspectRatio.Value;
            }
        }

        public static GameObject CreateNewElement(Transform[] selectedTransforms, GameObject sourceObject, string elementName = "", bool isParent = false)
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            string undoMessage = "create " + sourceObject.name;
            Undo.RegisterCreatedObjectUndo(newElement, undoMessage);

            if (elementName.Length > 0) {
                newElement.name = elementName;
            }

            if (selectedTransforms.Length > 1 || (selectedTransforms.Length > 0 && isParent == true)) {

                Array.Sort(selectedTransforms, new Utils.TransformSort());
                Transform parentTransform = selectedTransforms[0].parent;
                int sibIndex = selectedTransforms[0].GetSiblingIndex();

                for (int i = 0; i < selectedTransforms.Length; i++) {
                    Undo.SetTransformParent(selectedTransforms[i], newElement.transform, "set parent on game objects");
                }

                Undo.SetTransformParent(newElement.transform, parentTransform, "set parent on new element");
                newElement.transform.SetSiblingIndex(sibIndex);

            } else if (selectedTransforms.Length == 1) {
                newElement.transform.SetParent(selectedTransforms[0]);
            }
            
            FocusHierarchyWindow();
            return newElement;
        }

        private static void FocusHierarchyWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        }

        public static GameObject[] AddComponentToSelection(GameObject[] gameObjects, Type componentToAdd)
        {
            List<GameObject> gameObjectList = new List<GameObject>();

            for(int i=0; i<gameObjects.Length; i++) {

                if(gameObjects[i].GetComponent(componentToAdd) == null) {
                    gameObjectList.Add(gameObjects[i]);
                    Undo.AddComponent(gameObjects[i], componentToAdd);
                    Debug.Log("Added " + componentToAdd.Name + " to " + gameObjects[i].name);
                } else {
                    EditorUtility.DisplayDialog("Component already exists", "The component " + componentToAdd + " already exists on " + gameObjects[i].name, "Okay");
                }
            }

            return gameObjectList.ToArray();
        }

        public static VisualElementToggleData AddToVisualElementToggleData(VisualElementToggleData targetToggleData, Enum targetCondition, VisualElement elementToAdd)
        {
            if (targetToggleData.ContainsKey(targetCondition)) {
                targetToggleData[targetCondition].Add(elementToAdd);
            } else {
                List<VisualElement> visualElementList = new List<VisualElement>();
                visualElementList.Add(elementToAdd);
                targetToggleData.Add(targetCondition, visualElementList);
            }

            return targetToggleData;
        }

        public static VisualElementToggleData ToggleVisualElements(VisualElementToggleData sourceToggleData, Enum targetCondition, bool targetStatus = false)
        {
            if (sourceToggleData.ContainsKey(targetCondition)) {
                List<VisualElement> visualElementList = sourceToggleData[targetCondition];
                for (int i = 0; i < visualElementList.Count; i++) {
                    if (visualElementList[i] is Foldout) {
                        (visualElementList[i] as Foldout).value = targetStatus;
                    }
                    visualElementList[i].SetEnabled(targetStatus);
                }
            }
            return sourceToggleData;
        }

        public static ListView ToggleListView(ListView targetListView, bool expandListView)
        {
            if (targetListView != null) {
                if (expandListView == true) {
                    targetListView.AddToClassList("expanded");
                } else {
                    targetListView.RemoveFromClassList("expanded");
                }
            }
            return targetListView;
        }
    }
}