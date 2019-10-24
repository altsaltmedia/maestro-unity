using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class V2Reference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public Vector2 ConstantValue;
        public V2Variable Variable;

        public V2Reference()
        { }

        public V2Reference(Vector2 value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public Vector2 Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator Vector2(V2Reference reference)
        {
            return reference.Value;
        }
    }
}