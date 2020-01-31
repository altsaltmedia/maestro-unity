using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class ReferenceBase
    {
#if UNITY_EDITOR
        private const string variablePropertyPath = "_variable";

        [SerializeField]
        [HideInInspector]
        private bool _isSystemReference;

        public bool isSystemReference
        {
            get => _isSystemReference;
            set => _isSystemReference = value;
        }

        [ShowInInspector]
        [OnValueChanged(nameof(CallPopulateVariable))]
        [ShowIf(nameof(searchAttempted))]
        private bool _searchAttempted = false;

        private bool searchAttempted
        {
            get => _searchAttempted;
            set => _searchAttempted = value;
        }

        [SerializeField]
        [ShowIf(nameof(searchAttempted))]
        protected string _referenceName;

        public string referenceName
        {
            get => _referenceName;
            set => _referenceName = value;
        }
        
        private UnityEngine.Object _parentObject;

        public UnityEngine.Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }
        
        private List<string> _serializedPropertyPath = new List<string>();

        private List<string> serializedPropertyPath
        {
            get
            {
                if (_serializedPropertyPath == null) {
                    _serializedPropertyPath = new List<string>();
                }
                return _serializedPropertyPath;
            }
            set => _serializedPropertyPath = value;
        }
#endif

        public virtual ScriptableObject GetVariable()
        {
#if UNITY_EDITOR
//            if (isSystemReference == true) {
//                return null;
//            } 
//            
//            if (parentObject == null || serializedPropertyPath.Count < 1) {
//                throw new System.NullReferenceException($"You must specify a parent object and serialized property path on {this.GetType().Name}");
//            }
#endif
            return null;
        }

#if UNITY_EDITOR
        private ReferenceBase CallPopulateVariable()
        {
            return PopulateVariable(parentObject, serializedPropertyPath.ToArray());
        }
        
        public ReferenceBase PopulateVariable(UnityEngine.Object parentObject, string serializedPropertyPath)
        {
            return PopulateVariable(parentObject, new[] {serializedPropertyPath});
        }
        
        public ReferenceBase PopulateVariable(UnityEngine.Object parentObject, string[] serializedPropertyPath)
        {
//            
//            SerializedProperty parentObjectProperty =
//                FindReferenceProperty(serializedObject, serializedPropertyPath, nameof(_parentObject));
//            parentObjectProperty.objectReferenceValue = parentObject;
//
//            SerializedProperty propertyPathProperty = FindReferenceProperty(serializedObject, serializedPropertyPath,
//                nameof(_serializedPropertyPath));
//            propertyPathProperty.ClearArray();
//            for (int i = 0; i < serializedPropertyPath.Length; i++) {
//                propertyPathProperty.InsertArrayElementAtIndex(i);
//                propertyPathProperty.GetArrayElementAtIndex(i).stringValue = serializedPropertyPath[i];
//            }
//
//            serializedObject.ApplyModifiedProperties();
//            serializedObject.Update();
//            
            this.parentObject = parentObject;
            this.serializedPropertyPath.Clear();
            this.serializedPropertyPath.AddRange(serializedPropertyPath);

            if (isSystemReference == false && searchAttempted == false &&
                ShouldPopulateReference() == true && string.IsNullOrEmpty(referenceName) == false) {
                
                SerializedObject serializedObject = new SerializedObject(parentObject);

                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName);
                
                if (variableSearch != null) {
                    
                    SerializedProperty variableProperty =
                        FindReferenceProperty(serializedObject, serializedPropertyPath, variablePropertyPath);
                    variableProperty.objectReferenceValue = variableSearch;
                    
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    //EditorApplication.update.Invoke();
                    //StoreVariable(variableSearch);
                    
                    LogFoundReferenceMessage(GetType().Name, variableSearch);
                }
                else {
                    LogNotFoundError(GetType().Name);
                }
            }

            if (ReadVariable() != null) {
                UpdateReferenceName();
            }

            return this;
        }

        protected abstract bool ShouldPopulateReference();
        
        public static SerializedProperty FindReferenceProperty(SerializedObject sourceObject, string[] referencePropertyPath, string targetProperty)
        {
            var variableReferencePath = sourceObject.FindProperty(referencePropertyPath[0]);
            
            // If our serialized property path is only one item long, that means the variable reference
            // exists at the root of our serialized object, so we can just look for the _variable field right away
            if (referencePropertyPath.Length == 1) {
                return variableReferencePath.FindPropertyRelative(targetProperty);
            }
            
            // Otherwise, drill down through the defined path
            for (int i = 1; i < referencePropertyPath.Length; i++) {
                if (int.TryParse(referencePropertyPath[i], out var arrayIndex) == false) {
                    variableReferencePath = variableReferencePath.FindPropertyRelative(referencePropertyPath[i]);
                }
                else {
                    variableReferencePath = variableReferencePath.GetArrayElementAtIndex(arrayIndex);
                }
            }

            return variableReferencePath.FindPropertyRelative(targetProperty);
        }

        public ReferenceBase ResetSearchAttempted()
        {
            searchAttempted = false;
            return this;
        }

        private ReferenceBase StoreVariable(ScriptableObject variable)
        {
            SerializedObject serializedObject = new SerializedObject(parentObject);
            var variableReferencePath = serializedObject.FindProperty(serializedPropertyPath[0]);
            
            // If our serialized property path is only one item long, that means the variable reference
            // exists at the root of our serialized object, so we can just look for the _variable field right away
            if (serializedPropertyPath.Count == 1) {
                variableReferencePath.FindPropertyRelative("_variable").objectReferenceValue = variable;
            }
            
            // Otherwise, drill down through the defined path
            else {
                for (int i = 1; i < serializedPropertyPath.Count; i++) {
                    if (int.TryParse(serializedPropertyPath[i], out var arrayIndex) == false) {
                        variableReferencePath = variableReferencePath.FindPropertyRelative(serializedPropertyPath[i]);
                    }
                    else {
                        variableReferencePath = variableReferencePath.GetArrayElementAtIndex(arrayIndex);
                    }
                }
                variableReferencePath.FindPropertyRelative("_variable").objectReferenceValue = variable;
            }

            serializedObject.ApplyModifiedProperties();
            return this;
        }

        protected abstract ScriptableObject ReadVariable();

        protected ReferenceBase UpdateReferenceName()
        {
            referenceName = ReadVariable().name;
            return this;
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
        
        protected void LogNotFoundError(string typeName)
        {
            Debug.LogError($"Unable to find reference {referenceName} for {typeName} on {parentObject.name}. " +
                      $"Please repopulate the variable, ensure all assets have been imported, or remove this reference.", parentObject);
        }
#endif
    }
}