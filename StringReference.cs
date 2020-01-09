using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class StringReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private string _constantValue;

        public string constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private StringVariable _variable;

        public StringVariable variable
        {
            get
            {
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as StringVariable;
                }
                return _variable;
            }
            set => _variable = value;
        }

        public StringReference()
        { }

        public StringReference(string value)
        {
            useConstant = true;
            constantValue = value;
        }

        public string value => useConstant ? constantValue : variable.value;

        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }

        public static implicit operator string(StringReference reference)
        {
            return reference.value;
        }
    }
}