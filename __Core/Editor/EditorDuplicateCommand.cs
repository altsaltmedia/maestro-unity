using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorDuplicateCommand
{
    [MenuItem("Edit/AltSaltDuplicate #d", false, 0)]
    static void DuplicateSelection()
    {
        List<GameObject> newSelection = new List<GameObject>();
        for (int ui = 0; ui < Selection.gameObjects.Length; ui++) {
        
            GameObject go = Selection.gameObjects[ui];
            int siblingIndex = go.transform.GetSiblingIndex();

            GameObject newGo;

            GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
            if (prefabRoot != null) {
                newGo = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
                PrefabUtility.SetPropertyModifications(newGo, PrefabUtility.GetPropertyModifications(go));
            } else {
                newGo = GameObject.Instantiate(go);
            }
            newGo.transform.parent = go.transform.parent;
            newGo.transform.position = go.transform.position;
            newGo.transform.localScale = go.transform.localScale;
            newGo.transform.SetSiblingIndex(siblingIndex + 1);
            Undo.RegisterCreatedObjectUndo(newGo, "Duplicate");
            newGo.name = go.name;
            newSelection.Add(newGo);
        }
        Selection.objects = newSelection.ToArray();
    }
}

