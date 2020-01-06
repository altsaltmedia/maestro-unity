using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public abstract class VariableBase : RegisterableScriptableObject
    {
        [SerializeField]
        private bool _hasDefault;

        public bool hasDefault
        {
            get => _hasDefault;
            set => _hasDefault = value;
        }
        public abstract void SetDefaultValue();
    }
}