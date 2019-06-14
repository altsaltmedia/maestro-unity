﻿using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Vector3 Variable")]
    public class V3Variable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Vector3 Variable")]
        string DeveloperDescription = "";
#endif
        public Vector3 Value;

        public void SetValue(Vector3 value)
        {
            Value = value;
        }

        public void SetValue(V3Variable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(Vector3 amount)
        {
            Value += amount;
        }

        public void ApplyChange(V3Variable amount)
        {
            Value += amount.Value;
        }
    }
}