/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformPosBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("initialPosition")]
        [SerializeField]
        private Vector3 _initialValue = new Vector3(0, 0, 0);

        public Vector3 initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [FormerlySerializedAs("targetPosition")]
        [SerializeField]
        private Vector3 _targetValue = new Vector3(0, 0, 0);

        public Vector3 targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

#if UNITY_EDITOR

        public override object SetInitialValueToTarget()
        {
            initialValue = targetValue;
            return initialValue;
        }

        [InfoBox("Saves active object's position as initial.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.2f)]
        public void SaveActiveObjectInitialPosition()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            initialValue = rectTransform.anchoredPosition3D;
        }

        
        [InfoBox("Saves active object's position as target.")]
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.2f, 0.5f)]
        public void SaveActiveObjectTargetPosition()
        {
            RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            targetValue = rectTransform.anchoredPosition3D;
        }
#endif
    }
}