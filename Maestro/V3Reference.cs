using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V3Reference : VariableReferenceBase
    {
        public bool UseConstant = false;
        public Vector3 ConstantValue;
        public V3Variable Variable;

        public V3Reference()
        { }

        public V3Reference(Vector3 value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public Vector3 Value {
            get { return UseConstant ? ConstantValue : Variable.value; }
        }

        public static implicit operator Vector3(V3Reference reference)
        {
            return reference.Value;
        }
    }
}