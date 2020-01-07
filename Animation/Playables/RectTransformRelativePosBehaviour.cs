/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformRelativePosBehaviour : LerpToTargetBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("originReferenceObject")]
        private GameObject _originReferenceObject;

        public GameObject originReferenceObject
        {
            get => _originReferenceObject;
            set => _originReferenceObject = value;
        }

        [SerializeField]
        [FormerlySerializedAs("targetReferenceObject")]
        private GameObject _targetReferenceObject;

        public GameObject targetReferenceObject
        {
            get => _targetReferenceObject;
            set => _targetReferenceObject = value;
        }
        
        public Vector3 offsetVector = new Vector3(0,0,5);
    }
}