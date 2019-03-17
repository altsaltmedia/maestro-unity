using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt 
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
        
        [EnableIf("materialType", MaterialAttributeType.Color)]
        [ValidateInput("IsPopulated")]
        public ColorReference _Color;
        
        [EnableIf("materialType", MaterialAttributeType.Float)]
        [ValidateInput("IsPopulated")]
        public FloatReference _Float;
        
        private bool IsPopulated(ColorReference attribute)
        {
            if (materialType == MaterialAttributeType.Color) {
                if (attribute.UseConstant == true) {
                    return true;
                }
                else {
                    return attribute.Variable == null ? false : true;
                }
            }
            else {
                return true;
            }
        }
        
        private bool IsPopulated(FloatReference attribute)
        {
            if (materialType == MaterialAttributeType.Float) {
                if (attribute.UseConstant == true) {
                    return true;
                }
                else {
                    return attribute.Variable == null ? false : true;
                }
            }
            else {
                return true;
            }
        }
        
    }
    
}