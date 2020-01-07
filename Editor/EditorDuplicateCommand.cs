using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using AltSalt.Maestro.Animation;

namespace AltSalt.Maestro
{
    public static class EditorDuplicateCommand
    {
        [MenuItem("Edit/AltSaltDuplicate #d", false, 0)]
        static void DuplicateSelection()
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();

            if (Selection.gameObjects.Length >= 1) {
                
                newSelection.AddRange(Utils.DuplicateHierarchy(Selection.gameObjects));

//                for (int i = 0; i < Selection.gameObjects.Length; i++) {
//            
//                    GameObject[] fullSelection = Utils.GetChildGameObjects(Selection.gameObjects[i], true);
//                    GameObject rootObject = fullSelection[0];
//                    GameObject newRootObject = Utils.DuplicateObject(rootObject);
//
//                    for (int q = 1; i < fullSelection.Length; q++) {
//                        
//                        GameObject newGo = Utils.DuplicateObject(rootObject);
//
//                        newGo.transform.parent = rootObject.transform.parent;
//                        newGo.transform.SetSiblingIndex(rootObject.transform.GetSiblingIndex() + 1);
//                        Undo.RegisterCreatedObjectUndo(newGo, "Duplicate");
//
//                        newSelection.Add(newGo);
//                    }
//                }
            }

            if(TimelineEditor.inspectedAsset != null) {
                newSelection.AddRange(TrackPlacement.DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects));
                TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
            }

            Selection.objects = newSelection.ToArray();
        }
        
    }
}