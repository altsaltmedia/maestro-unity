﻿using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Vector2 Variable")]
    public class V2Variable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Vector2 Variable")]
        string DeveloperDescription = "";
#endif
        public Vector2 Value;

        public void SetValue(Vector2 value)
        {
            Value = value;
        }

        public void SetValue(V2Variable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(Vector2 amount)
        {
            Value += amount;
        }

        public void ApplyChange(V2Variable amount)
        {
            Value += amount.Value;
        }
    }
}