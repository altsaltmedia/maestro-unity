/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class ColorReference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public Color ConstantValue;
        public ColorVariable Variable;

        public ColorReference()
        { }

        public ColorReference(Color value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public Color Value {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator Color(ColorReference reference)
        {
            return reference.Value;
        }
    }
}