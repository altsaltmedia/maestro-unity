using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
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
        
        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private ToggleState _resetOnGameRefresh = ToggleState.NO;

        public ToggleState resetOnGameRefresh
        {
            get => _resetOnGameRefresh;
            set => _resetOnGameRefresh = value;
        }

        private void OnEnable()
        {
            if (resetOnGameRefresh == ToggleState.YES) {
                StoreCaller(this, "setting from OnEnable", "default functionality");
                SetToDefaultValue(); 
            }
        }
        
        public abstract void SetToDefaultValue();
    }
    
}