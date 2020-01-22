using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class ReferenceBase
    {
        [ShowInInspector]
        private bool _searchAttempted = false;

        protected bool searchAttempted
        {
            get => _searchAttempted;
            set => _searchAttempted = value;
        }

        [SerializeField]
        [OnInspectorGUI(nameof(UpdateReferenceName), false)]
        protected string _referenceName;

        public string referenceName
        {
            get => _referenceName;
            set => _referenceName = value;
        }
        
        [SerializeField]
        protected UnityEngine.Object _parentObject;

        public UnityEngine.Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        protected void LogMissingReferenceMessage(string typeName)
        {
            if (parentObject != null) {
                Debug.Log($"Reference not found in {typeName} on {parentObject.name}. Searching assets for {referenceName}.", parentObject);
            }
            else {
                Debug.Log($"Reference not found in {typeName}. Searching assets for {referenceName}.");
            }
        }
        
        protected void LogFoundReferenceMessage(string typeName, UnityEngine.Object referenceObject)
        {
            Debug.Log($"Found reference {referenceObject.name}. Setting to {typeName}.", referenceObject);
        }

        protected abstract void UpdateReferenceName();
    }
}