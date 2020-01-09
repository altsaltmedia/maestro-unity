using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class VariableReference : ReferenceBase
    {
        [FormerlySerializedAs("UseConstant")]
        [SerializeField]
        private bool _useConstant = false;

        public bool useConstant
        {
            get => _useConstant;
            set => _useConstant = value;
        }

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"TRUE", true },
            {"FALSE", false }
        };
    }
}