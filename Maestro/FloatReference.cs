/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatReference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public float ConstantValue;
        public FloatVariable Variable;

        public FloatReference()
        { }

        public FloatReference(float value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public float Value
        {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}