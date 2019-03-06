/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

namespace AltSalt
{
    [Serializable]
    public class RectTransformPosBehaviour : LerpToTargetBehaviour
    {
        public Vector3 initialPosition = new Vector3(0, 0, 0);
        public Vector3 targetPosition = new Vector3(0, 0, 0);

#if UNITY_EDITOR
        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves active object's position as initial.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.2f)]
        public void SaveActiveObjectInitialPosition()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            initialPosition = rectTransform.localPosition;
        }

        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves active object's position as target.")]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.2f, 0.5f)]
        public void SaveActiveObjectTargetPosition()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            targetPosition = rectTransform.localPosition;
        }
#endif
    }
}