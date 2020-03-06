using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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
                if (_sceneAspectRatio.GetVariable() == null) {
                    _sceneAspectRatio.SetVariable(Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio)));
                }
                return _sceneAspectRatio.GetValue();
            }
        }

        public static GameObject CreateElement(Transform[] selectedTransforms, GameObject sourceObject, string elementName = "", bool isParent = false)
        {
            GameObject newElement = PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
            string undoMessage = "create " + sourceObject.name;
            Undo.RegisterCreatedObjectUndo(newElement, undoMessage);

            if (elementName.Length > 0) {
                newElement.name = elementName;
            }

            // Handling to set the newly created object as a parent to the current selection.
            // When multiple objects are selected, we assume that the user wants the new object
            // to be the parent to the selection. Optionally, passing in the isParent bool will also
            // trigger this conditional.
            if (selectedTransforms.Length > 1 || (selectedTransforms.Length > 0 && isParent == true)) {

                Array.Sort(selectedTransforms, new Utils.TransformSort());
                
                // Store the parent and sibIndex of the current selection first
                Transform parentTransform = selectedTransforms[0].parent;
                int sibIndex = selectedTransforms[0].GetSiblingIndex();

                // Set the selection's parent to be the newly created object
                for (int i = 0; i < selectedTransforms.Length; i++) {
                    Undo.SetTransformParent(selectedTransforms[i], newElement.transform, "set parent on game objects");
                }

                // Re-parent the newly created element so that it appears in the hierarchy where you'd expect it to be
                Undo.SetTransformParent(newElement.transform, parentTransform, "set parent on new element");
                newElement.transform.SetSiblingIndex(sibIndex);
                newElement.layer = selectedTransforms[0].gameObject.layer;
                
            // Otherwise, set the newly created element as the child of the current selection. 
            } else if (selectedTransforms.Length == 1) {
                newElement.transform.SetParent(selectedTransforms[0]);
                newElement.layer = selectedTransforms[0].gameObject.layer;
            }

            FocusHierarchyWindow();
            return newElement;
        }

        public static void FocusHierarchyWindow()
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

        [MenuItem("Edit/Maestro/Duplicate #d", false, -1000)]
        private static void DuplicateSelection()
        {
            List<Object> newSelection = new List<Object>();

            if (Selection.gameObjects.Length >= 1) {
                newSelection.AddRange(Utils.DuplicateHierarchy(Selection.gameObjects));
            }

            if(TimelineEditor.inspectedAsset != null) {
                newSelection.AddRange(TrackPlacement.DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects));
                TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
            }

            Selection.objects = newSelection.ToArray();
        }

        [MenuItem("Edit/Maestro/Deselect All",false, -1000)]
        public static void DeselectAll()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            Selection.objects = new UnityEngine.Object[0];
            TimelineUtils.RefreshTimelineContentsModified();
        }
    }
}
