using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt
{
    public static class EditorDuplicateCommand
    {
        [MenuItem("Edit/AltSaltDuplicate #d", false, 0)]
        static void DuplicateSelection()
        {
            List<GameObject> newSelection = new List<GameObject>();
            for (int i = 0; i < Selection.gameObjects.Length; i++) {
        
                GameObject sourceObject = Selection.gameObjects[i];
                GameObject newGo = Utils.DuplicateObject(sourceObject);

                newGo.transform.parent = sourceObject.transform.parent;
                newGo.transform.SetSiblingIndex(sourceObject.transform.GetSiblingIndex() + 1);
                Undo.RegisterCreatedObjectUndo(newGo, "Duplicate");

                newGo.hideFlags = HideFlags.DontUnloadUnusedAsset;
                newSelection.Add(newGo);
            }
            Selection.objects = newSelection.ToArray();
        }
    }
}