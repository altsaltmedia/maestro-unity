using System;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V3Reference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Vector3 _constantValue;

        public Vector3 constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }
        
        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private V3Variable _variable;

        public V3Variable variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as V3Variable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }

        public V3Reference()
        { }

        public V3Reference(Vector3 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector3 value => useConstant ? constantValue : variable.value;

        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                hasSearchedForAsset = false;
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }

        public static implicit operator Vector3(V3Reference reference)
        {
            return reference.value;
        }
    }
}