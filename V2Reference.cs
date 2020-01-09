using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V2Reference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Vector2 _constantValue;

        public Vector2 constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private V2Variable _variable;

        public V2Variable variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as V2Variable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }

        public V2Reference()
        { }

        public V2Reference(Vector2 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector2 value => useConstant ? constantValue : variable.value;

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

        public static implicit operator Vector2(V2Reference reference)
        {
            return reference.value;
        }
    }
}