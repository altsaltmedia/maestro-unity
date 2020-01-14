using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public abstract class ModifiableEditorVariable : SimpleSignal
    {
        [SerializeField]
        private bool _hasDefault;

        public bool hasDefault
        {
            get => _hasDefault;
            set => _hasDefault = value;
        }
        public abstract void SetToDefaultValue();
    }
}