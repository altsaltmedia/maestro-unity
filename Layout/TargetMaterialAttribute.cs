using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout 
{
    [Serializable]
    public class TargetMaterialAttribute
    {
        public string targetAttributeName;
        
        [ValueDropdown("materialTypeValues")]
        public MaterialAttributeType materialType;
        
        [ValidateInput("IsPopulated")]
        ValueDropdownList<MaterialAttributeType> materialTypeValues = new ValueDropdownList<MaterialAttributeType>(){
            {"Color", MaterialAttributeType.Color },
            {"Float", MaterialAttributeType.Float }
        };
        
        [FormerlySerializedAs("_Color")]
        [EnableIf("materialType", MaterialAttributeType.Color)]
        [ValidateInput("IsPopulated")]
        public ColorReference _colorValue;

        public ColorReference colorValue => _colorValue;

        [FormerlySerializedAs("_Float")]
        [EnableIf("materialType", MaterialAttributeType.Float)]
        [ValidateInput("IsPopulated")]
        [SerializeField]
        private FloatReference _floatValue;

        public FloatReference floatValue => _floatValue;

        private bool IsPopulated(ColorReference attribute)
        {
            if (materialType == MaterialAttributeType.Color) {
                if (attribute.useConstant == true) {
                    return true;
                }
                else {
                    return attribute.GetVariable(attribute.parentObject) == null ? false : true;
                }
            }
            else {
                return true;
            }
        }
        
        private bool IsPopulated(FloatReference attribute)
        {
            if (materialType == MaterialAttributeType.Float) {
                if (attribute.useConstant == true) {
                    return true;
                }
                else {
                    return attribute.GetVariable(attribute.parentObject) == null ? false : true;
                }
            }
            else {
                return true;
            }
        }
        
    }
    
}