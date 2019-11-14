using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt.Maestro
{
    public static class EditorToolsCore
    {
        public static string ToggleableGroup = "toggleable-group";
        public static string UpdateWindowTrigger = "update-window-trigger";

        // 
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
    }
}