/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;

namespace AltSalt
{
    [Serializable]
    public class IntReference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public int ConstantValue;
        public IntVariable Variable;

        public IntReference()
        { }

        public IntReference(int value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public int Value
        {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}