using System;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif


namespace AltSalt
{
    [Serializable]
    public class RectTransformRotationBehaviour : LerpToTargetBehaviour
    {
        public Vector3 initialRotation = new Vector3(0, 0, 0);
        public Vector3 targetRotation = new Vector3(0, 0, 0);

#if UNITY_EDITOR

        [InfoBox("Saves active object's position as initial.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.2f)]
        public void SaveActiveObjectInitialRotation()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            initialRotation = rectTransform.localEulerAngles;
        }


        [InfoBox("Saves active object's position as target.")]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.2f, 0.5f)]
        public void SaveActiveObjectTargetRotation()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            targetRotation = rectTransform.localEulerAngles;
        }
#endif
    }
}