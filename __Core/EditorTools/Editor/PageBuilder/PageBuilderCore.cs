using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt
{
    public static class PageBuilderCore
    {
        static PageBuilderReferences prefabReferences;

        public static PageBuilderReferences PrefabReferences {

            get {
                if(prefabReferences == null) {
                    prefabReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;
                }
                return prefabReferences;
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

            return newElement;
        }

        public static ResponsiveElement[] AddBreakpointToSelection(GameObject[] gameObjects, float targetBreakpoint)
        {
            List<ResponsiveElement> componentList = new List<ResponsiveElement>();

            for (int i = 0; i < gameObjects.Length; i++) {

                ResponsiveElement[] objectComponents = gameObjects[i].GetComponents<ResponsiveElement>();
                for (int q = 0; q < objectComponents.Length; q++) {
                    objectComponents[i].AddBreakpoint(targetBreakpoint);
                }

                componentList.AddRange(objectComponents);
            }
            return componentList.ToArray();
        }

        public static IResponsiveSaveable[] SaveResponsiveValues(GameObject[] gameObjects)
        {
            List<IResponsiveSaveable> componentList = new List<IResponsiveSaveable>();

            for(int i=0; i<gameObjects.Length; i++) {

                IResponsiveSaveable[] objectComponents = gameObjects[i].GetComponents<IResponsiveSaveable>();
                for(int q=0; q<objectComponents.Length; q++) {
                    objectComponents[i].SaveValue();
                }

                componentList.AddRange(objectComponents);
            }
            return componentList.ToArray();
        }
    }
}
