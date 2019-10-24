/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;

namespace AltSalt
{
    [Serializable]
    public class LongReference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public long ConstantValue;
        public LongVariable Variable;

        public LongReference()
        { }

        public LongReference(long value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public long Value
        {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator long(LongReference reference)
        {
            return reference.Value;
        }
    }
}