using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class ReferenceBase
    {
        [ShowInInspector]
        private bool _hasSearchedForAsset = false;

        protected bool hasSearchedForAsset
        {
            get => _hasSearchedForAsset;
            set => _hasSearchedForAsset = value;
        }

        [SerializeField]
        [OnInspectorGUI(nameof(UpdateReferenceName), false)]
        [PropertySpace]
        [ReadOnly]
        protected string _referenceName;

        protected string referenceName
        {
            get => _referenceName;
            set => _referenceName = value;
        }
        
        [SerializeField]
        [ReadOnly]
        protected UnityEngine.Object _callingObject;

        public UnityEngine.Object callingObject
        {
            get => _callingObject;
            set => _callingObject = value;
        }

        protected void LogMissingReferenceMessage(string typeName)
        {
            Debug.Log($"Reference not found in {typeName} on {callingObject.name}. Searching assets for {referenceName}.", callingObject);
        }
        
        protected void LogFoundReferenceMessage(string typeName, UnityEngine.Object referenceObject)
        {
            Debug.Log($"Found reference {referenceObject.name}. Setting to {typeName}.", referenceObject);
        }

        protected abstract void UpdateReferenceName();
    }
}