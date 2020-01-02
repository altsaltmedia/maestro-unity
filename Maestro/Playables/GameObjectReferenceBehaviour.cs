using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class GameObjectReferenceBehaviour : LerpToTargetBehaviour
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
    }
}